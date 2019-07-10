using System.Collections;
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
    public string triggerAxis;
    public string XButton;


    public GunSO currentWeapon;

    public int currentAmmo;
    public int currentClips;

    public Vector2 calibrationVector;


    float currentRecoil;

    Quaternion facing;
    Quaternion rotation;
    Vector2 shootDir;

    AudioSource audio;

    public TextMeshProUGUI reloadingText;

    public bool isReloading;

    public bool canShoot;
    public Color32 invisible;
    Color32 startingColor;

    public Sprite bulletSprite;

    private void Awake()
    {
        timeSinceLastShot = 0;
        currentRecoil = 0;

        facing = transform.rotation;
        currentClips = int.MaxValue;
        currentAmmo = currentWeapon.clipSize;

        audio = GetComponent<AudioSource>();

        canShoot = true;

        shootDir = new Vector3(0, 0, 0);

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

    private void OnDrawGizmos()
    {
        Ray ray = new Ray();
        ray.origin = transform.position;
        ray.direction = calibrationVector;
        Gizmos.DrawRay(ray);

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
        currentClips = weaponToEquip.clipNum;
        currentAmmo = weaponToEquip.clipSize;
        isReloading = false;
        bulletSpawn = gunObj.transform.Find("BulletSpawner");
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
            // else
            // shootDir = startingEulers;

            shootDir = shootDir.normalized;

            rotation = Quaternion.LookRotation(Vector3.forward, -shootDir);
            rotation *= facing;
            transform.rotation = rotation;

            if (Input.GetAxisRaw(triggerAxis) > 0 && !isReloading)
            {

                if (Input.GetAxis(horizontalAxis) != 0 || Input.GetAxis(verticalAxis) != 0)
                {
                    //aim = new Vector3(Input.GetAxis(horizontalAxis), Input.GetAxis(verticalAxis), 0).normalized;
                    aim = shootDir;
                }
                if (aim.sqrMagnitude >= 0.1f)
                {
                    if (timeSinceLastShot >= currentWeapon.recoilDelay)
                    {
                        if (canShoot)
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

        audio.PlayOneShot(currentWeapon.GetRandomGunshotSFX);
    }

    void SpawnBullet()
    {
        Rigidbody2D bullet = (Rigidbody2D)Instantiate(projectile, bulletSpawnPoint, Quaternion.LookRotation(Vector3.forward, -shootDir));
        bullet.GetComponent<Bullet>().Construct(basePlayer.GetComponent<PlayerScript>().playerID, currentWeapon.GunDamage, basePlayer, bulletSprite);

        Vector3 dir = -Vector2.up * currentWeapon.bulletSpeed;
        bullet.AddRelativeForce(dir, ForceMode2D.Force);
        bullet.transform.rotation = rotation;
    }


    IEnumerator Reload()
    {
        currentClips--;

        if (currentClips < 0)
        {
            //if its our last clip no need to reload just drop the gun
            basePlayer.GetComponent<PlayerScript>().EquipArms(PlayerScript.GunType.pistol, GameManager.Instance.pistol);
            yield return null;
        }

        isReloading = true;
        reloadingText.alpha = 1;

        float reloadtimeIncrememnts = (float)currentWeapon.reloadTime / 6;

        //change sfx type base on reload type of the gun
        if (currentWeapon.GunType == PlayerScript.GunType.LMG || currentWeapon.GunType == PlayerScript.GunType.shotgun)
        {
            for (int i = 0; i < currentWeapon.reloadTime; i++)
            {
                yield return new WaitForSeconds(reloadtimeIncrememnts);
                GetComponent<AudioSource>().PlayOneShot(currentWeapon.reloadSound);
                reloadingText.text = "Reloading.";
                yield return new WaitForSeconds(reloadtimeIncrememnts);
                GetComponent<AudioSource>().PlayOneShot(currentWeapon.reloadSound);
                reloadingText.text = "Reloading..";
                yield return new WaitForSeconds(reloadtimeIncrememnts);
                GetComponent<AudioSource>().PlayOneShot(currentWeapon.reloadSound);
                reloadingText.text = "Reloading...";
            }
        }
        else
        {
            if (audio.isPlaying)
                audio.Stop();
            audio.PlayOneShot(currentWeapon.reloadSound);

            for (int i = 0; i < currentWeapon.reloadTime; i++)
            {
                yield return new WaitForSeconds(reloadtimeIncrememnts);
                reloadingText.text = "Reloading.";
                yield return new WaitForSeconds(reloadtimeIncrememnts);
                reloadingText.text = "Reloading..";
                yield return new WaitForSeconds(reloadtimeIncrememnts);
                reloadingText.text = "Reloading...";
            }
        }

        isReloading = false;

        currentAmmo = currentWeapon.clipSize;
        reloadingText.alpha = 0;



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

            if (audio.isPlaying)
                audio.Stop();

            audio.PlayOneShot(currentWeapon.GetRandomGunshotSFX);


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

        if (audio.isPlaying)
            audio.Stop();
        audio.PlayOneShot(currentWeapon.GetRandomGunshotSFX);


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

            float offset = Random.Range(-2.5f, 2.5f) * angle;

            Rigidbody2D bullet = (Rigidbody2D)Instantiate(projectile, bulletSpawnPoint, Quaternion.LookRotation(Vector3.forward, -shootDir));
            bullet.GetComponent<Bullet>().Construct(basePlayer.GetComponent<PlayerScript>().playerID, currentWeapon.GunDamage, basePlayer, bulletSprite);


            Vector3 dir = -Vector2.up * currentWeapon.bulletSpeed;
            bullet.transform.rotation = rotation;
            Vector2 nomralizedOffset = new Vector2(dir.x + offset, dir.y + offset).normalized;

            bullet.AddRelativeForce(nomralizedOffset * 20, ForceMode2D.Impulse);
            bullet.transform.rotation = rotation;
        }

    }



}
