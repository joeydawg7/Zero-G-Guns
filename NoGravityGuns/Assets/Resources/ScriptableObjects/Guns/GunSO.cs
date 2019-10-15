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
    public float delayBeforeShot;
    public int clipSize;
    public int numBullets;

    public GameObject gunPrefab;

    public Sprite theGun;

    //public PlayerScript.GunType GunType;

    [HideInInspector]
    public float gunDamageTotal;

    public float cameraShakeDuration;

    public List<AudioClip> bulletSounds;
    public AudioClip reloadSound;
    [Tooltip("Leave blank for nothing")]
    public AudioClip prefireSound;

    public string projectile;
    public Sprite EquipSprite;

    public string muzzleFlash;

    public AudioClip GetRandomGunshotSFX
    {
        get { return bulletSounds[Random.Range(0, bulletSounds.Count)]; }
    }

    public int GunDamage
    {
        get { return Random.Range(minDamageRange, maxDamageRange); }
    }

    //shoots the gun
    public virtual void Fire(PlayerScript player, Vector3 dir)
    {
        player.armsScript.audioSource.PlayOneShot(GetRandomGunshotSFX);

        player.StartCoroutine(DelayShotCoroutine(player, dir));

        ReduceBullets(player);
    }

    protected virtual IEnumerator DelayShotCoroutine(PlayerScript player, Vector3 dir)
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

        SpawnBullet(dir, player);
    }


    //gets rid of a bullet and checks if we're out and should swap back to pistol
    public virtual void ReduceBullets(PlayerScript player)
    {
        player.armsScript.currentAmmo--;

        if (player.armsScript.currentAmmo <= 0)
        {
            //reloadCoroutine = StartCoroutine(Reload());
            player.armsScript.EquipGun(GameManager.Instance.pistol);
        }
    }

    //spawn the bullet and passes along required info
    protected virtual void SpawnBullet(Vector3 dir, PlayerScript player)
    {
        Transform bulletSpawn = player.armsScript.bulletSpawn;

        //replace blank entries with default bullet :D
        if (projectile == "")
            projectile = "Bullet";

        GameObject bulletGo = ObjectPooler.Instance.SpawnFromPool(projectile, bulletSpawn.position, Quaternion.identity);
        dir = bulletSpawn.transform.right * bulletSpeed;


        bulletGo.GetComponent<Bullet>().Construct(GunDamage, player, dir, player.playerColor, this);
    }

    //pushes player back
    public virtual void KnockBack(PlayerScript player, Vector2 shootDir, float knockbackMultiplier)
    {
        ArmsScript arms = player.armsScript;


        player.rb.AddForce(-arms.bulletSpawn.transform.right * knockback * knockbackMultiplier, ForceMode2D.Impulse);
        //player.rb.AddForce(inverseDir * knockbackMultiplier, ForceMode2D.Impulse);
        player.armsScript.cameraShake.shakeDuration += cameraShakeDuration;
        arms.timeSinceLastShot = 0;
    }
}
