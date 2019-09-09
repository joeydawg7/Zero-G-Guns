﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Rewired;

public class EndGameScript : MonoBehaviour
{

    public TextMeshProUGUI gameOverText;
    public TextMeshProUGUI winOrTie;
    public TextMeshProUGUI Winners;

    public string mainSceneName;
    public string mainMenuScene;

    bool tickTimer = true;

    // Update is called once per frame
    void Update()
    {

        if(tickTimer)
            timer += Time.deltaTime;

        foreach (var player in ReInput.players.AllPlayers)
        {
            if (player.GetButtonDown("Join") || timer>=5f)
            {
                tickTimer = false;
                timer = 0;
                //SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
                RoundManager.Instance.NewRound();
            }

            if (player.GetButtonDown("Drop"))
            {
                SceneManager.LoadScene(mainMenuScene, LoadSceneMode.Single);
            }
        }

    }

    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void StartEndGame(List<PlayerScript> winners)
    {
        gameOverText.alpha = 0;
        winOrTie.alpha = 0;
        winOrTie.text = "";
        Winners.alpha = 0;
        Winners.text = "";
        gameObject.SetActive(true);

        foreach (var player in ReInput.players.AllPlayers)
        {
            player.controllers.maps.SetMapsEnabled(true, "UI");
        }


        StartCoroutine(EndGame(winners));
    }

    float timer;

    //marvel dont sue
    IEnumerator EndGame(List<PlayerScript> winnersList)
    {
        yield return new WaitForSeconds(0.2f);
        gameOverText.text = "Round Over!";
        gameOverText.alpha = 1;
        yield return new WaitForSeconds(0.5f);

        winOrTie.alpha = 1;
        Winners.alpha = 1;

        //if more than one person left alive we need to check kills
        if (winnersList.Count > 1)
        {
            List<PlayerScript> mostKillsPlayers = new List<PlayerScript>();

            //winOrTie.text = "Its a tie!";            
            for (int i = 0; i < winnersList.Count - 1; i++)
            {
                for (int j = i + 1; j < winnersList.Count; j++)
                {
                    //if they are not equal we should do more stuff
                    if (winnersList[i].numKills != winnersList[j].numKills)
                    {
                        //we found a new highest, kill the old stuff make this the only entry in the list
                        if (winnersList[i].numKills > winnersList[j].numKills)
                        {
                            mostKillsPlayers.Clear();
                            mostKillsPlayers.Add(winnersList[i]);
                        }
                    }
                    //otherwise they are equal so we might have a tie in kills
                    else
                    {
                        if (!mostKillsPlayers.Contains(winnersList[i]))
                            mostKillsPlayers.Add(winnersList[i]);
                        if (!mostKillsPlayers.Contains(winnersList[j]))
                            mostKillsPlayers.Add(winnersList[j]);
                    }
                }
            }

            //tie in kills, time to nitty gritty with health
            if (mostKillsPlayers.Count > 1)
            {
                for (int i = 0; i < winnersList.Count - 1; i++)
                {
                    for (int j = i + 1; j < winnersList.Count; j++)
                    {
                        //if they are not equal we should do more stuff
                        if (winnersList[i].health != winnersList[j].health)
                        {
                            //we found a new highest, kill the old stuff make this the only entry in the list
                            if (winnersList[i].health > winnersList[j].health)
                            {
                                mostKillsPlayers.Clear();
                                mostKillsPlayers.Add(winnersList[i]);
                            }
                        }
                        //otherwise they are equal so we might have a tie in health
                        else
                        {
                            if (!mostKillsPlayers.Contains(winnersList[i]))
                                mostKillsPlayers.Add(winnersList[i]);
                            if (!mostKillsPlayers.Contains(winnersList[j]))
                                mostKillsPlayers.Add(winnersList[j]);
                        }
                    }
                }
            }

            foreach (var realWinner in mostKillsPlayers)
            {
                if (mostKillsPlayers.Count <= 1)
                {
                    winOrTie.text = "<" + realWinner.hexColorCode + ">" + realWinner.playerName + " Is the winner!" + "</color>";
                    yield return new WaitForSeconds(0.5f);
                }
                else
                {
                    winOrTie.text = "<" + realWinner.hexColorCode + ">" + realWinner.playerName + " Are the winners! It's a tie!" + "</color>";
                    yield return new WaitForSeconds(0.35f);
                }
                
            }
         
            foreach (var player in GameManager.Instance.players)
            {
                yield return new WaitForSeconds(0.5f);
                Winners.text += "<" + player.hexColorCode + ">" + player.playerName + ": " + player.numKills + "</color> \n";

            }
        }
        //one winner
        else
        {

            PlayerDataScript roundWinner = AddRoundWinToWinner(winnersList[0]);

            //RoundManager roundManager = RoundManager.Instance;

            //if (roundManager.playerDataList.Count > 0)
            //{
            //    foreach (PlayerDataScript dataSet in roundManager.playerDataList)
            //    {
            //        if (dataSet.playerID == winnersList[0].playerID)
            //        {
            //            dataSet.IncreaseRoundWins();
            //        }
            //    }
            //}

            if(roundWinner == null)
            {
                Debug.LogError("No dataSet found for current round winner... this should never happen!");
                yield break;
            }

            winOrTie.text = "<" + winnersList[0].hexColorCode + ">" + winnersList[0].playerName + " Is the winner!" + "</color>";
            yield return new WaitForSeconds(0.5f);

            foreach (var winner in RoundManager.Instance.playerDataList)
            {
                yield return new WaitForSeconds(0.5f);
                Winners.text += "<" + winner.hexColorCode + ">" + winner.playerName + ": " + winner.roundWins + " Rounds Won " + "</color> \n";

            }


        }

    }


    PlayerDataScript AddRoundWinToWinner(PlayerScript winner)
    {
        RoundManager roundManager = RoundManager.Instance;

        if (roundManager.playerDataList.Count > 0)
        {
            foreach (PlayerDataScript dataSet in roundManager.playerDataList)
            {
                if (dataSet.playerID == winner.playerID)
                {
                    winner.roundWins++;
                    dataSet.IncreaseRoundWins();
                    return dataSet;
                }
            }
        }

        return null;
    }

}
