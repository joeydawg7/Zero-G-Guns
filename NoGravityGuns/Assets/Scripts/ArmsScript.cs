using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.InputSystem.PlayerInput;
using UnityEngine.InputSystem.Controls;


public class ArmsScript : MonoBehaviour
{

    public PlayerScript basePlayer;

    [Header("Controller Settings")]
    public string horizontalAxis;
    public string verticalAxis;
    public string triggerAxis;
    public string XButton;

    [Header("Gun")]
    public GunSO currentWeapon;
    public int currentAmmo;
    public int totalBulletsGunCanLoad;
    public GameObject reloadTimer;
    public bool isReloading;
    public Transform bulletSpawn;
    public Sprite bulletSprite;
    public Sprite rocketSprite;
    public GameObject currentArms;

    [Header("Audio")]
    public AudioClip dryFire;

    //private
    bool interruptReload;
    Quaternion facing;
    Quaternion rotation;
    Vector2 shootDir;
    float currentRecoil;
    AudioSource audioS;
    Color32 startingColor;
    float timeSinceLastShot;
    private CameraShake cameraShake;
    Coroutine reloadCoroutine;
    Coroutine rotateCoroutine;
    Vector3 dir;
    GameManager gameManager;

    private void Awake()
    {
        timeSinceLastShot = 0;
        currentRecoil = 0;

        facing = transform.rotation;
        currentAmmo = currentWeapon.clipSize;
        totalBulletsGunCanLoad = currentWeapon.numBullets;

        audioS = GetComponent<AudioSource>();

        shootDir = new Vector3(0, 0, 0);

        reloadTimer.SetActive(false);
        interruptReload = false;

        cameraShake = Camera.main.GetComponent<CameraShake>();

    }

    private void Start()
    {
        gameManager = GameManager.Instance;
    }

    void AimController()
    {
        Vector2 rawAim = basePlayer.player.GetAxis2D("Move Horizontal", "Move Vertical");

        if (rawAim.magnitude > 0f)
        {
            // aiming stuff
            shootDir = Vector2.right * rawAim + Vector2.up * rawAim;
            shootDir = shootDir.normalized;
            rotation = Quaternion.LookRotation(Vector3.forward, -shootDir);
            rotation *= facing;
            transform.rotation = rotation;
        }

    }

    public void OnReload()
    {
        if (basePlayer.player.GetButtonDown("Reload"))
        {
            if (!isReloading && currentAmmo < currentWeapon.clipSize)
                reloadCoroutine = StartCoroutine(Reload());
        }
    }

    private void Update()
    {
        if (gameManager.isGameStarted)
        {
            CountShotDelay();

            if (!basePlayer.isDead && !basePlayer.isDummy)
            {
                AimController();
                OnReload();
                ShootController();
            }
        }
    }

    void CountShotDelay()
    {
        //delay shooting stuff
        timeSinceLastShot += Time.deltaTime;

        if (currentRecoil > 0)
            currentRecoil -= Time.deltaTime;
        else
            currentRecoil = 0;
    }

    void ShootController()
    {
        if (basePlayer.player.GetAxis("Shoot") > 0.5f)
        {
            OnShoot();
        }
    }

    public void EquipGun(GunSO weaponToEquip, GameObject gunObj)
    {
        //if we go to pick up a gun we already have just add more shots instead
        //if (weaponToEquip == currentWeapon)
        //{
        //    totalBulletsGunCanLoad += weaponToEquip.numBullets;
        //    return;
        //}

        //intterupt reload if were doing that
        if (isReloading)
        {
            if (reloadCoroutine != null)
            {
                StopCoroutine(reloadCoroutine);
                reloadCoroutine = null;
            }
            if (rotateCoroutine != null)
            {
                Debug.Log("cancelling rotate!");
                StopCoroutine(rotateCoroutine);
                reloadTimer.SetActive(false);
                rotateCoroutine = null;
            }

        }

        //set weapon and bullet stats for new gun
        currentWeapon = weaponToEquip;
        totalBulletsGunCanLoad = weaponToEquip.numBullets;
        currentAmmo = weaponToEquip.clipSize;

        isReloading = false;

        //find the new bulelt spawn location
        bulletSpawn = gunObj.transform.Find("BulletSpawner");
        //update UI
        SendGunText();
    }

