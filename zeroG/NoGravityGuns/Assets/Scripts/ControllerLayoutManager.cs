using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
using System.Linq;

public static class ControllerLayoutManager 
{

    /// <summary>
    /// Changes controller layout to use gameplay settings
    /// </summary>
    public static void SwapToGameplayMaps()
    {
        Debug.Log("swapped to Gameplay maps");

        ControllerLayoutManager.RemoveAllJoysticksFromSystemPlayer();
        ControllerLayoutManager.SetAllPlayersToGameplayMaps();
    }

    /// <summary>
    /// Changes controller layout to use UI settings. 
    /// removeFromOtherPlayers: determines if joystick assignment should be remembered after switching controller maps.
    /// </summary>
    public static void SwapToUIMaps(bool removeFromOtherPlayers)
    {
        Debug.Log("swapped to UI maps");

        ControllerLayoutManager.AssignAllJoysticksToSystemPlayer(removeFromOtherPlayers);
        ControllerLayoutManager.SetAllPlayersToUIMaps();
    }


    static void AssignAllJoysticksToSystemPlayer(bool removeFromOtherPlayers)
    {
        foreach (var j in ReInput.controllers.Joysticks)
        {
            ReInput.players.GetSystemPlayer().controllers.AddController(j, removeFromOtherPlayers);   
        }

        //why isnt this a dictionary rewired!?!
        //basically 0 = gameplayMaps rule, 1 = UI maps rule
        ReInput.players.GetSystemPlayer().controllers.maps.mapEnabler.ruleSets[0].enabled = false;
        ReInput.players.GetSystemPlayer().controllers.maps.mapEnabler.ruleSets[1].enabled = true;

        ReInput.players.GetSystemPlayer().controllers.maps.mapEnabler.Apply();
    }

    static void RemoveAllJoysticksFromSystemPlayer()
    {

        //why isnt this a dictionary rewired!?!
        //basically 0 = gameplayMaps rule, 1 = UI maps rule      
        ReInput.players.GetSystemPlayer().controllers.maps.mapEnabler.ruleSets[0].enabled = true;
        ReInput.players.GetSystemPlayer().controllers.maps.mapEnabler.ruleSets[1].enabled = false;


        foreach (var j in ReInput.controllers.Joysticks)
        {
            ReInput.players.GetSystemPlayer().controllers.RemoveController(j);           
        }

        ReInput.players.GetSystemPlayer().controllers.maps.mapEnabler.Apply();            
    }

    static void SetAllPlayersToGameplayMaps()
    {
        foreach (var player in ReInput.players.AllPlayers)
        {
            player.controllers.maps.mapEnabler.ruleSets[0].enabled = true;
            player.controllers.maps.mapEnabler.ruleSets[1].enabled = false;
        }

        ReInput.players.GetSystemPlayer().controllers.maps.mapEnabler.Apply();
    }

    public static void SetPlayerToGamePlayMaps(Player player)
    {
        player.controllers.maps.mapEnabler.ruleSets[0].enabled = true;
        player.controllers.maps.mapEnabler.ruleSets[1].enabled = false;

        player.controllers.maps.mapEnabler.Apply();
    }

    static void SetAllPlayersToUIMaps()
    {
        foreach (var player in ReInput.players.AllPlayers)
        {
            ReInput.players.GetSystemPlayer().controllers.maps.mapEnabler.ruleSets[0].enabled = false;
            ReInput.players.GetSystemPlayer().controllers.maps.mapEnabler.ruleSets[1].enabled = true;
        }

        ReInput.players.GetSystemPlayer().controllers.maps.mapEnabler.Apply();
    }
    
}
