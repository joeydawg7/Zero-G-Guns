﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class GUIManager : MonoBehaviour
{

    public TextMeshProUGUI timerText;
    


    // Start is called before the first frame update
    void Start()
    {
        timerText.alpha = 0;
        timerText.text = GameManager.Instance.timer.ToString("mmss");
    }

    // Update is called once per frame
    void Update()
    {
        if(GameManager.Instance.isGameStarted)
        {
            GameManager.Instance.timer -= Time.deltaTime;
            string minutes = Mathf.Floor(GameManager.Instance.timer / 60).ToString("00");
            string seconds = Mathf.Floor(GameManager.Instance.timer % 60).ToString("00");

            timerText.text = string.Format("{0}:{1}", minutes, seconds); 
        }
        else
        {
            timerText.alpha = 0;
        }
    }

    public void OnGameStart()
    {
        timerText.alpha = 1;
    }

}
