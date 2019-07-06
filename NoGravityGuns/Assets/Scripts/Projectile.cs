using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetAxisRaw("Shoot") > 0)
        {
        }
    }

    private void OnDrawGizmos()
    {
        //Camera.main.ScreenToWorldPoint(Input.mousePosition
        Gizmos.DrawLine(new Vector2(gameObject.transform.position.x, gameObject.transform.position.y), new Vector2(transform.position.x + Input.GetAxis("Horizontal2"), transform.position.y + Input.GetAxis("Vertical2")));
    }

}
