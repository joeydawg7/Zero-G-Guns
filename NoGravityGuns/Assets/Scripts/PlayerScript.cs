using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{

    Rigidbody2D rb;

    int shotPower = 5;

    private void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(Input.GetAxisRaw("Shoot") >0)
        {
            Vector2 shootTarget = new Vector2(transform.position.x + Input.GetAxis("Horizontal2"), transform.position.y + Input.GetAxis("Vertical2"));

           rb.AddForce(5 * shootTarget , ForceMode2D.Impulse);
        }
    }

    private void OnDrawGizmos()
    {
        //Camera.main.ScreenToWorldPoint(Input.mousePosition
        Gizmos.DrawLine(new Vector2(gameObject.transform.position.x, gameObject.transform.position.y), new Vector2(transform.position.x + Input.GetAxis("Horizontal2"), transform.position.y + Input.GetAxis("Vertical2")));
    }

    //TODO: track mouse location in relation to center point of char
    //TODO: add negative force on click
    //TODO: raycast direction on click, check for hittables
    //TODO: deal dmg knockback, etc,
}
