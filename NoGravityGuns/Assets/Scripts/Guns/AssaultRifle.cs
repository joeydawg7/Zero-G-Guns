using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssaultRifle : Guns
{
    public string name;
    public float knockBackModifier;
    //public int minDamageRange;
    //public int maxDamageRange;
    //public float bulletSpeed;
    
    public float delayBeforeShot;
    
   

    public int numBurstShots;
    public float timeBetweenBurstShots;

    public override void Fire(PlayerScript player)
    {
        if (CheckIfAbleToiFire(this))
        {
            player.StartCoroutine(FireInBurst(player, numBurstShots));
        }
        else
        {
            CheckForGunTimeout(player);
        }       
    }

    IEnumerator FireInBurst(PlayerScript player, int bulletsToShoot)
    {
        ArmsScript arms = player.armsScript;

        for (int i = 0; i < bulletsToShoot; i++)
        {
            player.armsScript.audioSource.PlayOneShot(GetRandomGunshotSFX);
            player.StartCoroutine(DelayShotCoroutine(player, delayBeforeShot, /*bulletSpeed,*/ minDamageRange, maxDamageRange, this));
            ReduceBullets(player);
            yield return new WaitForSeconds(timeBetweenBurstShots);
        }
    }

}
