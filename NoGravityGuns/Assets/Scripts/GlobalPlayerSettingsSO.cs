using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GlobalPlayerSettings", menuName = "ScriptableObjects/Managers/GlobalPlayerSettings", order = 2)]
public class GlobalPlayerSettingsSO : ScriptableObject
{
    public Color32 player1Color;
    public Color32 player2Color;
    public Color32 player3Color;
    public Color32 player4Color;

    public Color32 player1DeadColor;
    public Color32 player2DeadColor;
    public Color32 player3DeadColor;
    public Color32 player4DeadColor;

    public int player1CollisionLayer;
    public int player2CollisionLayer;
    public int player3CollisionLayer;
    public int player4CollisionLayer;


    

    public Dictionary<int, PlayerSpawnPoint> GetAllPlayerSpawnPoints()
    {
        PlayerSpawnPoint[] PSP = FindObjectsOfType<PlayerSpawnPoint>();

        Dictionary<int, PlayerSpawnPoint> temp = new Dictionary<int, PlayerSpawnPoint>();

        for (int i = 0; i < PSP.Length; i++)
        {
            if(PSP[i].IDToSpawn !=0)
                temp.Add(PSP[i].IDToSpawn, PSP[i]);
        }


        return temp;
    }
}
