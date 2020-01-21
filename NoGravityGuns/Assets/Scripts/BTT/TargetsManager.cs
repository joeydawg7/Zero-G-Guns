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

    string timeInString;

    private void Start()
    {
        timeInString = "";

        List<TargetScript> targetsList = FindObjectsOfType<TargetScript>().ToList();

        foreach (var t in targetsList)
        {
            targets.Add(t.gameObject);
            GameObject.Instantiate(BTT_Manager.Instance.targetUIImagePrefab, BTT_Manager.Instance.targetsToDestroy.transform);
        }

        //this is just here to prevent errors from running from this scene instead of persistent scene. Evntually the persistant scene will load and this scene will restart and the problem fixes itself
        try
        {
            if (Camera.main.GetComponentInParent<CameraController>() != null)
                Camera.main.GetComponentInParent<CameraController>().setToMaxZoom = true;
        }
        catch
        {
            //haha lol
        }

        //show timer
        timerTextMesh.gameObject.SetActive(true);

    }

    public void TargetDestroyed(Transform lockOnTarget)
    {
        StartCoroutine(WaitThenCheckNumTargets(lockOnTarget));
    }

    /// <summary>
    /// pauses for a tenth of a second to allow the target to be properly destroyed, then determines if the last target has been destroyed
    /// </summary>
    /// <param name="lockOnTarget"></param>
    /// <returns></returns>
    IEnumerator WaitThenCheckNumTargets(Transform lockOnTarget)
    {
        //play a sound for target breaks
        SoundPooler.Instance.PlaySoundEffect(targetShatter);

        yield return new WaitForSeconds(0.1f);

        //reorder the list so that it only contains existing targets
        targets = targets.Where(item => item != null).ToList();

        Destroy(BTT_Manager.Instance.targetsToDestroy.transform.GetChild(0).gameObject);

        //if there are no existing targets, game is over
        if (targets.Count == 0)
        {
            //cut the timer
            GameManager.Instance.stopTimer = true;

            //show a slowmo zoom effect at the particle effect that plays
            GameManager.Instance.cameraController.TrackFinalBlow(lockOnTarget, 2f, PlayerScript.DamageType.self, GameManager.Instance.pistol);

            yield return new WaitForSeconds(0.25f);
            BTT_Manager.Instance.BTTEndCanvas.ShowEndScreen(timer, false);


        }
        //there still more targets to shoot!
        else
        {
            yield break;
        }

    }

    float timer;

    public void DeadPlayer(Transform player)
    {
        //cut the timer
        GameManager.Instance.stopTimer = true;

        //show a slowmo zoom effect at the particle effect that plays
        GameManager.Instance.cameraController.TrackFinalBlow(player, 2f, PlayerScript.DamageType.self, GameManager.Instance.pistol);

        BTT_Manager.Instance.BTTEndCanvas.ShowEndScreen(timer, true);
    }

    private void Update()
    {
        timer = GameManager.Instance.timeSinceRoundStarted;

        //after an hour the timer screws up, so we stop tracking
        if (timer < 3600)
        {
            timeInString = Extensions.FloatToTime(timer, "#0:00.000");
            timerTextMesh.text = timeInString;
        }
    }
}
