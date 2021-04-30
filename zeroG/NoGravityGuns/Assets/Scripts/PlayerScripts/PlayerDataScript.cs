using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class PlayerDataScript : MonoBehaviour
{
    int kills;
    public int roundWins;
    public PlayerControllerData playerControllerData;
    public int playerID;
    public string playerName;
    public string hexColorCode;
    public GameObject playerObj;
    public bool isCurrentWinner;
    public GlobalPlayerSettingsSO globalPlayerSettings;
    public GameObject playerCanvas;

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

    public void SetPlayerInfoAfterRoundStart(PlayerControllerData playerControllerData, GlobalPlayerSettingsSO globalPlayerSettings,
        GameObject playerCanvas)
    {
        this.playerControllerData = playerControllerData;
        this.globalPlayerSettings = globalPlayerSettings;
        isCurrentWinner = false;

        //set the player canvas so we can give it to the player 
        this.playerCanvas = playerCanvas;

        //sets player number and colour based on what we set as the global player settings. this is done so player color / names are
        //consistent and only have to be set one time by a human
        playerName = globalPlayerSettings.playerSettings[playerControllerData.ID].playerName;
        hexColorCode = globalPlayerSettings.playerSettings[playerControllerData.ID].playerColorHexCode;
        playerID = playerControllerData.ID;

        //Debug.Log("player id set for <color=" + hexColorCode + ">"+ playerName + "</color> with controller: " + playerControllerData.controller.name);
    }


    //// spawns corresponding controller ID with player Spawn points, sets canvas settings in the process.
    public void SpawnAtMatchingPoint()
    {
        Debug.Log("Creating Dictionary of Spawn points");
        Dictionary<int, PlayerSpawnPoint> playerSpawnPoints = globalPlayerSettings.GetAllPlayerSpawnPoints();

        Debug.Log("Number of Spawn Point in Dictionary " + playerSpawnPoints.Count);

        foreach (var sp in playerSpawnPoints)
        {
            Debug.Log(sp.Key);
            Debug.Log(sp.Value.characterToSpawn[0].name);
        }

        //assuming we have found an ID that correlates to a spawn point, spawn the player in now
        if (playerSpawnPoints[playerControllerData.ID] != null)
            // playerSpawnPoints[playerControllerData.ID].SpawnCharacter(playerControllerData.ID, playerControllerData.controller,
            //  globalPlayerSettings, playerCanvas, isCurrentWinner);
            playerSpawnPoints[playerControllerData.ID].SpawningCharacter(this);
        else
            Debug.LogError("ID " + " not found!");
    }

    public void SpawnAtMatchingPoint(PlayerControllerData thisPlayerControllerData)
    {
        Debug.Log("Creating Dictionary of Spawn points");
        Dictionary<int, PlayerSpawnPoint> playerSpawnPoints = globalPlayerSettings.GetAllPlayerSpawnPoints();

        Debug.Log("Number of Spawn Point in Dictionary " + playerSpawnPoints.Count);

        foreach (var sp in playerSpawnPoints)
        {
            Debug.Log(sp.Key);
            Debug.Log(sp.Value.characterToSpawn[0].name);
        }

        try
        {
            if (playerSpawnPoints[thisPlayerControllerData.ID] != null)
                //playerSpawnPoints[thisPlayerControllerData.ID].SpawnCharacter(thisPlayerControllerData.ID, thisPlayerControllerData.controller,
                //   globalPlayerSettings, playerCanvas, isCurrentWinner);
                playerSpawnPoints[playerControllerData.ID].SpawningCharacter(this);
        }
        //assuming we have found an ID that correlates to a spawn point, spawn the player in now        
        catch
        {
            Debug.LogError("ID  = " + thisPlayerControllerData.ID + " not found!");
        }
    }
}
