using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using TMPro;

public class TargetsManager : MonoBehaviour
{
    [HideInInspector]
    public List<GameObject> targets;
    public AudioClip targetShatter;
    public TextMeshProUGUI timerTextMesh;

    private void Start()
    {
        List<TargetScript> targetsList = FindObjectsOfType<TargetScript>().ToList();

        foreach (var t in targetsList)
        {
            targets.Add(t.gameObject);
        }

        try
        {
            if (Camera.main.GetComponentInParent<CameraController>() != null)
                Camera.main.GetComponentInParent<CameraController>().setToMaxZoom = true;
        }
        catch
        {
            //haha lol
        }

        

        timerTextMesh.gameObject.SetActive(true);
    }

    public void TargetDestroyed(Transform lockOnTarget)
    {
        StartCoroutine(WaitThenCheckNumTargets(lockOnTarget));

    }

    IEnumerator WaitThenCheckNumTargets(Transform lockOnTarget)
    {

        GameManager.Instance.stopTimer = true;

        SoundPooler.Instance.PlaySoundEffect(targetShatter);

        

        yield return new WaitForSeconds(0.1f);

        targets = targets.Where(item => item != null).ToList();

        if (targets.Count == 0)
        {
            //end round here!
            //SceneManager.LoadScene("PersistentScene");
            GameManager.Instance.cameraController.TrackFinalBlow(lockOnTarget, 2f, PlayerScript.DamageType.self, GameManager.Instance.pistol);
            Time.timeScale = 0;

        }

    }

    private void Update()
    {
        
        float timer = GameManager.Instance.timeSinceRoundStarted;

        if(timer < 3600)
            timerTextMesh.text = Extensions.FloatToTime(timer, "#0:00.000");

    }


}
