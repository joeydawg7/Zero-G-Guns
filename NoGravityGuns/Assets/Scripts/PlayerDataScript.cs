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
        this.playerControllerData = playerControllerData;
        this.globalPlayerSettings = globalPlayerSettings;

        Debug.Log("player id in set :" + playerControllerData.ID + " controller: " + playerControllerData.controller.name);
    }

  

    public void SpawnAtMatchingPoint(GlobalPlayerSettingsSO globalPlayerSettings, GameObject playerCanvas)
    {
        Dictionary<int, PlayerSpawnPoint> playerSpawnPoints = globalPlayerSettings.GetAllPlayerSpawnPoints();

        Debug.Log(playerControllerData.ID);
        print(playerControllerData.controller.name);

        if (playerSpawnPoints[playerControllerData.ID] != null)
            playerSpawnPoints[playerControllerData.ID].SpawnCharacter(playerControllerData.ID, playerControllerData.controller, globalPlayerSettings, playerCanvas);
        else
            Debug.LogError("ID " + " not found!");

    }
}
