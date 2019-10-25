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
    public bool isCurrentWinner;
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
        this.playerControllerData = playerControllerData;
        this.globalPlayerSettings = globalPlayerSettings;
        isCurrentWinner = false;

        Debug.Log("player id in set :" + playerControllerData.ID + " controller: " + playerControllerData.controller.name);
    }



    public void SpawnAtMatchingPoint(GlobalPlayerSettingsSO globalPlayerSettings, GameObject playerCanvas)
    {
        Dictionary<int, PlayerSpawnPoint> playerSpawnPoints = globalPlayerSettings.GetAllPlayerSpawnPoints();

        playerName = globalPlayerSettings.playerSettings[playerControllerData.ID].playerName;
        hexColorCode = globalPlayerSettings.playerSettings[playerControllerData.ID].playerColorHexCode;

        PlayerCanvasScript pcs = playerCanvas.GetComponent<PlayerCanvasScript>();


        if (playerSpawnPoints[playerControllerData.ID] != null)
            playerSpawnPoints[playerControllerData.ID].SpawnCharacter(playerControllerData.ID, playerControllerData.controller, globalPlayerSettings, playerCanvas, isCurrentWinner);
        else
            Debug.LogError("ID " + " not found!");

    }
}
