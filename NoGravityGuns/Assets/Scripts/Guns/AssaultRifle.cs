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
        base.KnockBack(player, player.knockbackMultiplier);
        player.StartCoroutine(FireInBurst(player, numBurstShots));
    }

    IEnumerator FireInBurst(PlayerScript player, int bulletsToShoot)
    {
        ArmsScript arms = player.armsScript;

        for (int i = 0; i < bulletsToShoot; i++)
        {
            player.armsScript.audioSource.PlayOneShot(GetRandomGunshotSFX);
            player.StartCoroutine(DelayShotCoroutine(player, delayBeforeShot, bulletSpeed, minDamageRange, maxDamageRange));
            ReduceBullets(player);
            yield return new WaitForSeconds(timeBetweenBurstShots);
        }
    }

}
