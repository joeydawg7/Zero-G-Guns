using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;

public class DataManager : MonoBehaviour
{
    public bool AllowWriteToFile;

    public string path;

    List<PlayerScript> players;

    GameManager gameManager;

    public void OnGameStart()
    {
        gameManager = GameManager.Instance;

        if(AllowWriteToFile)
        {
            path = Application.dataPath + "/Data/" + path + ".txt";

            if(!File.Exists(path))
            {
                File.WriteAllText(path, "Game Stats Log");
            }

            gameManager = GameManager.Instance;
            players = gameManager.players;

        }
    }

    public void OnGameEnd(List<PlayerScript> winners)
    {
        if(AllowWriteToFile)
        {
            WriteStatsToFile(winners);

        }
    }


    void WriteStatsToFile(List<PlayerScript> winners)
    {
        File.AppendAllText(path, "\nNew game registered at " + System.DateTime.Now + "\n------------------\n");

        //Write some text to the test.txt file
        File.AppendAllText(path, "Pistol Damage: " + gameManager.pistolDamage + ", Pistol Kills: " + gameManager.pistolKills + "\n");
        File.AppendAllText(path, "Assault Rifle Damage: " + gameManager.assaultDamage + ", Assault Rifle Kills: " + gameManager.assaultRifleKills + "\n");
        File.AppendAllText(path, "Shotgun Damage: " + gameManager.shotGunDamage + ", Shotgun Kills: " + gameManager.shotGunKills + "\n");
        File.AppendAllText(path, "Railgun Damage: " + gameManager.railgunDamage + ", Railgun Kills: " + gameManager.railgunKills + "\n");
        File.AppendAllText(path, "Collision Damage: " + gameManager.collisionDamage + ", Collision Kills: " + gameManager.collisionKills + "\n");
        File.AppendAllText(path, "Healed Damage: " + gameManager.healthPackHeals + "\n");

        foreach (var player in gameManager.players)
        {
            File.AppendAllText(path, "\nPlayer: " + player.playerName );
            File.AppendAllText(path, "\nPistol uptime: " + player.pistolTime);
            File.AppendAllText(path, "\nAssault rifle uptime: " + player.rifleTime);
            File.AppendAllText(path, "\nShotgun uptime: " + player.shotgunTime);
            File.AppendAllText(path, "\nMinigun uptime: " + player.miniGunTime);
            File.AppendAllText(path, "\nRailgun uptime: " + player.railgunTime);
        }

        File.AppendAllText(path, "\n\n");

        foreach (var winner in winners)
        {
            File.AppendAllText(path, "Winner: " + winner.playerName + "\n");
            File.AppendAllText(path, "They had : " + winner.numKills + " kills\n");
        }

    }


}
