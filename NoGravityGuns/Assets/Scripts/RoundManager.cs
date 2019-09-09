﻿using System.Collections;
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
        }

        finishedControllerSetup = false;

        currentRound = 0;
        maxRounds = 5;
        loadingSpinner.SetActive(false);

    }

    public void NewRound()
    {

        //TODO: get whatever the next room is going to be, get some info about it probably stored through scriptable object
        currentRound++;

        //TODO: only grab from a list of playable rooms so player can check off maps they dont want to play
        RoomSO nextRoom = rooms[Random.Range(0, rooms.Count)];

        //SceneManager.LoadScene(nextRoom.sceneName);
        StartCoroutine(AddLevel(nextRoom.sceneName, nextRoom));

    }

    bool loading = false;

    IEnumerator AddLevel(string lvl, RoomSO nextRoom)
    {
        loadingSpinner.SetActive(true);

        //set rotation to 0, set endpoint to 360 degrees later
        float startRotation = 0f;
        float endRotation = startRotation + 360.0f;

        float t = 0.0f;
        float FinalZRot = 0;

        AsyncOperation asyncLoadLevel = SceneManager.LoadSceneAsync(lvl);

        while (!asyncLoadLevel.isDone)
        {
            t += Time.deltaTime;
            //math magic
            float zRotation = Mathf.Lerp(startRotation, endRotation, t / 1f) % 360.0f;
            loadingSpinner.transform.eulerAngles = new Vector3(loadingSpinner.transform.eulerAngles.x, loadingSpinner.transform.eulerAngles.y, zRotation);
            FinalZRot = zRotation;
            yield return null;
        }

        loadingSpinner.transform.eulerAngles = new Vector3(loadingSpinner.transform.eulerAngles.x, loadingSpinner.transform.eulerAngles.y, FinalZRot);
        loadingSpinner.SetActive(false);

        loading = false;
        LevelLoaded(nextRoom);
    }

    void LevelLoaded(RoomSO nextRoom)
    {

        newRoundText = GameObject.Find("RoomNamePopup").GetComponent<TextMeshProUGUI>();
        newRoundTextAnimator = newRoundText.GetComponent<Animator>();
        newRoundText.alpha = 1;
        newRoundText.text = nextRoom.roomName;
        newRoundTextAnimator.SetTrigger("NewRound");
        roundNumText = newRoundText.transform.Find("RoundNum").GetComponent<TextMeshProUGUI>();
        roundNumText.text = "Round " + currentRound + " of " + maxRounds;

        //SetAllPlayersDataIntoPlayerObjects();

        GameManager.Instance.StartGame();
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

            Debug.Log("set player called");
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
        if (Input.GetKeyDown(KeyCode.F5) && loading == false)
        {
            loading = true;
            NewRound();
        }
    }


}
