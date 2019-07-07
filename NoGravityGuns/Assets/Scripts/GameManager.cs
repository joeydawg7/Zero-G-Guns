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

        isGameStarted = true;
    }

}
