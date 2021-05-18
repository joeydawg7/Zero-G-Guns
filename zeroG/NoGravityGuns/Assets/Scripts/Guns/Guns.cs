using System.Collections;
using System.Collections.Generic;
using XInputDotNetPure;
using Rewired;
using UnityEngine;
using UnityEngine.Experimental.Rendering.LWRP;

public abstract class Guns : MonoBehaviour
{
    public float knockBack;
    public int clipSize;
    [HideInInspector]
    public int numBullets;
    //  public float time;
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
    public float timeSinceLastShot;

    public float vibrateAmount;

    public GameObject gunPrefab;

    public Sprite theGunSprite;

    [HideInInspector]
    public bool canFire;

    private void Start()
    {
        numBullets = clipSize;
    }

    public AudioClip GetRandomGunshotSFX
    {
        get { return bulletSounds[Random.Range(0, bulletSounds.Count - 1)]; }
    }

    public int GunDamage(int min, int max)
    {
        return Random.Range(min, max);
    }

    public abstract void Fire(PlayerScript player);


    public IEnumerator DelayShotCoroutine(PlayerScript player, float delayBeforeShot, float bulletSpeed, int minDamage, int maxDamage, Guns gun)
    {

        timeSinceLastShot = Time.time;
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
                SoundPooler.Instance.PlaySoundEffect(prefireSound);
                //arms.audioSource.PlayOneShot(prefireSound);
        }

        if (player.armsScript.currentWeapon != gun)
            yield break;


        yield return new WaitForSeconds(delayBeforeShot);

        if(player.armsScript.currentWeapon!=gun)
            yield break;

        KnockBack(player, player.knockbackMultiplier);
        //player.armsScript.audioSource.PlayOneShot(GetRandomGunshotSFX);
        SoundPooler.Instance.PlaySoundEffect(GetRandomGunshotSFX);

