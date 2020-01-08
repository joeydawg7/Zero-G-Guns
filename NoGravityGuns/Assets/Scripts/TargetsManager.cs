using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class TargetsManager : MonoBehaviour
{
    //[HideInInspector]
    public List<GameObject> targets;
    public int numTargetsToDestroy;
    public int targetsDestroyed;
    private void Start()
    {
        List<TargetScript> targetsList = FindObjectsOfType<TargetScript>().ToList();

        foreach (var t in targetsList)
        {
            targets.Add(t.gameObject);
        }

        numTargetsToDestroy = targets.Count;
        try
        {
            if (Camera.main.GetComponentInParent<CameraController>() != null)
                Camera.main.GetComponentInParent<CameraController>().setToMaxZoom = true;
        }
        catch
        {
            //haha lol
        }
    }

    public void TargetDestroyed()
    {
        StartCoroutine(WaitThenCheckNumTargets());
       
    }

    IEnumerator WaitThenCheckNumTargets()
    {
        yield return new WaitForSeconds(0.1f);

        targets = targets.Where(item => item != null).ToList();

        if (targets.Count ==0)
        {
            //end round here!
            //SceneManager.LoadScene("PersistentScene");
            Debug.Log("you weeen!");
            Time.timeScale = 0;
            
        }



    }


}
