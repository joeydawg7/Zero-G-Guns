using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class PlayerDataScript : MonoBehaviour
{
    int kills;
    public int roundWins;
    public PlayerControllerData playerControllerData;
    public string playerName;
    public string hexColorCode;
    public GameObject playerObj;
    GlobalPlayerSettingsSO globalPlayerSettings;

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

    public void SetPlayerInfoAfterRoundStart(PlayerControllerData playerControllerData, GlobalPlayerSettingsSO globalPlayerSettings)
    {
        SetPlayer(playerControllerData, globalPlayerSettings);


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

    void SetPlayer(PlayerControllerData playerControllerData, GlobalPlayerSettingsSO globalPlayerSettings)
    {
        print(playerControllerData.ID);

        this.playerControllerData = playerControllerData;
        this.globalPlayerSettings = globalPlayerSettings;
        //playerScript.hexColorCode = hexColorCode;
        //playerScript.playerName = playerName;

        Debug.Log("player id in set :" + playerControllerData.ID);
        
        //playerScript.OnGameStart();
    }


    public void SpawnAtMatchingPoint()
    {
        Dictionary<int, PlayerSpawnPoint> playerSpawnPoints = globalPlayerSettings.GetAllPlayerSpawnPoints();

        Debug.Log(playerControllerData.ID);

        Debug.Log("KAW KAW");

        if (playerSpawnPoints[playerControllerData.ID + 1] != null)
            playerSpawnPoints[playerControllerData.ID + 1].SetCharacter(playerControllerData.ID, playerControllerData.controller);
        else
            Debug.LogError("ID " + " not found!");
    }
}
