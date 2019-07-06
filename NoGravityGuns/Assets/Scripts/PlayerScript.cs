using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{

    Rigidbody2D rb;

    int shotPower = 5;

    float timeSinceLastShot;

    const float RECOIL_DELAY = 0.2f;

    public Rigidbody2D projectile;
    Vector3 bulletSpawn = new Vector3();
    Vector3 aim;
    public float bulletSpeed;
    public AudioClip pistolShot;

    private void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        timeSinceLastShot = 0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        timeSinceLastShot += Time.deltaTime;

        if (Input.GetAxisRaw("Shoot") > 0)
        {
            
            

            if (Input.GetAxis("Horizontal2") != 0 || Input.GetAxis("Vertical2") != 0)
            {
                aim = new Vector3(Input.GetAxis("Horizontal2"), Input.GetAxis("Vertical2"), 0).normalized;
            }
            if (aim.magnitude != 0)
            {
                if (timeSinceLastShot >= RECOIL_DELAY)
                {
                    bulletSpawn.x = transform.position.x + aim.x;
                    bulletSpawn.y = transform.position.y + aim.y;

                    Rigidbody2D bullet = (Rigidbody2D)Instantiate(projectile, bulletSpawn, Quaternion.identity);
                    bullet.AddForce(aim * bulletSpeed, ForceMode2D.Impulse);
                    bullet.GetComponent<Bullet>().playerID = 1;
                    Vector2 shootDir = Vector2.right * Input.GetAxis("Horizontal2") + Vector2.up * Input.GetAxis("Vertical2");
                    rb.AddForce(-shootDir, ForceMode2D.Impulse);
                    Camera.main.GetComponent<CameraShake>().shakeDuration =0.1f;
                    timeSinceLastShot = 0;

                    GetComponent<AudioSource>().PlayOneShot(pistolShot);
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


    //TODO: track mouse location in relation to center point of char
    //TODO: add negative force on click
    //TODO: raycast direction on click, check for hittables
    //TODO: deal dmg knockback, etc,
}
