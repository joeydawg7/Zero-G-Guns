using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Profiling;
using Rewired;
using Photon.Pun;
using Photon.Realtime;

public class NetworkingRoundManager : MonoBehaviourPunCallbacks, IInRoomCallbacks
{
    #region singleton stuff

    private static NetworkingRoundManager _instance;
    public static NetworkingRoundManager Instance { get { return _instance; } }
   
    #endregion

    #region private fields    

    #endregion

    #region Public Fields   

    ////public bool isGameLoaded;
    //public int currentScene;
    //public int multiplayerScene;
    //public GameObject[] playerPrefabs;   
    #endregion

    [Header("DEBUG")]
    public DebugManager debugManager;
    public RoomSO debugStayOnThisScene;
    public bool multiPlayer;
    public bool singlePlayer;
    [Space]


    public int roundsToWin;

    [Header("Round-Fluid elements")]
    GameObject newRoundElementBacker;
    TextMeshProUGUI newRoundText;
    TextMeshProUGUI roundNumText;

    Animator newRoundTextAnimator;

    public List<RoomSO> arenaRooms;

    public List<RoomSO> newRooms;
    public List<RoomSO> usedRooms;

    public bool finishedControllerSetup;

    Dictionary<int, PlayerSpawnPoint> AllSpawnPoint;

    [HideInInspector]
    public int currentRound;

    public GameObject playerDataPrefab;

    public GameObject persistentCanvas;

    public RoundEndCanvasScript roundEndCanvasScript;

    public List<PlayerDataScript> playerDatas;

    private RoomSO nextRoom;
    private bool startOver;


    private void Awake()
    {        
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;


            newRooms = new List<RoomSO>();
            newRooms.AddRange(arenaRooms);
            arenaRooms.Clear();
            usedRooms = new List<RoomSO>(newRooms.Count);          

            //foreach (var room in this.ActiveRooms)
            //{
            //    room.isPlayable = true;
            //}

            // ObjectPooler.Instance.StartUp();
        }

        finishedControllerSetup = false;

        currentRound = 0;


        Cursor.visible = false;

        roundEndCanvasScript = FindObjectOfType<RoundEndCanvasScript>();

        ControllerLayoutManager.SwapToGameplayMaps();

        roundEndCanvasScript.ClearEndRoundCanvasDisplay();
        AllSpawnPoint = new Dictionary<int, PlayerSpawnPoint>();
        DontDestroyOnLoad(this.gameObject);

    }

    public void NewRound(bool startOver)
    {
        //shuts off previous loading bar
        LoadingBar.Instance.StopLoadingBar();

        //increment round number
        currentRound++;

        Time.timeScale = 1;

        //if we are starting over, eg when someone has won 3 rounds and presses restart, or when loading from main menu
        if (startOver)
        {
            //play music with error checking
            if (SoundPooler.Instance.levelSongs.Count > 0)
                MusicManager.PlaySong(SoundPooler.Instance.levelSongs[0], true);

            //reset round num
            currentRound = 0;
            //unset controller stuff
            finishedControllerSetup = false;

            //TODO: clear existing player data
        }
        else
        {
            //play music if needed
            if (SoundPooler.Instance.levelSongs.Count > 0)
                MusicManager.PlaySong(SoundPooler.Instance.levelSongs[1], true);

        }



        //TODO: only grab from a list of playable rooms so player can check off maps they dont want to play

        RoomSO nextRoom = null;

        //gets a random next room that wasn't previously used until we've used them all, than randomize them up again
        if (newRooms.Count > 1)
        {
            nextRoom = newRooms[Random.Range(0, newRooms.Count)];
            newRooms.Remove(nextRoom);
            usedRooms.Add(nextRoom);

        }
        else
        {
            nextRoom = newRooms[Random.Range(0, newRooms.Count)];
            newRooms.Remove(nextRoom);
            newRooms.AddRange(usedRooms);
            usedRooms.Clear();
            usedRooms.Add(nextRoom);
        }

        StartCoroutine(AddLevel(nextRoom.sceneName, nextRoom, startOver));

        roundEndCanvasScript.ClearEndRoundCanvasDisplay();
    }


    IEnumerator AddLevel(string lvl, RoomSO nextRoom, bool startOver)
    {
        //DEBUG: use original scene
        if (debugStayOnThisScene != null && debugManager.useDebugSettings)
        {
            nextRoom = debugStayOnThisScene;
        }

        ObjectPooler.Instance.ResetRound();

        if (PhotonNetwork.IsMasterClient)
        {
            var netUserDatas = FindObjectsOfType<NetworkUserData>();
            foreach (var players in netUserDatas)
            {
                PhotonView PV = players.GetComponent<PhotonView>();
                PV.RPC("RPC_LoadLevelAcrossNetwork", RpcTarget.AllBuffered,lvl);
            }
            //PhotonNetwork.LoadLevel(lvl);//asyncLoadLevel = SceneManager.LoadSceneAsync(lvl);
           

            this.nextRoom = nextRoom;
            this.startOver = startOver;
        }

        ObjectPooler.Instance.ResetRound();

        yield return new WaitForSeconds(0.5f);
    }


    public override void OnEnable()
    {
        base.OnEnable();
        PhotonNetwork.AddCallbackTarget(this);
        SceneManager.sceneLoaded += OnSceneFinishedLoading;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        PhotonNetwork.RemoveCallbackTarget(this);
        SceneManager.sceneLoaded -= OnSceneFinishedLoading;
    }

    void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode) //LevelLoaded(RoomSO nextRoom, bool startOver)
    {
        if (scene.name == "NetworkPlayerJoinScreen")
        {
            Debug.Log("Finished Loading " + scene.name);
        }
        else if (scene.name == "Networking_PersistentScene")
        {
            Debug.Log("Finished Loading " + scene.name);
            NewRound(true);
        }
        else
        {
            Debug.Log("Finished Loading " + scene.name);

            newRoundElementBacker = GameObject.Find("roomSetupBacker");
            newRoundTextAnimator = newRoundElementBacker.GetComponent<Animator>();
            newRoundText = newRoundElementBacker.transform.Find("RoomNamePopup").GetComponent<TextMeshProUGUI>();

            newRoundText.alpha = 1;
            newRoundText.text = string.Empty;
            //newRoundText.text = nextRoom.roomName;
            roundNumText = newRoundText.transform.Find("RoundNum").GetComponent<TextMeshProUGUI>();
            roundNumText.alpha = 1;
            roundNumText.text = "Round " + currentRound + "";

            newRoundTextAnimator.SetTrigger("NewRound");

            ControllerLayoutManager.SwapToUIMaps(true);
            ControllerLayoutManager.SwapToGameplayMaps();

            //TODO: seperate the newRound functions to work with the new PLayerJoining Menu

            //SpawnPlayers();
            //EventManager.Instance.SpawnPlayer();

            if (GameManager.Instance)
            {
                GameManager.Instance.StartGame();
            }
            else
            {
                Debug.Log("Game Manager Null");
                Debug.Log("KeyNotFoundException as GameObject Manager named = " + GameObject.FindGameObjectWithTag("GameManager").name);
            }

            nextRoom = null;
            startOver = false;
        }
    }

    
}