    public void OnShoot()
    {
        if (currentRecoil > currentWeapon.recoilMax)
            currentRecoil = currentWeapon.recoilMax;

        if (basePlayer.isDead)
            return;

        //dry fire effect
        if (isReloading && currentWeapon.GunType != PlayerScript.GunType.shotgun)
        {
            if (timeSinceLastShot >= currentWeapon.recoilDelay)
            {
                audioS.PlayOneShot(dryFire);
                timeSinceLastShot = 0;
            }
        }


        //can't shoot during reload except for shotgun interupt
        if ((!isReloading) || (currentWeapon.GunType == PlayerScript.GunType.shotgun && currentAmmo > 0))
        {
            if (currentWeapon.GunType == PlayerScript.GunType.shotgun && isReloading)
            {
                //interruptReload = true;
                if (reloadCoroutine != null)
                {
                    StopCoroutine(reloadCoroutine);
                    reloadCoroutine = null;
                    isReloading = false;
                    SendGunText();
                    Debug.Log("interupting shotgun reload");

                    if (rotateCoroutine != null)
                    {
                        Debug.Log("cancelling rotate!");
                        StopCoroutine(rotateCoroutine);
                        reloadTimer.SetActive(false);
                        rotateCoroutine = null;
                    }

                }
            }

            if (timeSinceLastShot >= currentWeapon.recoilDelay && Time.timeScale != 0)
            {
                //add force to player in opposite direction of shot
                if (currentWeapon.GunType != PlayerScript.GunType.RPG)
                    KnockBack(shootDir);

                //muzzle flash
                ParticleSystem[] particles = currentArms.GetComponentsInChildren<ParticleSystem>();
                foreach (var item in particles)
                {
                    item.Play();
                }

                switch (currentWeapon.fireType)
                {
                    case GunSO.FireType.semiAuto:
                        ShootyGunTemp();
                        break;
                    case GunSO.FireType.buckshot:
                        BuckShot();
                        break;
                    case GunSO.FireType.fullAuto:
                        ShootyGunTemp();
                        break;
                    case GunSO.FireType.Burst:
                        StartCoroutine(FireInBurst());
                        break;
                    case GunSO.FireType.rocket:
                        RocketKnockBack(shootDir);
                        break;
                    default:
                        ShootyGunTemp();
                        break;
                }

                if (currentAmmo <= 0 && totalBulletsGunCanLoad <= 0)
                    basePlayer.EquipArms(PlayerScript.GunType.pistol, gameManager.pistol);

                SendGunText();
                basePlayer.shotsFired++;
            }


        }

    }

    #region UIStuff
    public string AmmoText()
    {
        return currentAmmo + "/" + currentWeapon.clipSize + " (" + ((totalBulletsGunCanLoad < 2000) ? totalBulletsGunCanLoad.ToString() : "\u221E") + ")";
    }

    public void SendGunText()
    {
        basePlayer.playerUIPanel.SetGunText(currentWeapon);
        basePlayer.playerUIPanel.SetAmmoText(AmmoText());
    }

    public void SendGunText(string s)
    {
        basePlayer.playerUIPanel.SetAmmoText(s);
    }
    #endregion

    #region Reload
    IEnumerator Rotate(float duration)
    {
        reloadTimer.SetActive(true);

        float startRotation = 0f;
        float endRotation = startRotation + 360.0f;
        float t = 0.0f;
        float FinalZRot = 0;
        while (t < duration)
        {
            if (interruptReload)
                break;

            t += Time.deltaTime;
            float zRotation = Mathf.Lerp(startRotation, endRotation, t / duration) % 360.0f;
            reloadTimer.transform.eulerAngles = new Vector3(reloadTimer.transform.eulerAngles.x, reloadTimer.transform.eulerAngles.y, zRotation);
            FinalZRot = zRotation;
            yield return null;
        }

        reloadTimer.transform.eulerAngles = new Vector3(reloadTimer.transform.eulerAngles.x, reloadTimer.transform.eulerAngles.y, FinalZRot);
        reloadTimer.SetActive(false);
    }


