using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Gun", menuName = "ScriptableObjects/Guns/Gun_grappleShot", order = 1)]
public class GunSO_grappleShot : GunSO
{
    MonoBehaviour mono;

    Grapple grapple;


    public override void Fire(PlayerScript player, Vector3 dir)
    {
        //do nothing
    }

    //spawn the bullet and passes along required info
    protected override void SpawnBullet(Vector3 dir, PlayerScript player)
    {
        Transform bulletSpawn = player.armsScript.bulletSpawn;

        GameObject bulletGo = ObjectPooler.Instance.SpawnFromPool("", bulletSpawn.position, Quaternion.identity);
        dir = bulletSpawn.transform.right * bulletSpeed;

        grapple = bulletGo.GetComponent<Grapple>();
        grapple.Construct(GunDamage, player, dir, player.playerColor, this);

    }


    public override void KnockBack(PlayerScript player, Vector2 shootDir, float knockbackMultiplier)
    {
        mono = player;
        //mono.StartCoroutine(HoldGrapple(player, shootDir));
        ArmsScript arms = player.armsScript;

        arms.timeSinceLastShot = 0;
        float timer = 0;

        arms.cameraShake.shakeDuration += cameraShakeDuration;
        arms.audioSource.PlayOneShot(GetRandomGunshotSFX);

        if (name == "GrappleHook")
        {
            SpawnBullet(shootDir, player);
            ReduceBullets(player);
        }
    }

    public void StartGrapplePull(PlayerScript player)
    {
        mono.StartCoroutine(HoldGrapple(player));
    }

    IEnumerator HoldGrapple(PlayerScript player)
    {
        ArmsScript arms = player.armsScript;
       

        if (grapple == null)
        {
            Debug.LogError("No grapple found after bullet spawn!");
            yield break;
        }

        //while we are holding the trigger down
        while (player.player.GetAxis("Shoot") > 0.5f && grapple.isAttached)
        {
            //TODO: pull player in towards grapple point
            grapple.UpdateDistance(-Time.deltaTime*5); //TODO: multiplied by some mod 

            arms.timeSinceLastShot = 0;

            yield return null;
        }


        //get rid of previous grapple
        grapple.RemoveGrapple();
        grapple = null;

        arms.timeSinceLastShot = 0;

        if (arms.currentAmmo <= 0)
        {
            //reloadCoroutine = StartCoroutine(Reload());
            player.armsScript.EquipGun(GameManager.Instance.pistol, false);
        }
    }


}