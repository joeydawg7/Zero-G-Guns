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
    [Header("----------")]


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

        joiningPlayerScript = FindObjectOfType<JoiningPlayerScript>();
        roundEndCanvasScript = FindObjectOfType<RoundEndCanvasScript>();

        ControllerLayoutManager.SwapToGameplayMaps();

        roundEndCanvasScript.ClearEndRoundCanvasDisplay();
        globalPlayerSettings.SortPlayerSettings();

    }

    public void NewRound(bool startOver)
    {
        if (LoadingBar.Instance)
            LoadingBar.Instance.StopLoadingBar();
        loading = true;
        
        currentRound++;



        Time.timeScale = 1;
        //ObjectPooler.Instance.ResetRound();

        if (startOver)
        {

            CameraController cameraController = Camera.main.GetComponent<CameraController>();

            if (SoundPooler.Instance.levelSongs.Count >0 && cameraController)
                cameraController.GetComponent<AudioSource>().clip = SoundPooler.Instance.levelSongs[0];
            currentRound = 0;
            finishedControllerSetup = false;

            if (playerDataList.Count > 0)
            {
                for (int i = playerDataList.Count - 1; i >= 0; i--)
                {
                    Destroy(playerDataList[i]);
                }

                playerDataList.Clear();
                Camera.main.GetComponent<AudioSource>().Play();
                newRooms.AddRange(usedRooms);
                usedRooms.Clear();
            }
        }
        else
        {
            Camera.main.GetComponent<AudioSource>().clip = SoundPooler.Instance.levelSongs[1];
            Camera.main.GetComponent<AudioSource>().Play();
        }

        //TODO: only grab from a list of playable rooms so player can check off maps they dont want to play

        RoomSO nextRoom = null;

        if(newRooms.Count > 1)
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

    public bool loading = false;

    IEnumerator AddLevel(string lvl, RoomSO nextRoom, bool startOver)
    {
        

        //DEBUG: use original scene
        if (debugStayOnThisScene!=null && debugManager.useDebugSettings)
        {           
            nextRoom = debugStayOnThisScene;
        }

        ObjectPooler.Instance.ResetRound();


        AsyncOperation asyncLoadLevel = SceneManager.LoadSceneAsync(lvl);
        //print("loading async now!");
        while (!asyncLoadLevel.isDone)
        {
            yield return null;
        }
        //print("done loading!");
        
        LevelLoaded(nextRoom, startOver);


        ObjectPooler.Instance.ResetRound();

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
                if (pd.roundWins == max && max > 0)
                    pd.isCurrentWinner = true;
            }



            foreach (var PD in playerDataList)
            {
                PD.SpawnAtMatchingPoint(globalPlayerSettings, playerCanvas);
            }

            GameManager.Instance.StartGame();
        }



    }


    public void SpawnPlayerManager(PlayerControllerData playerControllerData)
    {
        PlayerDataScript PD = GameObject.Instantiate(playerDataPrefab).GetComponent<PlayerDataScript>();
        PD.gameObject.DontDestroyOnLoad();

        PD.SetPlayerInfoAfterRoundStart(playerControllerData, globalPlayerSettings);

        PD.SpawnAtMatchingPoint(globalPlayerSettings, playerCanvas);

        playerDataList.Add(PD);
    }


}
