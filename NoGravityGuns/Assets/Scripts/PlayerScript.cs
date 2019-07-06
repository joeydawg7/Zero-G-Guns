using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{

    Rigidbody2D rb;

    int shotPower = 5;

    float recoilTimer;

    const float RECOIL_DELAY = 0.2f;

    private void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        recoilTimer = 0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetAxisRaw("Shoot") > 0)
        {
            recoilTimer -= Time.deltaTime;

            if (recoilTimer <=0)
            {
                Vector2 shootDir = Vector2.right * Input.GetAxis("Horizontal2") + Vector2.up * Input.GetAxis("Vertical2");
                rb.AddForce(-shootDir, ForceMode2D.Impulse);
                recoilTimer = RECOIL_DELAY;
            }

        }
        else
            recoilTimer = 0;
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
