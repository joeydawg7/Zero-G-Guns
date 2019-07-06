using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour
{


    public int health;

    Rigidbody2D rb;

    //public int shotPower = 5;

    float timeSinceLastShot;

    //const float RECOIL_DELAY_PISTOL = 0.2f;

    public Rigidbody2D projectile;
    Vector3 bulletSpawn = new Vector3();
    Vector3 aim;
    //public float bulletSpeed;
    //public AudioClip pistolShot;


    public string horizontalAxis;
    public string verticalAxis;
    public string shootAxis;

    public Image healthBar;

    public GunSO currentWeapon;

    float currentRecoil;


    private void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        timeSinceLastShot = 0;
        health = 100;
        float barVal = ((float)health / 100f);
        healthBar.fillAmount = barVal;
        currentRecoil = 0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        timeSinceLastShot += Time.deltaTime;

        if (currentRecoil > 0)
        {
            currentRecoil -= Time.deltaTime;
        }

        if (currentRecoil > currentWeapon.recoilMax)
            currentRecoil = currentWeapon.recoilMax;


        if (Input.GetAxisRaw(shootAxis) > 0)
        {

            if (Input.GetAxis("Horizontal2") != 0 || Input.GetAxis("Vertical2") != 0)
            {
                aim = new Vector3(Input.GetAxis(horizontalAxis), Input.GetAxis(verticalAxis), 0).normalized;
            }
            if (aim.magnitude != 0)
            {
                if (timeSinceLastShot >= currentWeapon.recoilDelay)
                {
                    bulletSpawn.x = transform.position.x + aim.x;
                    bulletSpawn.y = transform.position.y + aim.y;

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
                    Vector2 shootDir = Vector2.right * Input.GetAxis("Horizontal2") + Vector2.up * Input.GetAxis("Vertical2");
                    rb.AddForce(-shootDir, ForceMode2D.Impulse);
                    Camera.main.GetComponent<CameraShake>().shakeDuration = currentWeapon.cameraShakeDuration;
                    timeSinceLastShot = 0;

                    
                }
            }
        }


    }

    private void OnDrawGizmos()
    {
        Vector2 shootDir = Vector2.right * Input.GetAxis("Horizontal2") + Vector2.up * Input.GetAxis("Vertical2");
        Ray ray = new Ray();
        ray.origin = transform.position;
        ray.direction = shootDir;
        Gizmos.DrawRay(ray);
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        float barVal = ((float)health / 100f);

        healthBar.fillAmount = barVal;
    }

    void ShootyGunTemp()
    {
        //cone of -1 to 1 multiplied by current recoil amount to determine just how random it can be
        float recoilMod = Random.Range(-1f, 1f) * currentRecoil;

        bulletSpawn = new Vector3(bulletSpawn.x, bulletSpawn.y + recoilMod);

        currentRecoil += currentWeapon.recoilPerShot;

        Rigidbody2D bullet = (Rigidbody2D)Instantiate(projectile, bulletSpawn, Quaternion.identity);
        bullet.AddForce(aim * currentWeapon.bulletSpeed, ForceMode2D.Impulse);

        bullet.GetComponent<Bullet>().damage = currentWeapon.GunDamage();
        GetComponent<AudioSource>().PlayOneShot(currentWeapon.GetRandomGunshotSFX());
    }

    IEnumerator FireInBurst()
    {
        for (int i = 0; i < 3; i++)
        {
            //cone of -1 to 1 multiplied by current recoil amount to determine just how random it can be
            float recoilMod = Random.Range(-1f, 1f) * currentRecoil;

            bulletSpawn = new Vector3(bulletSpawn.x, bulletSpawn.y + recoilMod);

            currentRecoil += currentWeapon.recoilPerShot;

            Rigidbody2D bullet = (Rigidbody2D)Instantiate(projectile, bulletSpawn, Quaternion.identity);
            bullet.AddForce(aim * currentWeapon.bulletSpeed, ForceMode2D.Impulse);

            bullet.GetComponent<Bullet>().damage = currentWeapon.GunDamage();
            GetComponent<AudioSource>().PlayOneShot(currentWeapon.GetRandomGunshotSFX());
            yield return new WaitForSeconds(0.08f);
        }
    }

    public void BuckShot()
    {
        for (int i = 0; i < Random.Range(8, 13); i++)
        {
            //cone of -1 to 1 multiplied by current recoil amount to determine just how random it can be
            float recoilMod = Random.Range(-1f, 1f) * currentRecoil;

            bulletSpawn = new Vector3(bulletSpawn.x, bulletSpawn.y + recoilMod);

            currentRecoil += currentWeapon.recoilPerShot;

            Rigidbody2D bullet = (Rigidbody2D)Instantiate(projectile, bulletSpawn, Quaternion.identity);
            bullet.AddForce(aim * currentWeapon.bulletSpeed, ForceMode2D.Impulse);

            bullet.GetComponent<Bullet>().damage = currentWeapon.GunDamage();
            GetComponent<AudioSource>().PlayOneShot(currentWeapon.GetRandomGunshotSFX());
        }
    }

    //TODO: track mouse location in relation to center point of char
    //TODO: add negative force on click
    //TODO: raycast direction on click, check for hittables
    //TODO: deal dmg knockback, etc,
}
