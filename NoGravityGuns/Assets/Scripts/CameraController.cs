﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class CameraController : MonoBehaviour
{
    public List<Transform> players;

    public Vector3 offset;
    public float smoothTime;

    public float minZoom;
    public float maxZoom;
    public float zoomLimit;
    public float zoomSpeed;

    Vector3 velocity;

    CameraShake cameraShake;
    Camera mainCam;

    AudioSource audioSource;

    public AudioClip timeSlow;
    public AudioClip timeSpeed;

    [HideInInspector]
    public PostProcessVolume CurrentGameVolume;

    private void Awake()
    {
        players = new List<Transform>();
        mainCam = Camera.main;
        cameraShake = mainCam.GetComponent<CameraShake>();
        audioSource = GetComponent<AudioSource>();
        CurrentGameVolume = FindObjectOfType<PostProcessVolume>();

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
        float newZoom = Mathf.Lerp(maxZoom, minZoom, GetGreatestDistance() / zoomLimit);
        mainCam.fieldOfView = Mathf.Lerp(mainCam.fieldOfView, newZoom, Time.deltaTime * zoomSpeed);
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

        return bounds.center;
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


    IEnumerator DelayOnGameStart()
    {
        yield return new WaitForSeconds(0.25f);

        GameObject[] playersArray = GameObject.FindGameObjectsWithTag("Player");

        for (int i = 0; i < playersArray.Length; i++)
        {
            players.Add(playersArray[i].transform);
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

    public void TrackFinalBlow(Transform playerWhoWasHit, float timeToHold)
    {
        StartCoroutine(HoldOnFinalBlow(playerWhoWasHit, timeToHold));
    }


    IEnumerator HoldOnFinalBlow(Transform playerWhoWasHit, float t)
    {
        //store old data about who we were tracking, then clear who we are tracking and set only the hit player so we zoom in on them
        Transform[] trackedPlayers = players.ToArray();
        players.Clear();
        players.Add(playerWhoWasHit);

        //play a whoosh effect
        audioSource.PlayOneShot(timeSlow);

        //swap profiles so we dont muck up old profile (probably not required)
        //CurrentGameVolume.profile = onSlowDownProfile;

        //gens distortion and chromatic abbberation effects
        LensDistortion lensDistortion;
        ChromaticAberration chromaticAbberation;
        CurrentGameVolume.profile.TryGetSettings(out lensDistortion);
        CurrentGameVolume.profile.TryGetSettings(out chromaticAbberation);

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
        while (timer < 0.3f)
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

        //starts showing the end game GUI
        GameManager.Instance.OnGameEnd();

        yield return new WaitForSeconds(1*Time.timeScale);

        //plays another woosh effect
        audioSource.PlayOneShot(timeSpeed);

        //ramps up return to normal time
        while (Time.timeScale < 1)
        {
            Time.timeScale += 0.01f;
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
        LensDistortion lensDistortion;
        ChromaticAberration chromaticAbberation;
        CurrentGameVolume.profile.TryGetSettings(out lensDistortion);
        CurrentGameVolume.profile.TryGetSettings(out chromaticAbberation);

        chromaticAbberation.intensity.value = 0f;
        lensDistortion.intensity.value = 0;
        Time.timeScale = 1;
        Time.fixedDeltaTime = 0.02f;
    }


}
