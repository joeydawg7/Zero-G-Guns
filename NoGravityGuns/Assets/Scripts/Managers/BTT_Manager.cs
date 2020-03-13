using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Rewired;

public class BTT_Manager : MonoBehaviour
{
    #region singleton stuff
    private static BTT_Manager _instance;
    public static BTT_Manager Instance { get { return _instance; } }
    #endregion   

    [Header("Round-Fluid elements")]
    GameObject newRoundElementBacker;
    TextMeshProUGUI newRoundText;
    TextMeshProUGUI roundNumText;

    Animator newRoundTextAnimator;

    public List<BTT_RoomSO> BTT_Rooms;

    public bool finishedControllerSetup;

    [HideInInspector]
    public int currentRound;

    public List<PlayerDataScript> playerDataList;
    public GameObject playerDataPrefab;

    public GameObject persistentCanvas;

    public GlobalPlayerSettingsSO globalPlayerSettings;
    public GameObject playerCanvas;
    public BTTEndCanvas BTTEndCanvas;
    public GameObject targetsToDestroy;
    public GameObject targetUIImagePrefab;
    JoiningPlayerScript joiningPlayerScript;
    public BTT_RoomSO currentRoom;
    public RoomSO mainMenu;

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
        }

        finishedControllerSetup = false;

        currentRound = 0;

        Cursor.visible = false;

        //SpawnPlayerManager(ReInput.players.GetSystemPlayer());

        BTTEndCanvas = FindObjectOfType<BTTEndCanvas>();


        ClearAllControllers();

        globalPlayerSettings.SortPlayerSettings();
    }


    public void NewBTT_Level(int nextRoomID)
    {

        foreach (var bttroom in BTT_Rooms)
        {
            if (bttroom.playOnLoad)
            {
                currentRoom = bttroom;
                NewBTT_Room(bttroom);
                break;
            }
        }
    }

    public void BackToMenu()
    {

        ObjectPooler.Instance.ResetRound();
        DontDestroyOnLoadManager.DestroyAll();
        Time.timeScale = 1.0f;
        SceneManager.LoadScene("SplashScreen");
        //StartCoroutine(LoadSceneAsyncByName("SplashScreen"));
    }

    public void BackToPersistentScene()
    {
        
        Time.timeScale = 1.0f;

        //reset all pooled objects
        ObjectPooler.Instance.ResetRound();
        //then destroy them all!
        DontDestroyOnLoadManager.DestroyAll();

        SceneManager.LoadScene("BTT_PersistentScene");
        //StartCoroutine(LoadSceneAsyncByName("BTT_PersistentScene"));
    }

    IEnumerator LoadSceneAsyncByName(string s)
    {

        AsyncOperation asyncLoadLevel = SceneManager.LoadSceneAsync(s);

        ClearAllControllers();

        while (!asyncLoadLevel.isDone)
        {
            yield return null;
        }
        print("done loading!");

        DontDestroyOnLoadManager.DestroyAll();

        yield return new WaitForSeconds(0.5f);


    }


    void ClearAllControllers()
    {
        // Iterating through Players (excluding the System Player) and clearing any existing data on them
        for (int i = 0; i < ReInput.players.playerCount; i++)
        {
            Player p = ReInput.players.Players[i];

            p.controllers.ClearAllControllers();
        }
    }

    private void NewBTT_Room(BTT_RoomSO nextRoom)
    {
        StartCoroutine(AddLevel(nextRoom.sceneName, nextRoom));

        // roundEndCanvasScript.ClearEndRoundCanvasDisplay();
    }

    public bool loading = false;

    IEnumerator AddLevel(string lvl, RoomSO nextRoom)
    {
        AsyncOperation asyncLoadLevel = SceneManager.LoadSceneAsync(lvl);
        print("loading async now!");

        ClearAllControllers();

        ObjectPooler.Instance.ResetRound();

        while (!ObjectPooler.Instance.finishedResetting)
        {
            yield return null;
        }

        while (!asyncLoadLevel.isDone)
        {
            yield return null;
        }

        print("done loading!");

        yield return new WaitForSeconds(0.5f);

        LevelLoaded(nextRoom);
        yield return new WaitForSeconds(0.5f);
    }

    void LevelLoaded(RoomSO nextRoom)
    {
        Time.timeScale = 1;

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

        foreach (var PD in playerDataList)
        {
            PD.SpawnAtMatchingPoint(globalPlayerSettings, playerCanvas);
        }

        SetupP1Controller();
        
        CameraController cameraController = FindObjectOfType<CameraController>();
        cameraController.setToMaxZoom = true;

        GameManager.Instance.StartGame();
    }

    private void SetupP1Controller()
    {

        Player player1 = ReInput.players.GetPlayer(0);

        Joystick j = ReInput.controllers.GetController(ControllerType.Joystick, 0) as Joystick;

        player1.controllers.AddController(j, true);

        PlayerControllerData playerDataScript = new PlayerControllerData(0, j);

        SpawnPlayerManager(playerDataScript);
       
        finishedControllerSetup = true;

    }



    private void Update()
    {
        if (finishedControllerSetup)
        {
            //if either of these things are null we can't proceed. probably broken from loading outside of persistent scene and will fix itself shortly :D
            if (!GameManager.Instance)
            {
                Debug.LogError("No gameManager or Roundmanager found in BTT_Manager!");
                return;
            }

        }
    }

    public void SpawnPlayerManager(PlayerControllerData playerControllerData)
    {
        PlayerDataScript PD = GameObject.Instantiate(playerDataPrefab).GetComponent<PlayerDataScript>();
        gameObject.DontDestroyOnLoad();

        PD.SetPlayerInfoAfterRoundStart(playerControllerData, globalPlayerSettings);

        PD.SpawnAtMatchingPoint(globalPlayerSettings, playerCanvas);

        playerDataList.Add(PD);

        GameManager.Instance.StartGame();

    }
}
