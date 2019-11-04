using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistol : Guns
{
    public string name;
    public float knockBackModifier;
    //public int minDamageRange;
    //public int maxDamageRange;
    //public float bulletSpeed;
    
    public float delayBeforeShot;

    public override void Fire(PlayerScript player)
    {
        base.KnockBack(player, player.knockbackMultiplier);
        player.armsScript.audioSource.PlayOneShot(GetRandomGunshotSFX);
        player.StartCoroutine(DelayShotCoroutine(player, delayBeforeShot, bulletSpeed, minDamageRange, maxDamageRange));
        ReduceBullets(player);
    }       
}