    IEnumerator Reload()
    {
        //no bullets to load, just drop gun and get out of here
        if (totalBulletsGunCanLoad <= 0)
        {
            Debug.Log("Tried to reload but theres not enough bullets so equipped pistol");
            basePlayer.EquipArms(PlayerScript.GunType.pistol, gameManager.pistol);
            yield break;
        }

        int shotsToReload = 0;

        shotsToReload = (totalBulletsGunCanLoad - currentWeapon.clipSize) + currentAmmo;
        shotsToReload = totalBulletsGunCanLoad - shotsToReload;

        isReloading = true;
        SendGunText("Reloading...");

        float reloadtimeIncrememnts = (float)currentWeapon.reloadTime / 6;

        //change reload type base on gun
        if (currentWeapon.GunType == PlayerScript.GunType.LMG)
        {
            //make lmg sound like you shoving bullets in it
            rotateCoroutine = StartCoroutine(Rotate(reloadtimeIncrememnts * 6));
            for (int i = 0; i < 6; i++)
            {
                yield return new WaitForSeconds(reloadtimeIncrememnts);
                GetComponent<AudioSource>().PlayOneShot(currentWeapon.reloadSound);
                SendGunText("Reloading...");
            }
        }
        else if (currentWeapon.GunType == PlayerScript.GunType.shotgun)
        {
            //make shotgun work like loading shells
            reloadtimeIncrememnts = (float)currentWeapon.reloadTime / 3f;

            rotateCoroutine = StartCoroutine(Rotate(reloadtimeIncrememnts * shotsToReload));

            for (int i = 0; i < shotsToReload; i++)
            {
                //more than 1 bullet to load and clipsize not yet reached
                if (totalBulletsGunCanLoad > 0 && currentAmmo < currentWeapon.clipSize)
                {
                    //if (interruptReload)
                    //{
                    //    interruptReload = false;
                    //    isReloading = false;
                    //    SendGunText();
                    //    yield break;
                    //}

                    yield return new WaitForSeconds(reloadtimeIncrememnts);

                    audioS.PlayOneShot(currentWeapon.reloadSound);
                    currentAmmo++;
                    totalBulletsGunCanLoad--;
                    SendGunText();

                    //if (interruptReload)
                    //{
                    //    interruptReload = false;
                    //    isReloading = false;
                    //    SendGunText();
                    //    yield break;
                    //}
                }
                else
                {
                    isReloading = false;
                    interruptReload = false;
                    SendGunText();
                    yield break;
                }

            }
        }
        else
        {
            //every other reload just play one sound
            audioS.PlayOneShot(currentWeapon.reloadSound);
            rotateCoroutine = StartCoroutine(Rotate(reloadtimeIncrememnts * 6));
            SendGunText("Reloading...");
            //haha this sucks who wrote this garbage-ass code
            yield return new WaitForSeconds(reloadtimeIncrememnts * 6);

        }

        isReloading = false;

        if (currentWeapon.GunType == PlayerScript.GunType.pistol)
        {
            currentAmmo = currentWeapon.clipSize;
            currentAmmo = currentWeapon.clipSize;
        }
        else if (currentWeapon.GunType != PlayerScript.GunType.shotgun)
        {
            totalBulletsGunCanLoad -= shotsToReload;
            currentAmmo += shotsToReload;
        }

        if (totalBulletsGunCanLoad < 0)
            totalBulletsGunCanLoad = 0;

        //do last
        SendGunText();

    }
    #endregion

    #region Shoot Various Guns
    void KnockBack(Vector2 shootDir)
    {
        float knockback = currentWeapon.knockback;

        basePlayer.rb.AddForce(-bulletSpawn.transform.right * knockback, ForceMode2D.Impulse);
        cameraShake.shakeDuration += currentWeapon.cameraShakeDuration;
        timeSinceLastShot = 0;
    }

    IEnumerator FireInBurst()
    {
        GunSO currWeapon = currentWeapon;
        for (int i = 0; i < 3; i++)
        {
            //cone of -1 to 1 multiplied by current recoil amount to determine just how random it can be
            float recoilMod = UnityEngine.Random.Range(-1f, 1f) * currentRecoil;

            currentRecoil += currentWeapon.recoilPerShot;

            SpawnBullet(currWeapon);

            currentAmmo--;

            if (audioS.isPlaying)
                audioS.Stop();

            audioS.PlayOneShot(currentWeapon.GetRandomGunshotSFX);

            SendGunText();
            yield return new WaitForSeconds(0.08f);
        }

        if (currentAmmo <= 0)
        {
            reloadCoroutine = StartCoroutine(Reload());
        }

    }

    void BuckShot()
    {
        currentAmmo--;

        currentRecoil += currentWeapon.recoilPerShot;

        SpawnBuckShot();

        if (audioS.isPlaying)
            audioS.Stop();
        audioS.PlayOneShot(currentWeapon.GetRandomGunshotSFX);

        if (currentAmmo <= 0)
        {
            StartCoroutine(Reload());
        }

    }

