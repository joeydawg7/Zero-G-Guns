using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Profiling;

public class RoundManager : MonoBehaviour
{
    #region singleton stuff
    private static RoundManager _instance;
    public static RoundManager Instance { get { return _instance; } }
    #endregion

    [Header("DEBUG")]
    public DebugManager debugManager;
    public RoomSO debugStayOnThisScene;
    public bool multiPlayer;
    public bool singlePlayer;
    [Header("----------")]


    public int roundsToWin;

    [Header("Round-Fluid elements")]
    GameObject newRoundElementBacker;
    TextMeshProUGUI newRoundText;
    TextMeshProUGUI roundNumText;

    Animator newRoundTextAnimator;

    private List<RoomSO> activeRooms;
    public List<RoomSO> ActiveRooms
    {
        get
        {
            var tempRooms = new List<RoomSO>();
            if (GameManager.Instance != null && GameModeFlag.Instance)
            {
                if (GameModeFlag.Instance.MultiPlayer == true)
                {
                    tempRooms = arenaRooms;
                }
                if (GameModeFlag.Instance.MultiPlayer == false)
                {
                    tempRooms = timedRooms;
                }
            }            
            else
            {                
                if (multiPlayer)
                {
                    tempRooms = arenaRooms;
                }

                if (singlePlayer)
                {
                    tempRooms = timedRooms;
                }

                if ((multiPlayer && singlePlayer) || (!multiPlayer && !singlePlayer))
                {
                    //Debug.Log("MULTIPLAYER AND/OR SINGLE PLAYER NOT SET PROPER ITS ALL RANDOM NOW");
                    tempRooms.AddRange(arenaRooms);
                    tempRooms.AddRange(timedRooms);
                }               
            }

            return tempRooms;
        }
        set
        {
            activeRooms = new List<RoomSO>(24);
            activeRooms = value;
        }
    }
    public List<RoomSO> arenaRooms;
    public List<RoomSO> timedRooms;

    public bool finishedControllerSetup;

    [HideInInspector]
    public int currentRound;

    public List<PlayerDataScript> playerDataList;
    public GameObject playerDataPrefab;
    
    public GameObject persistentCanvas;

    public GlobalPlayerSettingsSO globalPlayerSettings;
    public GameObject playerCanvas;
    public RoundEndCanvasScript roundEndCanvasScript;
    JoiningPlayerScript joiningPlayerScript;    

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;



            print("starting roundManager in scene " + SceneManager.GetActiveScene().name);
           
            foreach (var room in this.ActiveRooms)
            {
                room.isPlayable = true;
            }

