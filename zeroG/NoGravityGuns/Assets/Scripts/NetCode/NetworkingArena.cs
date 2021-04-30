using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using UnityEngine.SceneManagement;

public class NetworkingArena : MonoBehaviourPunCallbacks, IInRoomCallbacks
{
    #region Public Fields

    public static NetworkingArena arena;    

    //public bool isGameLoaded;   
    public GameObject[] playerPrefabs;

    //public int playersInRoom;
    //public int myNumberInRoom;
        
    //public int playersInGame;
    #endregion

    #region Private Fields

    //private PlayerDataScript[] players;
    

    #endregion

    #region Unity Code

    void Awake()
    {
        if(NetworkingArena.arena == null)
        {
            NetworkingArena.arena = this;
        }
        else
        {
            if(NetworkingArena.arena != this)
            {
                Destroy(NetworkingArena.arena.gameObject);
                NetworkingArena.arena = this;
            }
        }
        DontDestroyOnLoad(this.gameObject);
    }


    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //public override void OnEnable()
    //{
    //    base.OnEnable();
    //    PhotonNetwork.AddCallbackTarget(this);
    //    SceneManager.sceneLoaded += OnSceneFinishedLoading;
    //}

    //public override void OnDisable()
    //{
    //    base.OnDisable();
    //    PhotonNetwork.RemoveCallbackTarget(this);
    //    SceneManager.sceneLoaded -= OnSceneFinishedLoading;
    //}

    //public void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode)
    //{
    //    currentScene = scene.buildIndex;
    //    if(currentScene ==  multiplayerScene)
    //    {
    //        CreatePlayer();
    //    }
    //}

    public void StartGame(int multiplayerArenaID)
    {
        if(!PhotonNetwork.IsMasterClient)
        {
            return;
        }

        PhotonNetwork.LoadLevel(multiplayerArenaID);
        
    }

    #endregion

    #region custom Methods

    private void CreatePlayer()
    {
        PhotonNetwork.Instantiate(playerPrefabs[0].name, Vector3.zero, Quaternion.identity);
    }

    #endregion

}
