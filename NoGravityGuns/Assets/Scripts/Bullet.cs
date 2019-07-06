using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    public int playerID { get; set; }
    public int damage;
    public GameObject sparkyBoom;
    public bool inert;

    private void Awake()
    {
        inert = false; 
    }

    // Update is called once per frame
    void Update()
    {

    }



    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.gameObject.layer != LayerMask.NameToLayer("NonBulletCollide"))
        {
            if (collision.collider.tag != "Bullet" || collision.collider.GetComponent<Bullet>().playerID != playerID)
            {

                //TODO: prevent self damage, maybe make bullets bouncing off walls not hurt so bad
                if (collision.collider.tag == "Player")
                {
                    if (inert == false)
                        collision.gameObject.GetComponent<PlayerScript>().TakeDamage(damage);
                }
                else
                    inert = true;

                GameObject sparkyObj = GameObject.Instantiate(sparkyBoom);

                sparkyObj.transform.position = transform.position;
                sparkyObj.GetComponent<ParticleSystem>().Emit(100);
                Destroy(sparkyObj, 0.2f);


                Destroy(gameObject, 0.16f);
            }


        }
    }

}
