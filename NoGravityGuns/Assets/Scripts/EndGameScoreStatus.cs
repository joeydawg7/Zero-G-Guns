﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EndGameScoreStatus : MonoBehaviour
{
    public TextMeshProUGUI textName;
    public TextMeshProUGUI score;


    public void SetNameAndScore(string n, string s)
    {
        textName.text = n;
        score.text = s;
    }
}
