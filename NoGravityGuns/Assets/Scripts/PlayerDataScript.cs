using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class PlayerDataScript : MonoBehaviour
{
    int kills;
    public int roundWins;
    public int playerID;
    public Player player;
    public Controller controller;
    public string playerName;
    public string hexColorCode;

    public void IncreaseRoundWins()
    {
        roundWins++;
    }

    public void IncreaseKills()
    {
        kills++;
    }

    public void DecreaseKills()
    {
        kills--;
    }

    public void SetPlayerInfoAfterRoundStart(PlayerScript player)
    {
        SetPlayer(player);


        //switch (playerID)
        //{
        //    case 1:
        //        SetPlayer(GameManager.Instance.player1);
        //        break;
        //    case 2:
        //        SetPlayer(GameManager.Instance.player2);
        //        break;
        //    case 3:
        //        SetPlayer(GameManager.Instance.player3);
        //        break;
        //    case 4:
        //        SetPlayer(GameManager.Instance.player4);
        //        break;

        //    default:
        //        Debug.LogError("No ID set on initialilzation! This should never happen!");
        //        break;
        //}
    }

    void SetPlayer(PlayerScript playerScript)
    {
        playerScript.controller = controller;
        playerScript.player = player;
        playerScript.playerID  = playerID;
        playerScript.hexColorCode = hexColorCode;
        playerScript.playerName = playerName;

        //Debug.Log("player id in set :" + playerID);
        
        //playerScript.OnGameStart();
    }
}