        if (timeSinceLastShot > recoilDelay && player.armsScript.currentWeapon == gun)
            SpawnBullet(player, bulletSpeed, minDamage, maxDamage, gun);
           
    }


    public virtual void ReduceBullets(PlayerScript player)
    {
        player.armsScript.currentAmmo--;

        if (player.armsScript.currentAmmo <= 0)
        {
            if (outOfAmmoSound != null)
            {
                player.armsScript.audioSource.PlayOneShot(outOfAmmoSound);
            }

            //canFire = false;
            player.armsScript.EquipGun(GameManager.Instance.pistol, false);
        }
    }

    public void SpawnBullet(PlayerScript player, float bulletSpeed, int minDamagae, int maxDamage, Guns gun)
    {
        //before spawing a bullet, check that the held weapon is the same as the one that was intended to shoot the shot. Fixes a bug where you switch weapons mid-fire causing an error
        if (player.armsScript.currentWeapon != gun)
            projectileTypeName = gun.projectileTypeName;

        if (gun == null)
        {
            projectileTypeName = player.armsScript.currentWeapon.projectileTypeName;

        }

        Transform bulletSpawn = player.armsScript.bulletSpawn;

        //replace blank entries with default bullet :D
        if (string.IsNullOrEmpty(projectileTypeName))
            projectileTypeName = "Bullet";



        if (projectileTypeName == "BuckShot")
        {
            float spreadAngle = 22.0f;
            for (int i = 0; i < 6; i++)
            {

                GameObject bulletGo = ObjectPooler.Instance.SpawnFromPool("BuckShot", bulletSpawn.transform.position, Quaternion.identity);
                var dir = bulletSpawn.transform.right;

                float rotateAngle = spreadAngle +
               (Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);

                Vector2 MovementDirection = new Vector2(
              Mathf.Cos(rotateAngle * Mathf.Deg2Rad),
              Mathf.Sin(rotateAngle * Mathf.Deg2Rad)).normalized;

                MovementDirection *= bulletSpeed;

                bulletGo.GetComponent<Bullet>().Construct(GunDamage(minDamagae, maxDamage), player, MovementDirection, player.playerColor);
                spreadAngle -= 3.6f;

                if (bulletGo.GetComponent<Light2D>())
                    bulletGo.GetComponent<Light2D>().color = player.playerColor;


            }
        }
        else if (projectileTypeName == "Rocket")
        {
            GameObject bulletGo = ObjectPooler.Instance.SpawnFromPool("Rocket", bulletSpawn.transform.position, Quaternion.identity);
            bulletGo.GetComponent<SpriteRenderer>().enabled = false;
            Vector2 dir = bulletSpawn.transform.right * bulletSpeed;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            bulletGo.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            bulletGo.GetComponent<Rocket>().Construct(GunDamage(minDamageRange, maxDamageRange), player, dir, player.playerColor, gunPrefab.GetComponent<RPG>());

            if (bulletGo.GetComponent<Light2D>())
                bulletGo.GetComponent<Light2D>().color = player.playerColor;



        }
        else if (projectileTypeName == "Acid_Bullet")
        {
            GameObject bulletGo = ObjectPooler.Instance.SpawnFromPool("Acid_Bullet", bulletSpawn.transform.position, Quaternion.identity);
            var dir = bulletSpawn.transform.right * bulletSpeed;
            bulletGo.GetComponent<DOT_Bullet>().Construct(GunDamage(minDamagae, maxDamage), player, dir, player.playerColor);
        }
        else if (projectileTypeName == "Lightning_Bullet")
        {
            GameObject bulletGo = ObjectPooler.Instance.SpawnFromPool("Lightning_Bullet", bulletSpawn.transform.position, Quaternion.identity);
            var dir = bulletSpawn.transform.right * bulletSpeed;
            bulletGo.GetComponent<LightningBullet>().Construct(GunDamage(minDamagae, maxDamage), player, dir, player.playerColor);
        }

        else
        {
            if (!bulletSpawn)
            {
                bulletSpawn.position = player.armsScript.GetBulletSpawnPos();         
            }

            GameObject bulletGo = ObjectPooler.Instance.SpawnFromPool(projectileTypeName, bulletSpawn.position, Quaternion.identity);
            var dir = bulletSpawn.transform.right * bulletSpeed;

            if (projectileTypeName == "BlackHoleSun")
            {
                //bulletGo.GetComponent<BlackHoleSun>().Construct(GunDamage(minDamagae, maxDamage), player, dir, player.playerColor);
            }
            else
            {
                bulletGo.GetComponent<Bullet>().Construct(GunDamage(minDamagae, maxDamage), player, dir, player.playerColor);
            }

            if (bulletGo.GetComponent<Light2D>())
                bulletGo.GetComponent<Light2D>().color = player.playerColor;


        }
    }

    public void KnockBack(PlayerScript player, float knockBackModifier)
    {
        ArmsScript arms = player.armsScript;

        if (player.transform && arms.bulletSpawn)
        {
            player.rb.AddForce(-arms.bulletSpawn.transform.right * knockBack * knockBackModifier, ForceMode2D.Impulse);
        }

        //if (!RoundManager.Instance.debugManager.useDebugSettings)
        //{

        //Vibrate(player);


        //}


        //player.rb.AddForce(inverseDir * knockbackMultiplier, ForceMode2D.Impulse);
        //player.armsScript.cameraShake.shakeDuration += cameraShakeDuration;        
    }

    //private void Vibrate(PlayerScript player)
    //{
    //    player.Vibrate(vibrateAmount, 0.25f);
    //}

    public bool CheckIfAbleToiFire(Guns gun)
    {
        //gotta have bullets to shoot

        //enough time has passed between shots and not paused
        if ((Time.time - timeSinceLastShot) >= gun.recoilDelay && Time.timeScale != 0 && canFire)
        {
            //player.armsScript.currentWeapon.Fire(player);
            return true;
        }

        return false;
    }

    public void CheckForGunTimeout(PlayerScript player)
    {
        //if(player.armsScript.timeYouCanHoldGun <= 0)
        //{
        //    player.armsScript.audioSource.PlayOneShot(player.armsScript.dryFire);
        //    timeSinceLastShot = Time.time;
        //    player.armsScript.EquipGun(ObjectPooler.Instance.defaultPistol, true);
        //}        
    }


}
