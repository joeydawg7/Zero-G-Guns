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
            //only need to set parent objects to DDOL
            if(go.transform.parent ==null)
                DontDestroyOnLoad(go);
        }

        Debug.Log("starting new round from persistant scene");

        print(RoundManager.Instance.ActiveRooms[0].roomName);
        RoundManager.Instance.NewRound(true);

    }
}
