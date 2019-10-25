using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PersistenceGiverScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

        GameObject[] objects = FindObjectsOfType<GameObject>();

        foreach (var go in objects)
        {
            DontDestroyOnLoad(go);
        }

        Debug.Log("starting new round from persistant scene");
        RoundManager.Instance.NewRound(true);

    }


}
