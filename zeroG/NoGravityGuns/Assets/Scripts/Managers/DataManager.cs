using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;

public class DataManager : MonoBehaviour
{
    //public bool AllowWriteToFile;

    //public string path;

    //List<PlayerScript> players;

    //GameManager gameManager;

    //public void OnGameStart()
    //{
    //    gameManager = GameManager.Instance;

    //    if (AllowWriteToFile)
    //    {
    //        path = Application.dataPath + "/Data/" + path + ".txt";

    //        if (!File.Exists(path))
    //        {
    //            File.WriteAllText(path, "Game Stats Log");
    //        }

    //        gameManager = GameManager.Instance;
    //        players = gameManager.players;

    //    }
    //}

    //public void OnGameEnd(List<PlayerScript> winners)
    //{
    //    if (AllowWriteToFile)
    //    {
    //        WriteStatsToFile(winners);

    //    }
    //}


    //void WriteStatsToFile(List<PlayerScript> winners)
    //{
    //    File.AppendAllText(path, "\nNew game registered at " + System.DateTime.Now + "\n------------------\n");

    //    //Write some text to the test.txt file
    //    File.AppendAllText(path, "Pistol Damage: " + gameManager.pistolDamage + ", Pistol Kills: " + gameManager.pistolKills + "\n");
    //    File.AppendAllText(path, "Assault Rifle Damage: " + gameManager.assaultDamage + ", Assault Rifle Kills: " + gameManager.assaultRifleKills + "\n");
    //    File.AppendAllText(path, "Minigun Damage: " + gameManager.minigunDamage + ", Minigun Kills: " + gameManager.minigunKills + "\n");
    //    File.AppendAllText(path, "Shotgun Damage: " + gameManager.shotGunDamage + ", Shotgun Kills: " + gameManager.shotGunKills + "\n");
    //    File.AppendAllText(path, "Railgun Damage: " + gameManager.railgunDamage + ", Railgun Kills: " + gameManager.railgunKills + "\n");
    //    File.AppendAllText(path, "Collision Damage: " + gameManager.collisionDamage + ", Collision Kills: " + gameManager.collisionKills + "\n");
    //    File.AppendAllText(path, "Healed Damage: " + gameManager.healthPackHeals + "\n");

    //    foreach (var player in gameManager.players)
    //    {
    //        File.AppendAllText(path, "\n\nPlayer: " + player.playerName);
    //        File.AppendAllText(path, "\nPistol uptime: " + player.pistolTime);
    //        File.AppendAllText(path, "\nPistol dmg: " + player.pistolDmg);
    //        File.AppendAllText(path, "\nAssault rifle uptime: " + player.rifleTime);
    //        File.AppendAllText(path, "\nAssault rifle dmg: " + player.rifleDmg);
    //        File.AppendAllText(path, "\nShotgun uptime: " + player.shotgunTime);
    //        File.AppendAllText(path, "\nshotGun dmg: " + player.shotgunDmg);
    //        File.AppendAllText(path, "\nMinigun uptime: " + player.miniGunTime);
    //        File.AppendAllText(path, "\nMinigun dmg: " + player.miniGunTime);
    //        File.AppendAllText(path, "\nRailgun uptime: " + player.railgunTime);
    //        File.AppendAllText(path, "\nRailgun dmg: " + player.railgunDmg);
    //        File.AppendAllText(path, "\n------------------\n");

    //        float hitPercent = (player.shotsHit / player.shotsFired) * 100;
    //        float hsPercent = (player.headShots / player.shotsHit) * 100;
    //        float tsPercent = (player.torsoShots / player.shotsHit) * 100;
    //        float lsPercent = (player.legShots / player.shotsHit) * 100;
    //        float fsPercent = (player.footShots / player.shotsHit) * 100;
    //        File.AppendAllText(path, "Fired " + player.shotsFired + " shots (approx), of which " + player.shotsHit + " hit someone. (" + hitPercent + "%)\n");
    //        File.AppendAllText(path, "Headshots: " + player.headShots + "(" + hsPercent + "% of hits)\n");
    //        File.AppendAllText(path, "torsoShots: " + player.torsoShots + "(" + tsPercent + "% of hits)\n");
    //        File.AppendAllText(path, "legShots: " + player.legShots + "(" + lsPercent + "% of hits)\n");
    //        File.AppendAllText(path, "footShots: " + player.footShots + "(" + fsPercent + "% of hits)\n");

    //    }

    //    File.AppendAllText(path, "\n\n");
    //    File.AppendAllText(path, "\nWinner(s)\n------------------\n");

    //    foreach (var winner in winners)
    //    {
    //        File.AppendAllText(path, "Winner: " + winner.playerName + "\n");
    //        File.AppendAllText(path, "They had : " + winner.numKills + " kills\n\n");
    //    }

    //}


}
