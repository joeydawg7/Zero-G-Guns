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
    public TextMeshProUGUI newRoundText;
    TextMeshProUGUI roundNumText;

    Animator newRoundTextAnimator;

    public List<RoomSO> rooms;

    public bool finishedControllerSetup;

    [HideInInspector]
    public int currentRound;

    public List<PlayerDataScript> playerDataList;
    public GameObject playerDataPrefab;
    public GameObject loadingSpinner;
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
            DontDestroyOnLoad(loadingSpinner);

            //set rotation to 0, set endpoint to 360 degrees later
            startRotation = 0f;
            endRotation = startRotation + 360.0f;
            t = 0.0f;
            FinalZRot = 0;

        }

        finishedControllerSetup = false;

        currentRound = 0;
        loadingSpinner.SetActive(false);

    }

    public void NewRound(bool startOver)
    {
        loading = true;

        currentRound++;

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
        RoomSO nextRoom = rooms[Random.Range(0, rooms.Count)];

        StartCoroutine(AddLevel(nextRoom.sceneName, nextRoom, startOver));

    }

    bool loading = false;

    IEnumerator AddLevel(string lvl, RoomSO nextRoom, bool startOver)
    {
        loadingSpinner.SetActive(true);

        AsyncOperation asyncLoadLevel = SceneManager.LoadSceneAsync(lvl);

        while (!asyncLoadLevel.isDone)
        {
            yield return null;
        }

        LevelLoaded(nextRoom, startOver);
        yield return new WaitForSeconds(0.5f);

        loading = false;

    }

    void LevelLoaded(RoomSO nextRoom, bool startOver)
    {
        if (!startOver)
        {
            newRoundText = GameObject.Find("RoomNamePopup").GetComponent<TextMeshProUGUI>();
            newRoundTextAnimator = newRoundText.GetComponent<Animator>();
            newRoundText.alpha = 1;
            newRoundText.text = nextRoom.roomName;
            newRoundTextAnimator.SetTrigger("NewRound");
            roundNumText = newRoundText.transform.Find("RoundNum").GetComponent<TextMeshProUGUI>();
            roundNumText.text = "Round " + currentRound + ". First to 3 wins!";
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
            t += Time.deltaTime;
            //math magic
            float zRotation = Mathf.Lerp(startRotation, endRotation, t / 1.0f) % 360.0f;
            loadingSpinner.transform.eulerAngles = new Vector3(loadingSpinner.transform.eulerAngles.x, loadingSpinner.transform.eulerAngles.y, zRotation);
            FinalZRot = zRotation;
        }
        else
        {
            loadingSpinner.transform.eulerAngles = new Vector3(loadingSpinner.transform.eulerAngles.x, loadingSpinner.transform.eulerAngles.y, FinalZRot);
            loadingSpinner.SetActive(false);

        }
    }


}
