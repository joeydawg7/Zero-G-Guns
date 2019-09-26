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


    float startRotation;
    float endRotation;
    float t;
    float FinalZRot;

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

            //set rotation to 0, set endpoint to 360 degrees later
            startRotation = 0f;
            endRotation = startRotation + 360.0f;
            t = 0.0f;
            FinalZRot = 0;

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

    }

    public void NewRound(bool startOver)
    {
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

        nextRoom.isPlayable = false;

        StartCoroutine(AddLevel(nextRoom.sceneName, nextRoom, startOver));

    }

    bool loading = false;

    IEnumerator AddLevel(string lvl, RoomSO nextRoom, bool startOver)
    {
        loadingImage.gameObject.SetActive(true);

        AsyncOperation asyncLoadLevel = SceneManager.LoadSceneAsync(lvl);

        while (!asyncLoadLevel.isDone)
        {
            yield return null;
        }

        LevelLoaded(nextRoom, startOver);
        yield return new WaitForSeconds(0.5f);

        //loading = false;

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
            
            GameManager.Instance.StartGame();
        }


    }


    public void SetAllPlayersDataIntoPlayerObjects()
    {
        for (int i = 0; i < playerDataList.Count; i++)
        {
            playerDataList[i].SetPlayerInfoAfterRoundStart();
        }

    }

    public void SetPlayer(PlayerScript player)
    {

        //only do setup if its the first round
        if (currentRound == 0)
        {

            PlayerDataScript PD = GameObject.Instantiate(playerDataPrefab).GetComponent<PlayerDataScript>();

            DontDestroyOnLoad(PD);

            PD.playerID = player.playerID;
            PD.controller = player.controller;
            PD.player = player.player;
            PD.hexColorCode = player.hexColorCode;
            PD.playerName = player.playerName;

            playerDataList.Add(PD);

        }

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
