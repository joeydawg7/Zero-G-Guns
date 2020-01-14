using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : Guns
{
    public string name;
    public float knockBackModifier;
   
   
    public float delayBeforeShot;  
    
    
    public override void Fire(PlayerScript player)
    {
        if (CheckIfAbleToiFire(this))
        {
            ArmsScript arms = player.armsScript;

            if (arms.audioSource.isPlaying)
                arms.audioSource.Stop();

            //arms.audioSource.PlayOneShot(GetRandomGunshotSFX);
            player.StartCoroutine(DelayShotCoroutine(player, delayBeforeShot, bulletSpeed, minDamageRange, maxDamageRange,this));
            ReduceBullets(player);
        }
        else
        {
            CheckForGunTimeout(player);
        }        
    }
}
