using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class RoundManager : MonoBehaviour
{
    #region singleton stuff
    private static RoundManager _instance;
    public static RoundManager Instance { get { return _instance; } }
    #endregion

    [Header("DEBUG")]
    public bool debugStayOnThisScene;
    [Header("----------")]


    public int maxRounds;

    [Header("Round-Fluid elements")]
    GameObject newRoundElementBacker;
    TextMeshProUGUI newRoundText;
    TextMeshProUGUI roundNumText;

    Animator newRoundTextAnimator;

    public List<RoomSO> rooms;

    public bool finishedControllerSetup;

    [HideInInspector]
    public int currentRound;
    [HideInInspector]
    public float timeSinceRoundStarted;

    public List<PlayerDataScript> playerDataList;
    public GameObject playerDataPrefab;
    public Image loadingImage;
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
            DontDestroyOnLoad(gameObject);
            DontDestroyOnLoad(persistentCanvas);
            DontDestroyOnLoad(loadingImage.gameObject);

            foreach (var room in rooms)
            {
                room.isPlayable = true;
            }

                // ObjectPooler.Instance.StartUp();
        }

        finishedControllerSetup = false;

        currentRound = 0;
        loadingImage.gameObject.SetActive(false);

        Cursor.visible = false;

        joiningPlayerScript = FindObjectOfType<JoiningPlayerScript>();
        roundEndCanvasScript = FindObjectOfType<RoundEndCanvasScript>();


        roundEndCanvasScript.ClearEndRoundCanvasDisplay();
        globalPlayerSettings.SortPlayerSettings();

    }

    public void NewRound(bool startOver)
    {
        Debug.Log("starting new round");
        loading = true;
        loadingImage.fillAmount = 0;
        currentRound++;
        timeSinceRoundStarted = 0;


        if (startOver)
        {
            currentRound = 0;
            finishedControllerSetup = false;

            if (playerDataList.Count > 0)
            {
                for (int i = playerDataList.Count-1; i >=0; i--)
                {
                    Destroy(playerDataList[i]);
                }

                playerDataList.Clear();
            }

        }

        //TODO: only grab from a list of playable rooms so player can check off maps they dont want to play
        //RoomSO nextRoom = rooms[Random.Range(0, rooms.Count)];

        List<RoomSO> tempRooms = new List<RoomSO>();

        RoomSO nextRoom = null;

        //we have no room to go to!
        while(nextRoom == null)
        {
            //make a list of all possible rooms we could go to that are playable
            foreach (var room in rooms)
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
                foreach (var room in rooms)
                {
                    room.isPlayable = true;
                }
            }
            else
            {
                //then pick a random room from everything playable
                nextRoom = tempRooms[Random.Range(0, tempRooms.Count)];
            }

        }


        StartCoroutine(AddLevel(nextRoom.sceneName, nextRoom, startOver));
       
        roundEndCanvasScript.ClearEndRoundCanvasDisplay();
    }

    bool loading = false;

    IEnumerator AddLevel(string lvl, RoomSO nextRoom, bool startOver)
    {
        loadingImage.gameObject.SetActive(true);


        //DEBUG: use original scene
        if (debugStayOnThisScene && GameManager.Instance.debugManager.useDebugSettings)
        {
            lvl = SceneManager.GetActiveScene().name;

            //figure out which room we are in an set that as the real next room 
            for (int i = 0; i < rooms.Count; i++)
            {
                if (rooms[i].sceneName == lvl)
                {
                    nextRoom = rooms[i];
                    break;
                }
            }

        }

        ObjectPooler.Instance.ResetRound();

        AsyncOperation asyncLoadLevel = SceneManager.LoadSceneAsync(lvl); 

        while (!asyncLoadLevel.isDone )
        {
            yield return null;
        }

        

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

            Debug.Log("spawning players");
            foreach (var PD in playerDataList)
            {
                PD.SpawnAtMatchingPoint(globalPlayerSettings, playerCanvas);
            }


            GameManager.Instance.StartGame();

           
        }


       

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
        if (loading)
        {
            loadingImage.fillAmount += Time.deltaTime;

            if(loadingImage.fillAmount>=1)
            {
                loadingImage.fillAmount = 0;
                loading = false;
            }

        }
        else
        {
            loadingImage.gameObject.SetActive(false);

            timeSinceRoundStarted += Time.deltaTime;

        }
    }

  
}
