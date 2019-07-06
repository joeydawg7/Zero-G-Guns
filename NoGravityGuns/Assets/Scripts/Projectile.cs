using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Rigidbody2D projectile;
    Vector3 bulletSpawn = new Vector3();
    Vector3 aim;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetAxisRaw("Shoot") > 0)
        {
            if (Input.GetAxis("Horizontal2") != 0 || Input.GetAxis("Vertical2") != 0)
            {
                aim = new Vector3(Input.GetAxis("Horizontal2"), Input.GetAxis("Vertical2"), 0).normalized;
            }
            if (aim.magnitude != 0)
            {
                bulletSpawn.x = transform.position.x + aim.x;
                bulletSpawn.y = transform.position.y + aim.y;
                Debug.Log(bulletSpawn.y);
                Rigidbody2D bullet = (Rigidbody2D)Instantiate(projectile, bulletSpawn, Quaternion.identity);
                bullet.AddForce(aim * 10, ForceMode2D.Impulse);
            }
        }
    }

    private void OnDrawGizmos()
    {
        //Camera.main.ScreenToWorldPoint(Input.mousePosition
        Gizmos.DrawLine(new Vector2(gameObject.transform.position.x, gameObject.transform.position.y), new Vector2(transform.position.x + Input.GetAxis("Horizontal2"), transform.position.y + Input.GetAxis("Vertical2")));
    }

}
