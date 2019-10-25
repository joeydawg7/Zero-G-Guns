using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Gun", menuName = "ScriptableObjects/Guns/Gun_Minigun", order = 1)]
public class GunSO_minigun : GunSO
{
    public float spinUpTime;
    float spinUpAmount;
    //public override void Fire(PlayerScript player, Vector3 dir)
    //{
    //    player.armsScript.audioSource.PlayOneShot(GetRandomGunshotSFX);

    //    spinUpAmount += recoilDelay;

    //    player.StartCoroutine(spinUpMinigun(player));

    //    player.StartCoroutine(DelayShotCoroutine(player, dir));

    //    ReduceBullets(player);
    //}

    /*
     * hold button down
     * spinup begins
     * after like 25% of spinup time begin shooting slowly
     * increment this rate of fire until spinup time reached
     * maintain fire rate until button released
     * slowly decrease spinup amount until 0
     * 
     * */
    //IEnumerator spinUpMinigun(PlayerScript player)
    //{
    //    while (spinUpAmount < (spinUpTime+recoilDelay) && player.player.GetAxis("Shoot") > 0.5f && player.armsScript.currentWeapon.name == "Minigun")
    //    {
    //        spinUpAmount += Time.deltaTime;
    //        if (spinUpAmount >= 0.25f)
    //        {
    //            recoilDelay = 1 - spinUpAmount;
    //        }          
    //        yield return null;
    //    }

    //    recoilDelay = 0.08f;

    //    while (player.player.GetAxis("Shoot") < 0.5f)
    //    {

    //    }


    //    //timer = 0;
    //}


    //public override void KnockBack(PlayerScript player, Vector2 shootDir, float knockbackMultiplier)
    //{
    //    ArmsScript arms = player.armsScript;
    //    if (timer > spinUpTime)
    //    {
    //        player.rb.AddForce(-arms.bulletSpawn.transform.right * knockback * knockbackMultiplier, ForceMode2D.Impulse);
    //        //player.rb.AddForce(inverseDir * knockbackMultiplier, ForceMode2D.Impulse);
    //        player.armsScript.cameraShake.shakeDuration += cameraShakeDuration;
    //        arms.timeSinceLastShot = 0;
    //    }
    //}
}
