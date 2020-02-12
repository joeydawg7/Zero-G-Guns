using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Audio;


public class CameraController : MonoBehaviour
{
    [HideInInspector]
    public List<Transform> players;

    public Vector3 offset;
    public float minSmoothTime;
    public float maxSmoothTime;
    float smoothTime;

    public float minZoom;
    public float maxZoom;
    public float zoomLimit;
    public float zoomSpeed;

    public float minDistToCapY;
    public float yMax;
    public float yMin;
    //public float xMax;
    //public float xMin;

    Vector3 velocity;

    CameraShake cameraShake;
    Camera mainCam;

    AudioSource audioSource;
    public AudioMixer SFXMixer;

    public AudioClip timeSlow;
    public AudioClip timeSpeed;

    [HideInInspector]
    public PostProcessVolume CurrentGameVolume;

    LensDistortion lensDistortion;
    ChromaticAberration chromaticAbberation;

    public bool setToMaxZoom = false;

    private void Awake()
    {
        setToMaxZoom = false;
        players = new List<Transform>(4);
        mainCam = Camera.main;
        cameraShake = mainCam.GetComponent<CameraShake>();
        audioSource = GetComponent<AudioSource>();
        CurrentGameVolume = FindObjectOfType<PostProcessVolume>();

        CurrentGameVolume.profile.TryGetSettings(out lensDistortion);
        CurrentGameVolume.profile.TryGetSettings(out chromaticAbberation);

        smoothTime = minSmoothTime;


        if(lensDistortion == null || chromaticAbberation == null)
        {
            Debug.LogError("Lens distortion or chromatic abberation not found on current post processing profile!");
        }

    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (players.Count == 0)
            return;

        Move();
        Zoom();

    }

    void Move()
    {
        Vector3 centerPoint = GetCenterPoint();
        Vector3 newPosition = centerPoint + offset;
        transform.position = Vector3.SmoothDamp(transform.position, newPosition, ref velocity, smoothTime);
    }

    void Zoom()
    {
        if (setToMaxZoom)
        {
            float newZoom = Mathf.Lerp(maxZoom, 1f, 1f / zoomLimit);
             mainCam.fieldOfView = Mathf.Lerp(mainCam.fieldOfView, newZoom, Time.deltaTime * zoomSpeed);
        }
        else
        {
            float newZoom = Mathf.Lerp(maxZoom, minZoom, GetGreatestDistance() / zoomLimit);
            mainCam.fieldOfView = Mathf.Lerp(mainCam.fieldOfView, newZoom, Time.deltaTime * zoomSpeed);
        }
    }

    Vector3 GetCenterPoint()
    {
        if (players.Count == 1)
        {
            return players[0].position;
        }

        var bounds = new Bounds(players[0].position, Vector3.zero);

        for (int i = 0; i < players.Count; i++)
        {
            bounds.Encapsulate(players[i].position);
        }

        Vector3 centerPoint = bounds.center;
        if(GetGreatestDistance() > minDistToCapY)
        {
            smoothTime = minSmoothTime;
            if (bounds.center.y > 0)
            {
                if (bounds.center.y > yMax)
                {
                    centerPoint.y = yMax;
                }
            }
            else if (bounds.center.y < 0)
            {
                if (bounds.center.y < yMin)
                {
                    centerPoint.y = yMin;
                }
            }
        }
        else
        {
            smoothTime = maxSmoothTime;
        }
             

        return centerPoint;
    }

    //returns between 0 and 1
    float GetGreatestDistance()
    {
        var bounds = new Bounds(players[0].position, Vector3.zero);
        for (int i = 0; i < players.Count; i++)
        {
            bounds.Encapsulate(players[i].position);
        }

        return Mathf.Max(bounds.size.x, bounds.size.y);
    }

    public void OnGameStart()
    {
        StartCoroutine(DelayOnGameStart());
    }


    //grab all the players from the scene
    IEnumerator DelayOnGameStart()
    {
        players.Clear();

        yield return new WaitForSeconds(0.25f);

        PlayerScript[] playersArray = FindObjectsOfType<PlayerScript>();

        for (int i = 0; i < playersArray.Length; i++)
        {
            players.Add(playersArray[i].rb.transform);
        }
    }

