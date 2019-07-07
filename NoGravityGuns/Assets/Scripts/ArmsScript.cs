﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ArmsScript : MonoBehaviour
{

    public GameObject basePlayer;

    float timeSinceLastShot;

    public Rigidbody2D projectile;
    public Transform bulletSpawn;
    Vector3 bulletSpawnPoint;
    Vector3 aim;

    public string horizontalAxis;
    public string verticalAxis;
    public string triggerAXis;

    public GunSO currentWeapon;

    public int currentAmmo;
    public int currentClips;

    public Vector2 calibrationVector;


    float currentRecoil;

    Quaternion facing;
    Quaternion rotation;
    Vector2 shootDir;
    Transform startingTransform;

    public Sprite armColor;

    public TextMeshProUGUI reloadingText;

    public bool isReloading;

    private void Awake()
    {
        timeSinceLastShot = 0;
        currentRecoil = 0;

        facing = transform.rotation;
        currentClips = int.MaxValue;
        currentAmmo = currentWeapon.clipSize;
        reloadingText.alpha = 0;

        startingTransform = transform;

    }

    private void OnEnable()
    {

        shootDir = new Vector3(0, 0, 0);
        aim = shootDir;

        currentClips = currentWeapon.clipNum;
        currentAmmo = currentWeapon.clipSize;

        transform.rotation = startingTransform.rotation;
        transform.position = startingTransform.position;
        transform.localScale = startingTransform.localScale;
    }


    private void OnDrawGizmos()
    {
        /*
        Vector2 shootDir = Vector2.right * Input.GetAxis(horizontalAxis) + Vector2.up * Input.GetAxis(verticalAxis);
        Ray ray = new Ray();
        ray.origin = transform.position;
        ray.direction = shootDir;
        Gizmos.DrawRay(ray);
        */


        Ray ray = new Ray();
        ray.origin = transform.position;
        ray.direction = calibrationVector;
        Gizmos.DrawRay(ray);

    }

    void FixedUpdate()
    {
        if (GameManager.Instance.isGameStarted)
            ShootController();

    }


    private void OnDisable()
    {
        shootDir = new Vector3(0, 0, 0);
        aim = shootDir;
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

        if (!basePlayer.GetComponent<PlayerScript>().isDead)
        {
            shootDir = Vector2.right * Input.GetAxis(horizontalAxis) + Vector2.up * Input.GetAxis(verticalAxis);

            shootDir = shootDir.normalized;

            rotation = Quaternion.LookRotation(Vector3.forward, -shootDir);
            rotation *= facing;
            transform.rotation = rotation;

            if (Input.GetAxisRaw(triggerAXis) > 0 && !isReloading)
            {

                if (Input.GetAxis(horizontalAxis) != 0 || Input.GetAxis(verticalAxis) != 0)
                {
                    //aim = new Vector3(Input.GetAxis(horizontalAxis), Input.GetAxis(verticalAxis), 0).normalized;
                    aim = shootDir;
                }
                if (aim.magnitude != 0)
                {
                    if (timeSinceLastShot >= currentWeapon.recoilDelay)
                    {

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

                        //add force to player in opposite direction of shot
                        KnockBack(shootDir);

                    }
                }
            }
        }
    }

    void KnockBack(Vector2 shootDir)
    {
        basePlayer.GetComponent<Rigidbody2D>().AddForce(-shootDir * currentWeapon.knockback, ForceMode2D.Impulse);
        Camera.main.GetComponent<CameraShake>().shakeDuration += currentWeapon.cameraShakeDuration;
        timeSinceLastShot = 0;
    }

    void ShootyGunTemp()
    {
        //cone of -1 to 1 multiplied by current recoil amount to determine just how random it can be
        float recoilMod = Random.Range(-1f, 1f) * currentRecoil;

        bulletSpawnPoint = new Vector3(bulletSpawnPoint.x, bulletSpawnPoint.y);

        currentRecoil += currentWeapon.recoilPerShot;

        SpawnBullet();

        currentAmmo--;

        if (currentAmmo <= 0)
        {
            StartCoroutine(Reload());
        }

        GetComponent<AudioSource>().PlayOneShot(currentWeapon.GetRandomGunshotSFX);
    }

    void SpawnBullet()
    {
        Rigidbody2D bullet = (Rigidbody2D)Instantiate(projectile, bulletSpawnPoint, Quaternion.LookRotation(Vector3.forward, -shootDir));
        bullet.GetComponent<Bullet>().Construct(basePlayer.GetComponent<PlayerScript>().playerID, currentWeapon.GunDamage, basePlayer);
        bullet.AddForce(aim * currentWeapon.bulletSpeed, ForceMode2D.Impulse);
        bullet.transform.rotation = rotation;
    }


    IEnumerator Reload()
    {
        isReloading = true;
        reloadingText.alpha = 1;

        float reloadtimeIncrememnts = (float)currentWeapon.reloadTime / 6;

        for (int i = 0; i < currentWeapon.reloadTime; i++)
        {
            yield return new WaitForSeconds(reloadtimeIncrememnts);
            reloadingText.text = "Reloading.";
            yield return new WaitForSeconds(reloadtimeIncrememnts);
            reloadingText.text = "Reloading..";
            yield return new WaitForSeconds(reloadtimeIncrememnts);
            reloadingText.text = "Reloading...";
        }

        isReloading = false;
        currentClips--;
        currentAmmo = currentWeapon.clipSize;
        reloadingText.alpha = 0;

        if (currentClips <= 0)
            basePlayer.GetComponent<PlayerScript>().EquipArms(PlayerScript.GunType.pistol);

    }

    IEnumerator FireInBurst()
    {
        for (int i = 0; i < 3; i++)
        {
            //cone of -1 to 1 multiplied by current recoil amount to determine just how random it can be
            float recoilMod = Random.Range(-1f, 1f) * currentRecoil;

            bulletSpawnPoint = new Vector3(bulletSpawnPoint.x, bulletSpawnPoint.y);

            currentRecoil += currentWeapon.recoilPerShot;

            SpawnBullet();

            currentAmmo--;

            GetComponent<AudioSource>().PlayOneShot(currentWeapon.GetRandomGunshotSFX);


            yield return new WaitForSeconds(0.08f);
        }

        if (currentAmmo <= 0)
        {
            StartCoroutine(Reload());
        }

    }

    public void BuckShot()
    {
        currentAmmo--;

        //cone of -1 to 1 multiplied by current recoil amount to determine just how random it can be
        float recoilMod = Random.Range(-1f, 1f) * currentRecoil;

        bulletSpawnPoint = new Vector3(bulletSpawnPoint.x, bulletSpawnPoint.y);

        currentRecoil += currentWeapon.recoilPerShot;

        SpawnBuckShot();


        GetComponent<AudioSource>().PlayOneShot(currentWeapon.GetRandomGunshotSFX);


        if (currentAmmo <= 0)
        {
            StartCoroutine(Reload());
        }

    }

    void SpawnBuckShot()
    {
        for (int i = 0; i < Random.Range(5, 8); i++)
        {
            float angle = Vector2.SignedAngle(transform.position, aim);

            float offset = Random.Range(-0.0008f, 0.0008f) * angle;


            Vector2 nomralizedOffset = new Vector2(aim.x + offset, aim.y + offset).normalized;


            Rigidbody2D bullet = (Rigidbody2D)Instantiate(projectile, bulletSpawnPoint, Quaternion.LookRotation(Vector3.forward, -shootDir));
            bullet.GetComponent<Bullet>().Construct(basePlayer.GetComponent<PlayerScript>().playerID, currentWeapon.GunDamage, basePlayer);
            bullet.AddForce(nomralizedOffset * currentWeapon.bulletSpeed, ForceMode2D.Impulse);
            bullet.transform.rotation = rotation;
        }

    }



}
