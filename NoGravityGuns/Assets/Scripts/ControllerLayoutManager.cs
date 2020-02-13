using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public static class ControllerLayoutManager 
{
    /// <summary>
    /// Changes controller layout to use gameplay settings
    /// </summary>
    public static void SwapToGameplayMaps()
    {
        for (int i = 0; i < ReInput.players.AllPlayers.Count; i++)
        {
            foreach (Joystick joystick in ReInput.players.AllPlayers[i].controllers.Joysticks)
            {
                //ReInput.players.AllPlayers[i].controllers.maps.SetAllMapsEnabled(true);
                ReInput.players.AllPlayers[i].controllers.maps.SetMapsEnabled(false, "UI");
                ReInput.players.AllPlayers[i].controllers.maps.SetMapsEnabled(true, "Gameplay");
                ReInput.players.AllPlayers[i].controllers.maps.SetMapsEnabled(true, "Default");
            }
        }
    }

    /// <summary>
    /// Changes controller layout to use UI settings
    /// </summary>
    public static void SwapToUIMaps()
    {
        for (int i = 0; i < ReInput.players.AllPlayers.Count; i++)
        {
            foreach (Joystick joystick in ReInput.players.AllPlayers[i].controllers.Joysticks)
            {
                //ReInput.players.AllPlayers[i].controllers.maps.SetAllMapsEnabled(true);
                ReInput.players.AllPlayers[i].controllers.maps.SetMapsEnabled(true, "UI");
                ReInput.players.AllPlayers[i].controllers.maps.SetMapsEnabled(false, "Gameplay");
                ReInput.players.AllPlayers[i].controllers.maps.SetMapsEnabled(false, "Default");
            }
        }
    }
}
