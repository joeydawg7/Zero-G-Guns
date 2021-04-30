using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class GUIManager : MonoBehaviour
{

    public TextMeshProUGUI timerText;
    GameManager gameManager;

    public Sprite redHeadSprite;
    public Sprite blueHeadSprite;
    public Sprite greenHeadSprite;
    public Sprite yellowHeadSprite;

    bool isTimerRunning;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.Instance;

    }

    // Update is called once per frame
    void Update()
    {
        //if(GameManager.Instance.isGameStarted && isTimerRunning)
        //{
        //    timerText.alpha = 1;
        //    gameManager.timer -= Time.deltaTime;
        //    string minutes = Mathf.Floor(gameManager.timer / 60f).ToString("00");
        //    string seconds = Mathf.Floor(gameManager.timer % 60f).ToString("00");

        //    timerText.text = string.Format("{0}:{1}", minutes, seconds); 
        //}
        //else
        //{
        //    timerText.alpha = 0;
        //}
    }
    
    public void RunTimer(bool isTimerRunning)
    {
        this.isTimerRunning = isTimerRunning;
    }

    public void OnGameStart()
    {
       // timerText.alpha = 1;
    }

}
