using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    float damage;
    public GameObject sparkyBoom;



    int playerID;


    bool canImapact;


    private void Awake()
    {
        canImapact = false;
    }

    public void Construct(int playerID, float damage, GameObject player)
    {
        this.playerID = playerID;
        this.damage = damage;

        foreach (var collider in player.GetComponentsInChildren<Collider2D>())
        {
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), collider, true);
        }

        canImapact = true;
    }


    private void Update()
    {
        //transform.rotation = Quaternion.LookRotation(GetComponent<Rigidbody2D>().velocity * Vector3.forward);
        //transform.rotation = Quaternion.

        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.gameObject.layer != LayerMask.NameToLayer("NonBulletCollide") && canImapact == true)
        {
            if (collision.collider.tag != "Bullet" || collision.collider.GetComponent<Bullet>().playerID != playerID)
            {

                //TODO: prevent self damage, maybe make bullets bouncing off walls not hurt so bad

                //checks where we hit the other guy, and that it isnt self damage so we cant shoot ourselves in the knees
                if (collision.collider.tag == "Torso" && collision.gameObject.GetComponent<PlayerScript>().playerID != playerID )
                {
                    collision.gameObject.GetComponent<PlayerScript>().TakeDamage(damage, PlayerScript.DamageType.torso);
                }
                if (collision.collider.tag == "Head" && collision.gameObject.GetComponent<PlayerScript>().playerID != playerID)
                {
                    collision.gameObject.GetComponent<PlayerScript>().TakeDamage(damage, PlayerScript.DamageType.head);
                }
                if (collision.collider.tag == "Feet" && collision.gameObject.GetComponent<PlayerScript>().playerID != playerID)
                {
                    collision.gameObject.GetComponent<PlayerScript>().TakeDamage(damage, PlayerScript.DamageType.feet);
                }
                if (collision.collider.tag == "Leg" && collision.gameObject.GetComponent<PlayerScript>().playerID != playerID)
                {
                    collision.gameObject.GetComponent<PlayerScript>().TakeDamage(damage, PlayerScript.DamageType.legs);
                }

                GameObject sparkyObj = GameObject.Instantiate(sparkyBoom);

                sparkyObj.transform.position = transform.position;
                sparkyObj.GetComponent<ParticleSystem>().Emit(100);
                Destroy(sparkyObj, 0.2f);


                Destroy(gameObject, 0.16f);
            }


        }
    }

}
