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

    public override void Fire(PlayerScript player)
    {
        //does nothing :D
        //player.StartCoroutine(PushBackBeforeKnockBack(player, knockbackMultiplier));
    }

    void KnockBackAfterShot(PlayerScript player, float knockbackMultiplier)
    {
        ArmsScript arms = player.armsScript;

        player.rb.AddForce(-arms.bulletSpawn.transform.right * base.knockBack * knockbackMultiplier, ForceMode2D.Impulse);
        player.armsScript.cameraShake.shakeDuration += cameraShakeDuration;
        arms.timeSinceLastShot = 0;
    }

    IEnumerator PushBackBeforeKnockBack(PlayerScript player, float knockbackMultiplier)
    {
        ArmsScript arms = player.armsScript;

        arms.timeSinceLastShot = 0;
        float timer = 0;

        arms.cameraShake.shakeDuration += cameraShakeDuration;
        arms.audioSource.PlayOneShot(GetRandomGunshotSFX);

        //sparks before shot
        //arms.currentArms.GetComponentInChildren<ParticleSystem>().Emit(UnityEngine.Random.Range(15, 40));

        //while we are holding the trigger down and still holding an rpg
        while (player.player.GetAxis("Shoot") > 0.5f && player.armsScript.currentWeapon.name == "RPG")
        {
            //time to give pushback
            if (timer < 0.5f)
                player.rb.AddForce(-arms.bulletSpawn.transform.right * base.knockBack * Time.deltaTime * rocketPushbackMod, ForceMode2D.Impulse);

            //rocket held too long, blow up in hand
            if (timer > 3.0f)
            {
                ExplodeInHand(player.armsScript);
                ReduceBullets(player);

                //break early so we dont also fire a real rocket
                yield break;
            }

            //timer determines pushback status and if the thing should just explode in hand
            timer += Time.deltaTime;
            //time since last shot remains 0 as long as this is held down so new rockets wont try to be shot
            arms.timeSinceLastShot = 0;

            yield return null;
        }

        //if its still an rpg... just checking :)
        if (player.armsScript.currentWeapon.name == "RPG")
        {
            player.rb.AddForce(arms.bulletSpawn.transform.right * base.knockBack, ForceMode2D.Impulse);
            arms.cameraShake.shakeDuration += cameraShakeDuration;
            arms.timeSinceLastShot = 0;

            base.SpawnBullet(player, bulletSpeed, minDamageRange, maxDamageRange);

            KnockBackAfterShot(player, knockbackMultiplier);

            base.ReduceBullets(player);
        }

        if (arms.currentAmmo <= 0)
        {
            //reloadCoroutine = StartCoroutine(Reload());
            player.armsScript.EquipGun(GameManager.Instance.pistol, false);
        }
    }

    void ExplodeInHand(ArmsScript arms)
    {
        GameObject bulletGo = ObjectPooler.Instance.SpawnFromPool(projectileTypeName, arms.bulletSpawn.transform.position, Quaternion.identity);
        bulletGo.GetComponent<SpriteRenderer>().enabled = false;
        bulletGo.GetComponent<Rocket>().ExplodeBullet(true);
    }
}
