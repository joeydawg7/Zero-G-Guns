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
    }

    /// <summary>
    /// Changes controller layout to use UI settings. 
    /// removeFromOtherPlayers: determines if joystick assignment should be remembered after switching controller maps.
    /// </summary>
    public static void SwapToUIMaps(bool removeFromOtherPlayers)
    {
        Debug.Log("swapped to UI maps");

        ControllerLayoutManager.AssignAllJoysticksToSystemPlayer(removeFromOtherPlayers);
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
        foreach (var j in ReInput.controllers.Joysticks)
        {
            ReInput.players.GetSystemPlayer().controllers.RemoveController(j);           
        }

        //why isnt this a dictionary rewired!?!
        //basically 0 = gameplayMaps rule, 1 = UI maps rule
        ReInput.players.GetSystemPlayer().controllers.maps.mapEnabler.ruleSets[0].enabled = true;
        ReInput.players.GetSystemPlayer().controllers.maps.mapEnabler.ruleSets[1].enabled = false;

        ReInput.players.GetSystemPlayer().controllers.maps.mapEnabler.Apply();

            
    }
    
}
