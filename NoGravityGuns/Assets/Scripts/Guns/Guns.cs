using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Guns : MonoBehaviour
{    
    public float knockBack;
    public int clipSize;
    public int numBullets;
    public float recoilDelay;
    public int minDamageRange;
    public int maxDamageRange;
    public float bulletSpeed;

    public string projectileTypeName;
    public float cameraShakeDuration;

    public List<AudioClip> bulletSounds;
    public AudioClip reloadSound;
    [Tooltip("Leave blank for nothing")]
    public AudioClip prefireSound;
    public AudioClip outOfAmmoSound;

    public GameObject gunPrefab;

    public Sprite theGunSprite;


    public AudioClip GetRandomGunshotSFX
    {
        get { return bulletSounds[Random.Range(0, bulletSounds.Count)]; }
    }

    public int GunDamage(int min, int max)
    {
        return Random.Range(min, max);
    }

    public abstract void Fire(PlayerScript player);
   

    public IEnumerator DelayShotCoroutine(PlayerScript player, float delayBeforeShot, float bulletSpeed, int minDamage, int maxDamage)
    {
        ArmsScript arms = player.armsScript;

        //GameObject tempGo = gunPrefab.transform.GetChild(0).transform.Find(muzzleFlash).gameObject;

        ParticleSystem ps = arms.currentGunGameObject.GetComponentInChildren<ParticleSystem>();

        if (ps != null)
        {
            var main = ps.main;
            main.startColor = new ParticleSystem.MinMaxGradient(player.playerColor);

            ps.Play(true);

            //var main = ps.main;
            //main.startColor = new Color(player.playerColor.r, player.playerColor.g, player.playerColor.b, player.playerColor.a);
            //var trails = ps.trails;


            if (prefireSound != null)
                arms.audioSource.PlayOneShot(prefireSound);
        }

        yield return new WaitForSeconds(delayBeforeShot);

        SpawnBullet(player, bulletSpeed, minDamage, maxDamage);
    }


    public virtual void ReduceBullets(PlayerScript player)
    {
        player.armsScript.currentAmmo--;
        if(player.armsScript.currentAmmo <= 0)
        {
            if(outOfAmmoSound != null)
            {
                player.armsScript.audioSource.PlayOneShot(outOfAmmoSound);
            }
            player.armsScript.EquipGun(GameManager.Instance.pistol, false);
        }
    }

    public void SpawnBullet(PlayerScript player, float bulletSpeed,int minDamagae,int maxDamage)
    {
        Transform bulletSpawn = player.armsScript.bulletSpawn;

        //replace blank entries with default bullet :D
        if (string.IsNullOrEmpty(projectileTypeName))
            projectileTypeName = "Bullet";

        GameObject bulletGo = ObjectPooler.Instance.SpawnFromPool(projectileTypeName, bulletSpawn.position, Quaternion.identity);
        var dir = bulletSpawn.transform.right * bulletSpeed;

        bulletGo.GetComponent<Bullet>().Construct(GunDamage(minDamagae, maxDamage), player, dir, player.playerColor);
    }
    
    public void KnockBack(PlayerScript player, float knockBackModifier)
    {
        ArmsScript arms = player.armsScript;

        player.rb.AddForce(-arms.bulletSpawn.transform.right * knockBack * knockBackModifier, ForceMode2D.Impulse);
        //player.rb.AddForce(inverseDir * knockbackMultiplier, ForceMode2D.Impulse);
        player.armsScript.cameraShake.shakeDuration += cameraShakeDuration;
        arms.timeSinceLastShot = 0;
    }
   
}
