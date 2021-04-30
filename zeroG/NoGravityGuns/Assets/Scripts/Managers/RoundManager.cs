using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Profiling;
using Rewired;

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

    public RoomSO localJoiningRoom;
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

    [HideInInspector]
    public int currentRound;

    public GameObject playerDataPrefab;

    public GameObject persistentCanvas;

    public RoundEndCanvasScript roundEndCanvasScript;

    public PlayerDataScript[] playerDatas;

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

            currentRound = 0;
        }

        finishedControllerSetup = false;


        Cursor.visible = false;

        roundEndCanvasScript = FindObjectOfType<RoundEndCanvasScript>();

        ControllerLayoutManager.SwapToGameplayMaps();

        roundEndCanvasScript.ClearEndRoundCanvasDisplay();

    }

    public void NewRound(bool startOver, bool loadToMenu)
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
            Debug.Log("starting over!");
        }
        else
        {
            //play music if needed
            if (SoundPooler.Instance.levelSongs.Count > 0)
                MusicManager.PlaySong(SoundPooler.Instance.levelSongs[1], true);

        }



        //TODO: only grab from a list of playable rooms so player can check off maps they dont want to play

        RoomSO nextRoom = null;

        if (loadToMenu)
        {
            //set nextRoom = playerJoinLobby
            nextRoom = localJoiningRoom;
        }
        else
        {
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


        AsyncOperation asyncLoadLevel = SceneManager.LoadSceneAsync(lvl);
        while (!asyncLoadLevel.isDone)
        {
            yield return null;
        }

        LevelLoaded(nextRoom, startOver);


        ObjectPooler.Instance.ResetRound();

        yield return new WaitForSeconds(0.5f);
    }

    void LevelLoaded(RoomSO nextRoom, bool startOver)
    {
        //dont need to do this if on joining lobby
        if (SceneManager.GetActiveScene().name == localJoiningRoom.sceneName)
            return;

        //if (!startOver)
        //{

        newRoundElementBacker = GameObject.Find("roomSetupBacker");
        newRoundTextAnimator = newRoundElementBacker.GetComponent<Animator>();
        newRoundText = newRoundElementBacker.transform.Find("RoomNamePopup").GetComponent<TextMeshProUGUI>();

        newRoundText.alpha = 1;
        newRoundText.text = nextRoom.roomName;
        roundNumText = newRoundText.transform.Find("RoundNum").GetComponent<TextMeshProUGUI>();
        roundNumText.alpha = 1;
        roundNumText.text = "Round " + (currentRound+1).ToString() + "";

        newRoundTextAnimator.SetTrigger("NewRound");

        //ControllerLayoutManager.SwapToUIMaps(true);
        ControllerLayoutManager.SwapToGameplayMaps();


        Debug.Log(GameManager.Instance.gameObject.name);
       
        GameManager.Instance.StartGame();

        /*

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
            if (pd.roundWins == max && max > 0)
                pd.isCurrentWinner = true;
        }




        */



        //}




    }

    //call the spawn function on our player Datas
    //void SpawnPlayers()
    //{
    //    PlayerDataScript[] pds = FindObjectsOfType<PlayerDataScript>();

    //    foreach (var PD in pds)
    //    {
    //        PD.SpawnAtMatchingPoint();
    //    }

    //    playerDatas = pds;
    //}





}
