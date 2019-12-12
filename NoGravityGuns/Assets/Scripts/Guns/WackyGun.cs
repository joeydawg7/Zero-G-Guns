using System.Collections;
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

    private void Start()
    {
       
    }

    public override void Fire(PlayerScript player)
    {
        if (CheckIfAbleToiFire(this))
        {            
            //player.armsScript.audioSource.PlayOneShot(GetRandomGunshotSFX);
            player.StartCoroutine(ShootThisGun(player));
            ReduceBullets(player);
        }
        else
        {
            CheckForAmmo(player);
        }       
    }

    public IEnumerator ShootThisGun(PlayerScript player)
    {
        player.armsScript.bulletSpawn = bulletSpawns[Random.Range(0, bulletSpawns.Count)];
        player.StartCoroutine(DelayShotCoroutine(player, delayBeforeShot, bulletSpeed, minDamageRange, maxDamageRange));
        ReduceBullets(player);
        yield return new WaitForSeconds(delayBeforeShot);
        player.armsScript.bulletSpawn = bulletSpawns[Random.Range(0, bulletSpawns.Count)];
        player.StartCoroutine(DelayShotCoroutine(player, delayBeforeShot, bulletSpeed, minDamageRange, maxDamageRange));
        ReduceBullets(player);
        yield return new WaitForSeconds(delayBeforeShot);
        player.armsScript.bulletSpawn = bulletSpawns[Random.Range(0, bulletSpawns.Count)];
        player.StartCoroutine(DelayShotCoroutine(player, delayBeforeShot, bulletSpeed, minDamageRange, maxDamageRange));
        ReduceBullets(player);
    }
}
