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
    
    public float minSpeed, maxSpeed;
    
    
    public bool spinning;
    //public override int GunDamage()
    //{
    //    return Random.Range(minDamageRange, maxDamageRange); 
    //}

    public AudioClip spinUp, spinDown;

    private void Start()
    {
        spinning = false;
        recoilDelay = minSpeed;
    }

    public override void Fire(PlayerScript player)
    {

        if (CheckIfAbleToiFire(this))
        {            
           
            if(!spinning)
            {
                player.StartCoroutine(spinUpMinigun(player));

                //player.armsScript.audioSource.PlayOneShot(spinUp);
                //SoundPooler.Instance.PlaySoundEffect(spinUp);

            }
            if (recoilDelay > maxSpeed)
            {
                recoilDelay -= Time.deltaTime;
                //SoundPooler.Instance.PlaySoundEffect(spinDown);

            }
            player.StartCoroutine(DelayShotCoroutine(player, 0.0f, bulletSpeed, minDamageRange, maxDamageRange,this));
            ReduceBullets(player);
        }
        else
        {
            CheckForGunTimeout(player);
        }
       
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
        spinning = true;
        while (spinning)
        {
            if (player.player.GetAxis("Shoot") < 0.5f)
            {
                if (recoilDelay < minSpeed)
                {
                    recoilDelay += Time.deltaTime;
                }                
                yield return null;
            }
            yield return null;
        }        
    }
}
