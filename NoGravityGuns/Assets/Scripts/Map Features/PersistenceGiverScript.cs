using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PersistenceGiverScript : MonoBehaviour
{
    #region singleton stuff
    private static PersistenceGiverScript _instance;
    public static PersistenceGiverScript Instance { get { return _instance; } }
    #endregion

    List<string> persistentStuff;
    // Start is called before the first frame update
    void Start()
    {
        if(_instance != null)
        {
            Destroy(this);
        }
        else
        {
            _instance = this;
        }

        persistentStuff = new List<string>();
        GameObject[] objects = FindObjectsOfType<GameObject>();

        foreach (var go in objects)
        {
            //only need to set parent objects to DDOL
            if(go.transform.parent ==null)
            {
                DontDestroyOnLoad(go);
                persistentStuff.Add(go.name);
            }
                
        }

        Debug.Log("starting new round from persistant scene");

        //print(RoundManager.Instance.arenaRooms[0].roomName);
        if(RoundManager.Instance)
        {
            RoundManager.Instance.NewRound(true);
        }
        else if (BTT_Manager.Instance)
        {
            BTT_Manager.Instance.NewBTT_Level(0);
        }
    }

    public void PersistenceTaker()
    {
        GameObject[] objects = FindObjectsOfType<GameObject>();

        foreach (var go in objects)
        {
            if(persistentStuff.Contains(go.name))
            {
                Destroy(go);
            }
        }
    }
}
