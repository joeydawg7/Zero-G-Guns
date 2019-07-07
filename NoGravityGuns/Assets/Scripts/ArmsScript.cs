using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    float currentRecoil;

    Quaternion facing;

    Vector2 shootDir;

    private void Awake()
    {
        timeSinceLastShot = 0;
        currentRecoil = 0;

        facing = transform.rotation;

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
    }

    void FixedUpdate()
    {
        if(GameManager.Instance.isGameStarted)
            ShootController();
      
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

            Vector3 forwardVector = Quaternion.Euler(shootDir) * Vector3.forward;

            var rotation = Quaternion.LookRotation(Vector3.forward, -shootDir.normalized);
            rotation *= facing;
            transform.rotation = rotation;

            if (Input.GetAxisRaw(triggerAXis) > 0)
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
        basePlayer.GetComponent<Rigidbody2D>().AddForce(-shootDir, ForceMode2D.Impulse);
        Camera.main.GetComponent<CameraShake>().shakeDuration = currentWeapon.cameraShakeDuration;
        timeSinceLastShot = 0;
    }

    void ShootyGunTemp()
    {
        //cone of -1 to 1 multiplied by current recoil amount to determine just how random it can be
        float recoilMod = Random.Range(-1f, 1f) * currentRecoil;

        bulletSpawnPoint = new Vector3(bulletSpawnPoint.x, bulletSpawnPoint.y );

        currentRecoil += currentWeapon.recoilPerShot;

        Rigidbody2D bullet = (Rigidbody2D)Instantiate(projectile, bulletSpawnPoint, Quaternion.identity);
        bullet.GetComponent<Bullet>().Construct(basePlayer.GetComponent<PlayerScript>().playerID, currentWeapon.GunDamage, basePlayer);
        bullet.transform.rotation = Quaternion.Euler(shootDir);
        bullet.AddForce(aim * currentWeapon.bulletSpeed, ForceMode2D.Impulse);

        
        GetComponent<AudioSource>().PlayOneShot(currentWeapon.GetRandomGunshotSFX);
    }

    IEnumerator FireInBurst()
    {
        for (int i = 0; i < 3; i++)
        {
            //cone of -1 to 1 multiplied by current recoil amount to determine just how random it can be
            float recoilMod = Random.Range(-1f, 1f) * currentRecoil;

            bulletSpawnPoint = new Vector3(bulletSpawnPoint.x, bulletSpawnPoint.y );

            currentRecoil += currentWeapon.recoilPerShot;

            Rigidbody2D bullet = (Rigidbody2D)Instantiate(projectile, bulletSpawnPoint, Quaternion.identity);
            bullet.GetComponent<Bullet>().Construct(basePlayer.GetComponent<PlayerScript>().playerID, currentWeapon.GunDamage, basePlayer);
            bullet.AddForce(aim * currentWeapon.bulletSpeed, ForceMode2D.Impulse);

           
            GetComponent<AudioSource>().PlayOneShot(currentWeapon.GetRandomGunshotSFX);
            yield return new WaitForSeconds(0.08f);
        }
    }

    public void BuckShot()
    {
        for (int i = 0; i < Random.Range(8, 13); i++)
        {
            //cone of -1 to 1 multiplied by current recoil amount to determine just how random it can be
            float recoilMod = Random.Range(-1f, 1f) * currentRecoil;

            bulletSpawnPoint = new Vector3(bulletSpawnPoint.x, bulletSpawnPoint.y );

            currentRecoil += currentWeapon.recoilPerShot;

            Rigidbody2D bullet = (Rigidbody2D)Instantiate(projectile, bulletSpawnPoint, Quaternion.identity);
            bullet.GetComponent<Bullet>().Construct(basePlayer.GetComponent<PlayerScript>().playerID, currentWeapon.GunDamage, basePlayer);
            bullet.AddForce(aim * currentWeapon.bulletSpeed, ForceMode2D.Impulse);

            
            GetComponent<AudioSource>().PlayOneShot(currentWeapon.GetRandomGunshotSFX);
        }
    }


}
