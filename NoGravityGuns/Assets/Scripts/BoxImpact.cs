using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class BoxImpact : MonoBehaviour
{

    Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        //if (collision.collider.tag == "Torso" || collision.collider.tag == "Head" || collision.collider.tag == "Legs")
        //{
        //    //Debug.Log(rb.velocity);

        //    float dmg = Mathf.Abs(rb.velocity.x + rb.velocity.y);

        //    if (dmg > 5)
        //    {
        //        collision.gameObject.GetComponent<PlayerScript>().TakeDamage(dmg, PlayerScript.DamageType.torso, 0);
        //    }

        //}
    }
}
