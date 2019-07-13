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
    public TextMeshProUGUI reloadingText;
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

    }

    private void Start()
    {
        reloadingText.alpha = 0;
    }


    public void SetChildrenWithAxis(int playerID)
    {
        foreach (Transform child in transform)
        {
            if (child.tag == "Arms")
            {
                child.GetComponent<ArmsScript>().triggerAxis = "J" + playerID + "Trigger";
                child.GetComponent<ArmsScript>().horizontalAxis = "J" + playerID + "Horizontal";
                child.GetComponent<ArmsScript>().verticalAxis = "J" + playerID + "Vertical";

            }
        }

    }

    public void UnsetChildrenWithAxis(int playerID)
    {
        foreach (Transform child in transform)
        {
            if (child.tag == "Arms")
            {
                child.GetComponent<ArmsScript>().triggerAxis = "";
                child.GetComponent<ArmsScript>().horizontalAxis = "";
                child.GetComponent<ArmsScript>().verticalAxis = "";

            }
        }

    }


    private void Update()
    {
        if (GameManager.Instance.isGameStarted && Input.GetButton(XButton) && !isReloading && currentAmmo < currentWeapon.clipSize)
            StartCoroutine(Reload());

    }

    void FixedUpdate()
    {

        if (GameManager.Instance.isGameStarted)
            ShootController();



    }


    public void EquipGun(GunSO weaponToEquip, GameObject gunObj)
    {
        currentWeapon = weaponToEquip;
        currentAmmo = weaponToEquip.clipSize;
        totalBulletsGunCanLoad = weaponToEquip.numBullets;
        isReloading = false;
        bulletSpawn = gunObj.transform.Find("BulletSpawner");
        SendGunText();
    }


    void ShootController()
    {
        bulletSpawnPoint = bulletSpawn.position;

        timeSinceLastShot += Time.deltaTime;

        if (currentRecoil > 0)
        {
            currentRecoil -= Time.deltaTime;
        }

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
            if ((Input.GetAxisRaw(triggerAxis) > 0 && !isReloading) || ((Input.GetAxisRaw(triggerAxis) > 0 && currentWeapon.GunType == PlayerScript.GunType.shotgun && currentAmmo > 0)))
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
        Camera.main.GetComponent<CameraShake>().shakeDuration += currentWeapon.cameraShakeDuration;
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

        if (currentAmmo <= 0)
        {
            StartCoroutine(Reload());
        }

        audioS.PlayOneShot(currentWeapon.GetRandomGunshotSFX);
    }


    IEnumerator Rotate(float duration)
    {
        reloadTimer.SetActive(true);

        float startRotation = transform.eulerAngles.z;
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
            reloadingText.alpha = 1;
            //gunAndAmmo.text = "Reloading...";
            SendGunText("Reloading...");

            float reloadtimeIncrememnts = (float)currentWeapon.reloadTime / 6;

            //change sfx type base on reload type of the gun
            if (currentWeapon.GunType == PlayerScript.GunType.LMG)
            {
                StartCoroutine(Rotate(currentWeapon.reloadTime * reloadtimeIncrememnts));
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

                StartCoroutine(Rotate(reloadtimeIncrememnts * shotsToReload));

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

                        GetComponent<AudioSource>().PlayOneShot(currentWeapon.reloadSound);
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
                StartCoroutine(Rotate(currentWeapon.reloadTime));
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
            StartCoroutine(Reload());
        }

    }

    void BuckShot()
    {
        currentAmmo--;

        //cone of -1 to 1 multiplied by current recoil amount to determine just how random it can be
        float recoilMod = UnityEngine.Random.Range(-1f, 1f) * currentRecoil;

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
            cone += cone*2;

            float offset = cone * angle;

            GameObject bulletGo = ObjectPooler.Instance.SpawnFromPool("Bullet", bulletSpawnPoint, Quaternion.LookRotation(Vector3.forward, -shootDir));
            Vector3 dir = -Vector2.up * currentWeapon.bulletSpeed;
            Vector2 nomralizedOffset = new Vector2(dir.x + offset, dir.y + offset);

            bulletGo.GetComponent<Bullet>().Construct(basePlayer.playerID, currentWeapon.GunDamage, basePlayer.gameObject, bulletSprite, currentWeapon.GunType, (nomralizedOffset));

        }
    }

    void SpawnBullet()
    {
        GameObject bulletGo = ObjectPooler.Instance.SpawnFromPool("Bullet", bulletSpawnPoint, Quaternion.LookRotation(Vector3.forward, -shootDir));
        Vector3 dir = -Vector2.up * currentWeapon.bulletSpeed;

        bulletGo.GetComponent<Bullet>().Construct(basePlayer.playerID, currentWeapon.GunDamage, basePlayer.gameObject, bulletSprite, currentWeapon.GunType, dir);

    }



}
