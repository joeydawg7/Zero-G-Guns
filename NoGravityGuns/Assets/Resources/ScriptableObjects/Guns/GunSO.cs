using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Gun", menuName = "ScriptableObjects/Guns/StandardFire", order = 1)]
public class GunSO : ScriptableObject
{
    public string name;
    public int minDamageRange;
    public int maxDamageRange;
    public float knockback;
    public float bulletSpeed;
    public float recoilDelay;
    public int clipSize;
    public int numBullets;
    public float reloadTime;

    public GameObject armsObject;

    public Sprite weaponPad;

    public enum FireType { semiAuto, buckshot, fullAuto, Burst, rocket};
    public FireType fireType;

    //public PlayerScript.GunType GunType;

    [HideInInspector]
    public float gunDamageTotal;

    public float cameraShakeDuration;

    public List<AudioClip> bulletSounds;
    public AudioClip reloadSound;

    public GameObject projectile;
    public Sprite EquipSprite;


    public AudioClip GetRandomGunshotSFX
    {
        get { return bulletSounds[Random.Range(0, bulletSounds.Count)];  }
    }

    public int GunDamage
    {
        get { return Random.Range(minDamageRange, maxDamageRange); }
    }
    
    //shoots the gun
    public virtual void Fire(PlayerScript player, Vector3 dir)
    {
        player.armsScript.audioSource.PlayOneShot(GetRandomGunshotSFX);

        SpawnBullet(dir, player);

        ReduceBullets(player);
    }


    //gets rid of a bullet and checks if we're out and should swap back to pistol
    public virtual void ReduceBullets(PlayerScript player)
    {
        player.armsScript.currentAmmo--;

        if (player.armsScript.currentAmmo <= 0)
        {
            //reloadCoroutine = StartCoroutine(Reload());
            player.EquipArms(GameManager.Instance.pistol);
        }
    }

    //spawn the bullet and passes along required info
    protected virtual void SpawnBullet(Vector3 dir, PlayerScript player)
    {
        Transform bulletSpawn = player.armsScript.bulletSpawn;

        GameObject bulletGo = ObjectPooler.Instance.SpawnFromPool("Bullet", bulletSpawn.position, Quaternion.identity);
        dir = bulletSpawn.transform.right * bulletSpeed;

        bulletGo.GetComponent<Bullet>().Construct(GunDamage, player, dir, player.armsScript.bulletSprite, this);
    }

    //pushes player back
    public virtual void KnockBack(PlayerScript player, Vector2 shootDir)
    {
        ArmsScript arms = player.armsScript;

        player.rb.AddForce(-arms.bulletSpawn.transform.right * knockback, ForceMode2D.Impulse);
        player.armsScript.cameraShake.shakeDuration += cameraShakeDuration;
        arms.timeSinceLastShot = 0;
    }
}