    public void RemovePlayerFromCameraTrack(GameObject player, float time)
    {
        StartCoroutine(RemovePlayerFromCameraTrackAfterTime(player, time));
    }

    IEnumerator RemovePlayerFromCameraTrackAfterTime(GameObject player, float time)
    {
        

        cameraShake.shakeDuration += 0.5f;
        yield return new WaitForSeconds(time);
        cameraShake.shakeDuration = 0.0f;
        Time.timeScale = 1f;

        if (players.Contains(player.transform))
            players.Remove(player.transform);
        else
        {
            Debug.LogError("Could not find gameobject " + player.name + " from camera list!");
            yield break;
        }
    }

    public void TrackFinalBlow(Transform playerWhoWasHit, float timeToHold, PlayerScript.DamageType damageType, Guns gunWhoShotYou)
    {
        StartCoroutine(HoldOnFinalBlow(playerWhoWasHit, timeToHold, damageType, gunWhoShotYou));
    }

    bool holdingOnFinalBlow = false;

    IEnumerator HoldOnFinalBlow(Transform playerWhoWasHit, float t, PlayerScript.DamageType damageType, Guns gunWhoShotYou)
    {
        holdingOnFinalBlow = true;
        setToMaxZoom = false;

        //store old data about who we were tracking, then clear who we are tracking and set only the hit player so we zoom in on them
        var lastPlayerStanding = players[0];
        Transform[] trackedPlayers = players.ToArray();
        players.Clear();
        players.Add(playerWhoWasHit);

        //play a whoosh effect
        audioSource.PlayOneShot(timeSlow);

        //make all SFX pitched down
        SFXMixer.SetFloat("SFXPitch", 0.75f);

        //mess with time
        while (Time.timeScale >0.1f)
        {
            Time.timeScale -= 0.1f;
            Time.fixedDeltaTime = 0.02f * Time.deltaTime;
            yield return null;
        }
        Time.timeScale = 0.1f;
        Time.fixedDeltaTime = 0.02f * Time.deltaTime;

        

        //initial slowdown last 0.3 /0.1 due to slow down seconds (3s) and scales up post effects during that time
        float timer = 0;
        while (timer < 0.2f)
        {
            chromaticAbberation.intensity.value += Time.deltaTime*5f;
            lensDistortion.scale.value -= Time.deltaTime*5f;

            timer += Time.deltaTime;

            yield return null;
        }

        //empties out player tracking and then adds everyone back so we return to previous view
        players.Clear();
        for (int i = 0; i < trackedPlayers.Length; i++)
        {
            players.Add(trackedPlayers[i]);
        }
        //
        
        //additional wait
        yield return new WaitForSeconds(1 * Time.timeScale);
        //starts showing the end game GUI
        GameManager.Instance.OnGameEnd();

        if (RoundManager.Instance)
            RoundManager.Instance.roundEndCanvasScript.EndRoundCanvasDisplay(playerWhoWasHit, damageType, gunWhoShotYou);


        //make all SFX pitched normal
        SFXMixer.SetFloat("SFXPitch", 1f);

        //plays another woosh effect
        audioSource.PlayOneShot(timeSpeed);


        //ramps up return to normal time
        while (Time.timeScale < 1)
        {
            Time.timeScale += 0.05f;
            Time.fixedDeltaTime = 0.02f*Time.deltaTime;
            chromaticAbberation.intensity.value -= Time.deltaTime * 5f;
            lensDistortion.scale.value += Time.deltaTime * 5f;
            yield return null;
        }

       

        //sets everything back to normal
        ResetAllSlowdownEffects();


    }

    public void ResetAllSlowdownEffects()
    {     

        chromaticAbberation.intensity.value = 0f;
        lensDistortion.intensity.value = 0;
        Time.timeScale = 1;
        Time.fixedDeltaTime = 0.02f;

        //make all SFX pitched normal
        SFXMixer.SetFloat("SFXPitch", 1f);
        players.Clear();
    }


   


}
