﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WackyGun : Guns
{
    public string name;
    public float knockBackModifier;
    //public int minDamageRange;
    //public int maxDamageRange;
    //public float bulletSpeed;

    public float delayBeforeShot;
    public List<Transform> bulletSpawns;
    private List<int> spawnIds = new List<int> { 0, 1, 2 };

    bool shooting;

    private void Start()
    {
        shooting = false;
    }

    public override void Fire(PlayerScript player)
    {
        if (CheckIfAbleToiFire(this) && !shooting)
        {            
            //player.armsScript.audioSource.PlayOneShot(GetRandomGunshotSFX);
            player.StartCoroutine(ShootThisGun(player));
            ReduceBullets(player);
        }
        else
        {
            CheckForGunTimeout(player);
        }       
    }

    public IEnumerator ShootThisGun(PlayerScript player)
    {
        if(player.armsScript.currentWeapon is WackyGun)
        {
            shooting = true;
            var id = Random.Range(0, spawnIds.Count - 1);
            if (CheckIfAbleToiFire(this))
            {
                player.armsScript.bulletSpawn = bulletSpawns[spawnIds[id]];
                spawnIds.Remove(spawnIds[id]);
                player.StartCoroutine(DelayShotCoroutine(player, delayBeforeShot, bulletSpeed, minDamageRange, maxDamageRange,this));
                
            }
            yield return new WaitForSeconds(delayBeforeShot);
            ReduceBullets(player);
            if (CheckIfAbleToiFire(this))
            {
                id = Random.Range(0, spawnIds.Count - 1);
                player.armsScript.bulletSpawn = bulletSpawns[spawnIds[id]];
                spawnIds.Remove(spawnIds[id]);
                player.StartCoroutine(DelayShotCoroutine(player, delayBeforeShot, bulletSpeed, minDamageRange, maxDamageRange,this));
                
            }
            yield return new WaitForSeconds(delayBeforeShot);
            ReduceBullets(player);
            if (CheckIfAbleToiFire(this))
            {
                id = Random.Range(0, spawnIds.Count - 1);
                player.armsScript.bulletSpawn = bulletSpawns[spawnIds[id]];
                player.StartCoroutine(DelayShotCoroutine(player, delayBeforeShot, bulletSpeed, minDamageRange, maxDamageRange,this));
                
            }            
            yield return new WaitForSeconds(delayBeforeShot);
            ReduceBullets(player);
        }        
        shooting = false;
        spawnIds.Clear();
        spawnIds.AddRange(new List<int> { 0, 1, 2 });
        timeSinceLastShot = 0;
    }
}
