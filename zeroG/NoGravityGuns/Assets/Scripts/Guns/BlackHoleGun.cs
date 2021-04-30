using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHoleGun : Guns
{
    public string name;
    public float knockBackModifier;
    //public int minDamageRange;
    //public int maxDamageRange;
    //public float bulletSpeed;

    public float delayBeforeShot;

    public override void Fire(PlayerScript player)
    {
        if (CheckIfAbleToiFire(this))
        {
            //player.armsScript.audioSource.PlayOneShot(GetRandomGunshotSFX);
            player.StartCoroutine(DelayShotCoroutine(player, delayBeforeShot, bulletSpeed, minDamageRange, maxDamageRange, this));
            ReduceBullets(player);
        }
        else
        {
            CheckForGunTimeout(player);
        }        
    }
}
