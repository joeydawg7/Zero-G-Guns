using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    public List<RoomSO> BTT_Rooms;   

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
        }

        finishedControllerSetup = false;

        currentRound = 0;

        Cursor.visible = false;

        joiningPlayerScript = FindObjectOfType<JoiningPlayerScript>();
        roundEndCanvasScript = FindObjectOfType<RoundEndCanvasScript>();

        roundEndCanvasScript.ClearEndRoundCanvasDisplay();
        globalPlayerSettings.SortPlayerSettings();
    }

    public void NewBTT_Level(int nextRoomID)
    {
        NewBTT_Room(BTT_Rooms[nextRoomID]);
    }

    private void NewBTT_Room(RoomSO nextRoom)
    {
        StartCoroutine(AddLevel(nextRoom.sceneName, nextRoom));       

        roundEndCanvasScript.ClearEndRoundCanvasDisplay();
    }

    public bool loading = false;

    IEnumerator AddLevel(string lvl, RoomSO nextRoom)
    {
        AsyncOperation asyncLoadLevel = SceneManager.LoadSceneAsync(lvl);
        print("loading async now!");
        while (!asyncLoadLevel.isDone)
        {
            yield return null;
        }
        print("done loading!");

        LevelLoaded(nextRoom);
        yield return new WaitForSeconds(0.5f);
    }

    void LevelLoaded(RoomSO nextRoom)
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


    public void SpawnPlayerManager(PlayerControllerData playerControllerData)
    {
        PlayerDataScript PD = GameObject.Instantiate(playerDataPrefab).GetComponent<PlayerDataScript>();
        DontDestroyOnLoad(PD);

        PD.SetPlayerInfoAfterRoundStart(playerControllerData, globalPlayerSettings);

        PD.SpawnAtMatchingPoint(globalPlayerSettings, playerCanvas);

        playerDataList.Add(PD);

    }
}
