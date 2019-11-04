using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minigun : Guns
{
    public string name;
    public float knockBackModifier;
    //public int minDamageRange;
    //public int maxDamageRange;
    //public float bulletSpeed;
    
    public float delayBeforeShot;
    
    
    public float spinUpTime;
    float spinUpAmount;

    //public override int GunDamage()
    //{
    //    return Random.Range(minDamageRange, maxDamageRange); 
    //}

    public override void Fire(PlayerScript player)
    {
        base.KnockBack(player, player.knockbackMultiplier);
        player.armsScript.audioSource.PlayOneShot(GetRandomGunshotSFX);
        spinUpAmount += recoilDelay;
        player.StartCoroutine(spinUpMinigun(player));
        player.StartCoroutine(DelayShotCoroutine(player, delayBeforeShot, bulletSpeed, minDamageRange, maxDamageRange));
        ReduceBullets(player);
    }

    

    /*
     * hold button down
     * spinup begins
     * after like 25% of spinup time begin shooting slowly
     * increment this rate of fire until spinup time reached
     * maintain fire rate until button released
     * slowly decrease spinup amount until 0
     * 
     * */
    IEnumerator spinUpMinigun(PlayerScript player)
    {
        while (spinUpAmount < (spinUpTime + recoilDelay) && player.player.GetAxis("Shoot") > 0.5f && player.armsScript.currentWeapon.name == "Minigun")
        {
            spinUpAmount += Time.deltaTime;
            if (spinUpAmount >= 0.25f)
            {
                recoilDelay = 1 - spinUpAmount;
            }
            yield return null;
        }

        recoilDelay = 0.08f;

        while (player.player.GetAxis("Shoot") < 0.5f)
        {

        }
        //timer = 0;
    }
}