    void SpawnBuckShot()
    {
        for (int i = 0; i < UnityEngine.Random.Range(5, 8); i++)
        {
            // randomize angle variation between bullets
            float spreadAngle = UnityEngine.Random.Range(
           -10f,
            10f);

            GameObject bulletGo = ObjectPooler.Instance.SpawnFromPool("Bullet", bulletSpawn.transform.position, Quaternion.identity);
            dir = bulletSpawn.transform.right;

            float rotateAngle = spreadAngle +
           (Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);

            Vector2 MovementDirection = new Vector2(
          Mathf.Cos(rotateAngle * Mathf.Deg2Rad),
          Mathf.Sin(rotateAngle * Mathf.Deg2Rad)).normalized;

            MovementDirection *= currentWeapon.bulletSpeed;

            bulletGo.GetComponent<Bullet>().Construct(basePlayer.playerID, currentWeapon.GunDamage, basePlayer, bulletSprite, currentWeapon.GunType, MovementDirection, basePlayer.collisionLayer);

        }

    }

    IEnumerator waitthendont()
    {
        yield return new WaitForSeconds(0.3f);
        Debug.Break();
    }

    void ShootyGunTemp()
    {
        GunSO currWeapon = currentWeapon;

        audioS.PlayOneShot(currentWeapon.GetRandomGunshotSFX);

        float recoilMod = UnityEngine.Random.Range(-1f, 1f) * currentRecoil;

        currentRecoil += currentWeapon.recoilPerShot;

        currentAmmo--;

        SpawnBullet(currWeapon);

        if (currentAmmo <= 0)
        {
            reloadCoroutine = StartCoroutine(Reload());
        }
            
    }

    void RocketKnockBack(Vector2 shootDir)
    {
        StartCoroutine(PushBackBeforeKnockBack(shootDir));
    }

    const float ROCKET_PUSHBACK_MOD = 1.2f;

    IEnumerator PushBackBeforeKnockBack(Vector2 shootDir)
    {
        GunSO currWeapon = currentWeapon;

        timeSinceLastShot = 0;
        float timer = 0;

        cameraShake.shakeDuration += currWeapon.cameraShakeDuration;

        audioS.PlayOneShot(currentWeapon.GetRandomGunshotSFX);

        currentArms.GetComponentInChildren<ParticleSystem>().Emit(UnityEngine.Random.Range(15, 40));

        while (timer < 0.5f)
        {
            basePlayer.rb.AddForce(-bulletSpawn.transform.right * currWeapon.knockback * Time.deltaTime * ROCKET_PUSHBACK_MOD, ForceMode2D.Impulse);

            timer += Time.deltaTime;

            yield return null;
        }

        basePlayer.rb.AddForce(-shootDir * currWeapon.knockback, ForceMode2D.Impulse);
        cameraShake.shakeDuration += currentWeapon.cameraShakeDuration;
        timeSinceLastShot = 0;

        SpawnRocket(currWeapon);

        currentAmmo--;

        if (currentAmmo <= 0)
        {
            reloadCoroutine = StartCoroutine(Reload());
        }
    }

    void SpawnBullet(GunSO currWeapon)
    {
        GameObject bulletGo = ObjectPooler.Instance.SpawnFromPool("Bullet", bulletSpawn.transform.position, Quaternion.identity);
        dir = bulletSpawn.transform.right * currWeapon.bulletSpeed;

        bulletGo.GetComponent<Bullet>().Construct(basePlayer.playerID, currentWeapon.GunDamage, basePlayer, bulletSprite, currentWeapon.GunType, dir, basePlayer.collisionLayer);
    }

    void SpawnRocket(GunSO currWeapon)
    {
        //dir = bulletSpawn.transform.right * currWeapon.bulletSpeed;
        GameObject bulletGo = ObjectPooler.Instance.SpawnFromPool("Rocket", bulletSpawn.transform.position, Quaternion.Euler(shootDir));
        bulletGo.GetComponent<SpriteRenderer>().enabled = false;
        Vector2 dir = bulletSpawn.transform.right * currWeapon.bulletSpeed;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        bulletGo.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        bulletGo.GetComponent<Bullet>().Construct(basePlayer.playerID, currentWeapon.GunDamage, basePlayer, rocketSprite, PlayerScript.GunType.RPG, dir, basePlayer.collisionLayer);
    }
    #endregion

    private void DrawHelperAtCenter(Vector3 direction, Color color, float scale)
    {
        Gizmos.color = color;
        Vector3 destination = transform.position + direction * scale;
        Gizmos.DrawLine(bulletSpawn.transform.position, destination);
    }



}
