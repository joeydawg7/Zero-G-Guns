using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GlobalPlayerSettings", menuName = "ScriptableObjects/Managers/GlobalPlayerSettings", order = 2)]
public class GlobalPlayerSettingsSO : ScriptableObject
{
    [System.Serializable]
    public class PlayerSettings
    {
        public int ID;
        public string playerName;
        public Color32 Color;
        public string playerColorHexCode;
        public Color32 DeadColor;
        public int CollisionLayer;
        public PlayerCanvasSettings PlayerCanvasSettings;
    }

    [System.Serializable]
    public class PlayerCanvasSettings
    {
        public Sprite hpFront;
        public Sprite hpBack;
        public Sprite hpCriticalFlash;
    }


    public PlayerSettings[] playerSettings = new PlayerSettings[4];

    public void SortPlayerSettings()
    {
        //sort all playersettings to align with ID
        for (int j = 0; j <= playerSettings.Length - 2; j++)
        {
            for (int i = 0; i <= playerSettings.Length - 2; i++)
            {
                if (playerSettings[i].ID > playerSettings[i + 1].ID)
                {
                    PlayerSettings temp = playerSettings[i + 1];
                    playerSettings[i + 1] = playerSettings[i];
                    playerSettings[i] = temp;
                }
            }
        }

        //foreach (var item in playerSettings)
        //{
            //Debug.Log(item.ID);
        //}
    }

    public Dictionary<int, PlayerSpawnPoint> GetAllPlayerSpawnPoints()
    {
        PlayerSpawnPoint[] PSP = FindObjectsOfType<PlayerSpawnPoint>();

        Dictionary<int, PlayerSpawnPoint> temp = new Dictionary<int, PlayerSpawnPoint>();

        for (int i = 0; i < PSP.Length; i++)
        {

            temp.Add(PSP[i].IDToSpawn, PSP[i]);
        }


        return temp;
    }
}

