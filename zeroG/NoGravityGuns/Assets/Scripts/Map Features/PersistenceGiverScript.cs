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

    public List<string> persistentStuff;
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
                go.DontDestroyOnLoad();
                persistentStuff.Add(go.name);
            }
                
        }

        ControllerLayoutManager.SwapToGameplayMaps();

        
        if (RoundManager.Instance)
        {
            Debug.Log("newround!");
            if (Photon.Pun.PhotonNetwork.IsConnected)
            {
                if (Photon.Pun.PhotonNetwork.IsMasterClient)
                {
                    Debug.Log("networked newround!");
                    NetworkingRoundManager.Instance.NewRound(true);
                }
            }
            else
            {
                Debug.Log("local newround!");
                RoundManager.Instance.NewRound(true, false);
            }
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

    void OnEnable()
    {
        //Tell our 'OnLevelFinishedLoading' function to start listening for a scene change as soon as this script is enabled.
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    void OnDisable()
    {
        //Tell our 'OnLevelFinishedLoading' function to stop listening for a scene change as soon as this script is disabled. Remember to always have an unsubscription for every delegate you subscribe to!
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
       // Debug.Log("Previous Level: " + PlayerPrefs.GetString("LastScene"));

    }
}
