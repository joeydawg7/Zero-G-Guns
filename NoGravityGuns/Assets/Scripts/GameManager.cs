using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    public static GameManager Instance { get { return _instance; } }

    public bool isGameStarted;

    public List<PlayerScript> players;

    public CameraController cameraController;

    public JoiningPlayerScript joiningPlayerScript;

    public GunSO pistol;

    public float matchTime;

    public GUIManager guiManager;

    public float timer;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

        isGameStarted = false;

        
    }

    void OnEnable()
    {

        isGameStarted = false;

    }

    public void StartGame()
    {
        Debug.Log("ytea");

        for (int i = 0; i < players.Count; i++)
        {
            players[i].OnGameStart();
        }

        //camera does its shit after
        cameraController.OnGameStart();

        joiningPlayerScript.OnGameStart();

        guiManager.OnGameStart();

        isGameStarted = true;
        timer = matchTime;
    }

    private void Update()
    {
        timer -= Time.deltaTime;

        if(timer<=0 && isGameStarted)
        {
            isGameStarted = false;
            OnGameEnd();
        }
    }


    void OnGameEnd()
    {
       List<PlayerScript> winner = DetermineWinner();

        if(winner.Count > 1)
        {
            Debug.Log("Winners are");

            for (int i = 0; i < winner.Count; i++)
            {
                Debug.Log("Player " + winner[i].playerID);
            }

        }
        //one winner
        else
        {
            Debug.Log("Winner is");

            for (int i = 0; i < winner.Count; i++)
            {
                Debug.Log("Player " + winner[i].playerID);
            }
        }
    }

    List<PlayerScript> DetermineWinner()
    {
        int highestkills = 0;
        List<PlayerScript> highestKillPlayer = new List<PlayerScript>();

        for (int i = 0; i < players.Count; i++)
        {
            if(players[i].numKills == highestkills)
            {
                highestKillPlayer.Add(players[i]);
                highestkills = players[i].numKills;
            }
            else if (players[i].numKills > highestkills)
            {
                highestKillPlayer.Clear();
                highestKillPlayer.Add(players[i]);
                highestkills = players[i].numKills;
            }


        }

        return highestKillPlayer;
    }


}