            // ObjectPooler.Instance.StartUp();
        }

        finishedControllerSetup = false;

        currentRound = 0;
        

        Cursor.visible = false;

        joiningPlayerScript = FindObjectOfType<JoiningPlayerScript>();
        roundEndCanvasScript = FindObjectOfType<RoundEndCanvasScript>();


        roundEndCanvasScript.ClearEndRoundCanvasDisplay();
        globalPlayerSettings.SortPlayerSettings();

    }

    public void NewRound(bool startOver)
    {
        if (LoadingBar.Instance)
            LoadingBar.Instance.StopLoadingBar();
        Debug.Log("starting new round");
        loading = true;
        
        currentRound++;

        Time.timeScale = 1;
        ObjectPooler.Instance.ResetRound();

        if (startOver)
        {
            if (GameModeFlag.Instance)
                GameModeFlag.Instance.SetMusicClip(SoundPooler.Instance.levelSongs[0]);
            currentRound = 0;
            finishedControllerSetup = false;

            if (playerDataList.Count > 0)
            {
                for (int i = playerDataList.Count - 1; i >= 0; i--)
                {
                    Destroy(playerDataList[i]);
                }

                playerDataList.Clear();
                GameModeFlag.Instance.PlayMusic();
            }
        }
        else
        {
            if (GameModeFlag.Instance)
            {
                GameModeFlag.Instance.SetMusicClip(SoundPooler.Instance.levelSongs[1]);
                GameModeFlag.Instance.PlayMusic();
            }
        }

        //TODO: only grab from a list of playable rooms so player can check off maps they dont want to play
        //RoomSO nextRoom = rooms[Random.Range(0, rooms.Count)];

        List<RoomSO> tempRooms = new List<RoomSO>();

        RoomSO nextRoom = null;

        //we have no room to go to!
        while (nextRoom == null)
        {
            //make a list of all possible rooms we could go to that are playable
            foreach (var room in this.ActiveRooms)
            {
                if (room.isPlayable)
                {
                    tempRooms.Add(room);
                    // Debug.Log(room.name);
                }
            }

            //Debug.Log("---------------");

            //if our list has no playable rooms make everything playable
            if (tempRooms.Count < 1)
            {
                foreach (var room in this.ActiveRooms)
                {
                    room.isPlayable = true;
                }
            }
            else
            {
                //then pick a random room from everything playable
                nextRoom = tempRooms[Random.Range(0, tempRooms.Count)];
                nextRoom.isPlayable = false;
            }

        }


        StartCoroutine(AddLevel(nextRoom.sceneName, nextRoom, startOver));

        roundEndCanvasScript.ClearEndRoundCanvasDisplay();
    }

    public bool loading = false;

    IEnumerator AddLevel(string lvl, RoomSO nextRoom, bool startOver)
    {
        


        //DEBUG: use original scene
        if (debugStayOnThisScene!=null && debugManager.useDebugSettings)
        {
            lvl = debugStayOnThisScene.sceneName;

            //figure out which room we are in an set that as the real next room 
            for (int i = 0; i < this.ActiveRooms.Count; i++)
            {
                if (this.ActiveRooms[i].sceneName == lvl)
                {
                    nextRoom = this.ActiveRooms[i];
                    break;
                }
            }

        }


        AsyncOperation asyncLoadLevel = SceneManager.LoadSceneAsync(lvl);
        print("loading async now!");
        while (!asyncLoadLevel.isDone)
        {
            yield return null;
        }
        print("done loading!");


        LevelLoaded(nextRoom, startOver);
        yield return new WaitForSeconds(0.5f);

    }

    void LevelLoaded(RoomSO nextRoom, bool startOver)
    {
        if (!startOver)
        {
            newRoundElementBacker = GameObject.Find("roomSetupBacker");
            newRoundTextAnimator = newRoundElementBacker.GetComponent<Animator>();
            newRoundText = newRoundElementBacker.transform.Find("RoomNamePopup").GetComponent<TextMeshProUGUI>();

            newRoundText.alpha = 1;
            newRoundText.text = nextRoom.roomName;
            roundNumText = newRoundText.transform.Find("RoundNum").GetComponent<TextMeshProUGUI>();
            roundNumText.alpha = 1;
            roundNumText.text = "Round " + currentRound + "";

            newRoundTextAnimator.SetTrigger("NewRound");


            //find out who current winner is (if any) and set them to receive a crown
            int max = 0;
            for (int i = 0; i < playerDataList.Count; i++)
            {
                playerDataList[i].isCurrentWinner = false;

                if (playerDataList[i].roundWins > max)
                {
                    max = playerDataList[i].roundWins;
                }
            }

            //if we have a tie in who the current winner is, nobody gets a crown
            foreach (var pd in playerDataList)
            {
                if (pd.roundWins == max && max>0)
                    pd.isCurrentWinner = true;
            }



            foreach (var PD in playerDataList)
            {
                PD.SpawnAtMatchingPoint(globalPlayerSettings, playerCanvas);
            }
            GameManager.Instance.StartGame();
        }
        //else
        //    joiningPlayerScript.Start();







    }




    //public void SetPlayer(PlayerScript player)
    //{

    //    //only do setup if its the first round
    //    if (currentRound == 0)
    //    {

    //        PlayerDataScript PD = GameObject.Instantiate(playerDataPrefab).GetComponent<PlayerDataScript>();

    //        DontDestroyOnLoad(PD);

    //        PD.playerID = player.playerID;
    //        PD.controller = player.controller;
    //        PD.player = player.player;
    //        PD.hexColorCode = player.hexColorCode;
    //        PD.playerName = player.playerName;

    //        playerDataList.Add(PD);

    //    }

    //}

    public void SpawnPlayerManager(PlayerControllerData playerControllerData)
    {
        PlayerDataScript PD = GameObject.Instantiate(playerDataPrefab).GetComponent<PlayerDataScript>();
        DontDestroyOnLoad(PD);

        PD.SetPlayerInfoAfterRoundStart(playerControllerData, globalPlayerSettings);

        PD.SpawnAtMatchingPoint(globalPlayerSettings, playerCanvas);

        playerDataList.Add(PD);

    }



    private void Update()
    {
        //spins a fun lil' loading spinner       
    }


}
