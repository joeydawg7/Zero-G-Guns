using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

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
    public GameObject currentArms;

    [Header("Audio")]
    public AudioClip dryFire;


    //private
    bool interruptReload;
    Quaternion facing;
    Quaternion rotation;
    Vector2 shootDir;
    Vector3 bulletSpawnPoint;
    Vector3 aim;
    float currentRecoil;
    AudioSource audioS;
    Color32 startingColor;
    float timeSinceLastShot;
    private CameraShake cameraShake;
    Coroutine reloadCoroutine;
    Coroutine rotateCoroutine;
    Vector3 dir;

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

    public void SetChildrenWithAxis(int playerID)
    {
        foreach (Transform child in transform)
        {
            if (child.tag == "Arms")
            {
                ArmsScript arms = child.GetComponent<ArmsScript>();
                arms.triggerAxis = "J" + playerID + "Trigger";
                arms.horizontalAxis = "J" + playerID + "Horizontal";
                arms.verticalAxis = "J" + playerID + "Vertical";
            }
        }

    }

    public void UnsetChildrenWithAxis(int playerID)
    {
        foreach (Transform child in transform)
        {
            if (child.tag == "Arms")
            {
                ArmsScript arms = child.GetComponent<ArmsScript>();
                arms.triggerAxis = "";
                arms.horizontalAxis = "";
                arms.verticalAxis = "";

            }
        }

    }


    private void Update()
    {
        if (GameManager.Instance.isGameStarted && Input.GetButton(XButton) && !isReloading && currentAmmo < currentWeapon.clipSize)
            reloadCoroutine = StartCoroutine(Reload());

    }

    void FixedUpdate()
    {
        if (GameManager.Instance.isGameStarted)
            ShootController();
    }


    public void EquipGun(GunSO weaponToEquip, GameObject gunObj)
    {
        if (isReloading)
        {
            if (reloadCoroutine != null)
            {
                StopCoroutine(reloadCoroutine);
                reloadCoroutine = null;
            }
            if (rotateCoroutine != null)
            {
                StopCoroutine(rotateCoroutine);
                reloadTimer.SetActive(false);
                rotateCoroutine = null;
            }

            isReloading = false;
        }



        if (weaponToEquip == currentWeapon)
        {
            totalBulletsGunCanLoad += weaponToEquip.numBullets;
            Debug.Log("adding " + totalBulletsGunCanLoad + " to " + weaponToEquip.numBullets);
        }
        else
        {
            currentWeapon = weaponToEquip;
            totalBulletsGunCanLoad = weaponToEquip.numBullets;
            currentAmmo = weaponToEquip.clipSize;
            Debug.Log("setting ammo = " + weaponToEquip.clipSize);
        }

        isReloading = false;
        bulletSpawn = gunObj.transform.Find("BulletSpawner");
        SendGunText();
    }



    void ShootController()
    {
        bulletSpawnPoint = bulletSpawn.position;

        timeSinceLastShot += Time.deltaTime;

        if (currentRecoil > 0)
            currentRecoil -= Time.deltaTime;
        else
            currentRecoil = 0;

        if (currentRecoil > currentWeapon.recoilMax)
            currentRecoil = currentWeapon.recoilMax;

        if (!basePlayer.isDead)
        {

            shootDir = Vector2.right * Input.GetAxis(horizontalAxis) + Vector2.up * Input.GetAxis(verticalAxis);

            shootDir = shootDir.normalized;

            rotation = Quaternion.LookRotation(Vector3.forward, -shootDir);
            rotation *= facing;
            transform.rotation = rotation;

            if (Input.GetAxisRaw(triggerAxis) > 0 && isReloading)
            {
                if (timeSinceLastShot >= currentWeapon.recoilDelay)
                {
                    audioS.PlayOneShot(dryFire);
                    timeSinceLastShot = 0;
                }
            }
            //can't shoot during reload except for shotgun interupt
            if (((Input.GetAxisRaw(triggerAxis) > 0 && !isReloading) || ((Input.GetAxisRaw(triggerAxis) > 0 && currentWeapon.GunType == PlayerScript.GunType.shotgun && currentAmmo > 0))) && shootDir.magnitude != 0)
            {
                if (currentWeapon.GunType == PlayerScript.GunType.shotgun && isReloading)
                {
                    interruptReload = true;
                }

                if (Input.GetAxis(horizontalAxis) != 0 || Input.GetAxis(verticalAxis) != 0)
                {
                    //aim = new Vector3(Input.GetAxis(horizontalAxis), Input.GetAxis(verticalAxis), 0).normalized;
                    aim = shootDir;
                }
                if (aim.sqrMagnitude >= 0.1f)
                {
                    if (timeSinceLastShot >= currentWeapon.recoilDelay)
                    {
                        //add force to player in opposite direction of shot
                        KnockBack(shootDir);

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
                            default:
                                ShootyGunTemp();
                                break;
                        }

                        if (currentAmmo <= 0 && totalBulletsGunCanLoad <= 0)
                            basePlayer.EquipArms(PlayerScript.GunType.pistol, GameManager.Instance.pistol);

                        SendGunText();
                        basePlayer.shotsFired++;
                    }
                }

            }
        }
    }
    public string GunInfo()
    {
        return currentWeapon.name + ": " + currentAmmo + "/" + currentWeapon.clipSize + " (" + ((totalBulletsGunCanLoad < 2000) ? totalBulletsGunCanLoad.ToString() : "\u221E") + ")";
    }

    public void SendGunText()
    {
        basePlayer.playerUIPanel.setGun(GunInfo());
    }

    public void SendGunText(string s)
    {
        basePlayer.playerUIPanel.setGun(s);
    }

    void KnockBack(Vector2 shootDir)
    {
        basePlayer.rb.AddForce(-shootDir * currentWeapon.knockback, ForceMode2D.Impulse);
        cameraShake.shakeDuration += currentWeapon.cameraShakeDuration;
        timeSinceLastShot = 0;
    }

    void ShootyGunTemp()
    {
        //cone of -1 to 1 multiplied by current recoil amount to determine just how random it can be
        float recoilMod = UnityEngine.Random.Range(-1f, 1f) * currentRecoil;

        bulletSpawnPoint = new Vector3(bulletSpawnPoint.x, bulletSpawnPoint.y);

        currentRecoil += currentWeapon.recoilPerShot;

        SpawnBullet();

        currentAmmo--;

        audioS.PlayOneShot(currentWeapon.GetRandomGunshotSFX);

        if (currentAmmo <= 0)
        {
            reloadCoroutine = StartCoroutine(Reload());
        }


    }


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
        if (totalBulletsGunCanLoad > 0)
        {
            int shotsToReload = 0;

            shotsToReload = (totalBulletsGunCanLoad - currentWeapon.clipSize) + currentAmmo;
            shotsToReload = totalBulletsGunCanLoad - shotsToReload;


            isReloading = true;
            SendGunText("Reloading...");

            float reloadtimeIncrememnts = (float)currentWeapon.reloadTime / 6;

            //change sfx type base on reload type of the gun
            if (currentWeapon.GunType == PlayerScript.GunType.LMG)
            {
                rotateCoroutine = StartCoroutine(Rotate(reloadtimeIncrememnts * 6));
                for (int i = 0; i < currentWeapon.reloadTime; i++)
                {
                    yield return new WaitForSeconds(reloadtimeIncrememnts);
                    GetComponent<AudioSource>().PlayOneShot(currentWeapon.reloadSound);
                    SendGunText("Reloading...");
                }
            }
            else if (currentWeapon.GunType == PlayerScript.GunType.shotgun)
            {
                reloadtimeIncrememnts = (float)currentWeapon.reloadTime / 3f;

                rotateCoroutine = StartCoroutine(Rotate(reloadtimeIncrememnts * shotsToReload));

                for (int i = 0; i < currentWeapon.clipSize; i++)
                {

                    if (totalBulletsGunCanLoad > 0 && currentAmmo < currentWeapon.clipSize)
                    {
                        if (interruptReload)
                        {
                            interruptReload = false;
                            isReloading = false;
                            SendGunText();
                            yield break;
                        }

                        yield return new WaitForSeconds(reloadtimeIncrememnts);

                        audioS.PlayOneShot(currentWeapon.reloadSound);
                        currentAmmo++;
                        totalBulletsGunCanLoad--;
                        SendGunText();

                        if (interruptReload)
                        {
                            interruptReload = false;
                            isReloading = false;
                            SendGunText();
                            yield break;
                        }
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
                audioS.PlayOneShot(currentWeapon.reloadSound);
                rotateCoroutine = StartCoroutine(Rotate(currentWeapon.reloadTime));
                for (int i = 0; i < currentWeapon.reloadTime; i++)
                {
                    yield return new WaitForSeconds(reloadtimeIncrememnts);
                    SendGunText("Reloading.");
                    yield return new WaitForSeconds(reloadtimeIncrememnts);
                    SendGunText("Reloading..");
                    yield return new WaitForSeconds(reloadtimeIncrememnts);
                    SendGunText("Reloading...");
                }
            }


            Debug.Log("this should never come up if a reload was interupted by pickup!");

            isReloading = false;

            if (currentWeapon.GunType == PlayerScript.GunType.pistol)
            {
                currentAmmo = currentWeapon.clipSize;
            }
            else if (currentWeapon.GunType != PlayerScript.GunType.shotgun)
            {
                totalBulletsGunCanLoad -= shotsToReload;
                currentAmmo += shotsToReload;
            }

            if (totalBulletsGunCanLoad < 0)
                totalBulletsGunCanLoad = 0;
        }


        //do last
        SendGunText();

    }

    IEnumerator FireInBurst()
    {
        for (int i = 0; i < 3; i++)
        {
            //cone of -1 to 1 multiplied by current recoil amount to determine just how random it can be
            float recoilMod = UnityEngine.Random.Range(-1f, 1f) * currentRecoil;

            bulletSpawnPoint = new Vector3(bulletSpawnPoint.x, bulletSpawnPoint.y);

            currentRecoil += currentWeapon.recoilPerShot;

            SpawnBullet();

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

        //cone of -1 to 1 multiplied by current recoil amount to determine just how random it can be
        //float recoilMod = UnityEngine.Random.Range(-1f, 1f) * currentRecoil;

        bulletSpawnPoint = new Vector3(bulletSpawnPoint.x, bulletSpawnPoint.y);

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
            float angle = Vector2.SignedAngle(transform.position, aim);

            float cone = UnityEngine.Random.Range(-1f, 1f);
            cone += cone * 2;

            float offset = cone * angle;

            GameObject bulletGo = ObjectPooler.Instance.SpawnFromPool("Bullet", bulletSpawnPoint, Quaternion.identity);
            dir = bulletSpawn.transform.right * currentWeapon.bulletSpeed;
            Vector2 nomralizedOffset = new Vector2(dir.x + offset, dir.y + offset);

            bulletGo.GetComponent<Bullet>().Construct(basePlayer.playerID, currentWeapon.GunDamage, basePlayer, bulletSprite, currentWeapon.GunType, (nomralizedOffset));

        }
    }

    void SpawnBullet()
    {
        GameObject bulletGo = ObjectPooler.Instance.SpawnFromPool("Bullet", bulletSpawnPoint, Quaternion.identity);
        dir = bulletSpawn.transform.right * currentWeapon.bulletSpeed;

        bulletGo.GetComponent<Bullet>().Construct(basePlayer.playerID, currentWeapon.GunDamage, basePlayer, bulletSprite, currentWeapon.GunType, dir);

    }


    private void DrawHelperAtCenter(
                     Vector3 direction, Color color, float scale)
    {
        Gizmos.color = color;
        Vector3 destination = transform.position + direction * scale;
        Gizmos.DrawLine(bulletSpawn.transform.position, destination);
    }



}
