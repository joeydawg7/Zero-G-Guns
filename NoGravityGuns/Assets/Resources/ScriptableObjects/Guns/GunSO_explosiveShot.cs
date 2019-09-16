using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Gun", menuName = "ScriptableObjects/Guns/Gun_explosiveShot", order = 1)]
public class GunSO_explosiveShot : GunSO
{
    [Header("explosive specific variables")]
    public float rocketPushbackMod;

    MonoBehaviour mono;

    public override void Fire(PlayerScript player, Vector3 dir)
    {
       //does nothing :D
    }

    protected override void SpawnBullet(GunSO currWeapon, Transform bulletSpawn, Vector3 shootDir, PlayerScript player)
    {
        GameObject bulletGo = ObjectPooler.Instance.SpawnFromPool("Rocket", bulletSpawn.transform.position, Quaternion.Euler(shootDir));
        bulletGo.GetComponent<SpriteRenderer>().enabled = false;
        Vector2 dir = bulletSpawn.transform.right * currWeapon.bulletSpeed;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        bulletGo.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        bulletGo.GetComponent<Bullet>().Construct(GunDamage, player, dir, player.armsScript.rocketSprite);
    }

    public override void KnockBack(PlayerScript player, Vector2 shootDir)
    {
        mono = player;
        mono.StartCoroutine(PushBackBeforeKnockBack(player, shootDir));
    }

    void KnockBackAfterShot(PlayerScript player, Vector3 dir)
    {
        ArmsScript arms = player.armsScript;

        player.rb.AddForce(-arms.bulletSpawn.transform.right * knockback, ForceMode2D.Impulse);
        player.armsScript.cameraShake.shakeDuration += cameraShakeDuration;
        arms.timeSinceLastShot = 0;
    }

    IEnumerator PushBackBeforeKnockBack(PlayerScript player, Vector3 dir)
    {
        ArmsScript arms = player.armsScript;

        arms.timeSinceLastShot = 0;
        float timer = 0;

        arms.cameraShake.shakeDuration += cameraShakeDuration;
        arms.audioSource.PlayOneShot(GetRandomGunshotSFX);

        //sparks before shot
        arms.currentArms.GetComponentInChildren<ParticleSystem>().Emit(UnityEngine.Random.Range(15, 40));

        //while we are holding the trigger down and still holding an rpg
        while (player.player.GetAxis("Shoot") > 0.5f && name == "RPG")
        {
            //time to give pushback
            if (timer < 0.5f)
                player.rb.AddForce(-arms.bulletSpawn.transform.right * knockback * Time.deltaTime * rocketPushbackMod, ForceMode2D.Impulse);

            //rocket held too long, blow up in hand
            if (timer > 3.0f)
            {
                ExplodeInHand(player.armsScript, dir);
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
        if (name == "RPG")
        {
            player.rb.AddForce(-dir * knockback, ForceMode2D.Impulse);
            arms.cameraShake.shakeDuration += cameraShakeDuration;
            arms.timeSinceLastShot = 0;

            SpawnBullet(this, arms.bulletSpawn, dir, player);

            KnockBackAfterShot(player, dir);

            ReduceBullets(player);
        }

        if (arms.currentAmmo <= 0)
        {
            //reloadCoroutine = StartCoroutine(Reload());
            player.EquipArms(GameManager.Instance.pistol);
        }
    }

    void ExplodeInHand(ArmsScript arms, Vector3 shootDir)
    {
        GameObject bulletGo = ObjectPooler.Instance.SpawnFromPool("Rocket", arms.bulletSpawn.transform.position, Quaternion.Euler(shootDir));
        bulletGo.GetComponent<SpriteRenderer>().enabled = false;
        bulletGo.GetComponent<Bullet>().ExplodeBullet(true);
    }
}
