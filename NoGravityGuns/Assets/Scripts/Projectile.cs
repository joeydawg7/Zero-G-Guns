using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Rigidbody2D projectile;
    Vector3 bulletSpawn = new Vector3();
    Vector3 colliderBounds;
    Vector3 aim;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxisRaw("Shoot") > 0)
        {
            aim = new Vector3(Input.GetAxis("Horizontal2"), Input.GetAxis("Vertical2"), 0).normalized;
            //colliderBounds = GetComponent<Collider2D>().bounds.size/2;
            bulletSpawn.x = aim.x;
            bulletSpawn.y = aim.y;
            Debug.Log(bulletSpawn.y);
            Rigidbody2D bullet = (Rigidbody2D)Instantiate(projectile, bulletSpawn, Quaternion.identity);
            //bullet.AddForce(aim, ForceMode2D.Impulse);
        }
    }

    private void OnDrawGizmos()
    {
        //Camera.main.ScreenToWorldPoint(Input.mousePosition
        Gizmos.DrawLine(new Vector2(gameObject.transform.position.x, gameObject.transform.position.y), new Vector2(transform.position.x + Input.GetAxis("Horizontal2"), transform.position.y + Input.GetAxis("Vertical2")));
    }

}
