using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RPG : Guns
{
    public string name;
    [Header("explosive specific variables")]
    public float rocketPushbackMod;

    public float physicsObjectPushForceMod;

    public float explosionRadius;
    public float explosionPower;

    public float rocketMaxSpeed;
    public float rocketAccelerationMod;
    public float damageAtCenter;

    //public float knockback;
    public float knockbackMultiplier;
    public Sprite emptyRPG;    

    private bool isReloading;
    

    private void Awake()
    {       
        isReloading = false;
    }

    private void Update()
    {
        if (isReloading)
        {
            if (CheckIfAbleToiFire(this))
            {
                this.gameObject.GetComponent<SpriteRenderer>().sprite = theGunSprite;
                isReloading = false;
            }
        }
    }

    public override void Fire(PlayerScript player)
    {       
        //base.KnockBack(player, player.knockbackMultiplier);
        if(CheckIfAbleToiFire(this))
        {            
            player.StartCoroutine(PushBackBeforeKnockBack(player));
        }
        else
        {
            CheckForAmmo(player);
        }
    }        

    IEnumerator PushBackBeforeKnockBack(PlayerScript player)
    {
        this.gameObject.GetComponent<SpriteRenderer>().sprite = theGunSprite;
        base.timeSinceLastShot = Time.time;
        base.KnockBack(player, player.knockbackMultiplier);
        player.armsScript.audioSource.PlayOneShot(base.GetRandomGunshotSFX);
        float timer = 0.0f;
        //while we are holding the trigger down and still holding an rpg
        while (player.player.GetAxis("Shoot") > 0.5f /*&& player.armsScript.currentWeapon.name == "RPG"*/)
        {
            //rocket held too long, blow up in hand
            if (timer > 3.0f)
            {
                ExplodeInHand(player.armsScript);
                ReduceBullets(player);

                //break early so we dont also fire a real rocket
                yield break;
            }

            timer += Time.deltaTime;
            //time since last shot remains 0 as long as this is held down so new rockets wont try to be shot
            base.timeSinceLastShot = Time.time;

            yield return null;
        }
     
            base.KnockBack(player, player.knockbackMultiplier);
            player.armsScript.audioSource.PlayOneShot(base.GetRandomGunshotSFX);
            base.SpawnBullet(player, bulletSpeed, minDamageRange, maxDamageRange);            
            base.ReduceBullets(player);
            this.gameObject.GetComponent<SpriteRenderer>().sprite = emptyRPG;
            isReloading = true;



        if (player.armsScript.currentAmmo < 1)
        {
            player.armsScript.EquipGun(GameManager.Instance.pistol, true);
        }
      
    }

    
    void ExplodeInHand(ArmsScript arms)
    {
        GameObject bulletGo = ObjectPooler.Instance.SpawnFromPool(projectileTypeName, arms.bulletSpawn.transform.position, Quaternion.identity);
        bulletGo.GetComponent<SpriteRenderer>().enabled = false;
        bulletGo.GetComponent<Rocket>().ExplodeBullet(true, this);
    }
}
