using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    float damage;
    public GameObject sparkyBoom;

    const float HEADSHOT_MULTIPLIER =2f;
    const float TORSOSHOT_MULTIPLIER = 1f;
    const float FOOTSHOT_MULTIPLIER = 0.5f;
    const float LEGSHOT_MULTIPLIER = 0.75f;


    int playerID;


    bool canImapact;

    private void Awake()
    {
        canImapact = false;
        //GetComponent<Collider>().isTrigger = true;

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
                    collision.gameObject.GetComponent<PlayerScript>().TakeDamage(damage*TORSOSHOT_MULTIPLIER);
                }
                if (collision.collider.tag == "Head" && collision.gameObject.GetComponent<PlayerScript>().playerID != playerID)
                {
                    collision.gameObject.GetComponent<PlayerScript>().TakeDamage(damage*HEADSHOT_MULTIPLIER);
                }
                if (collision.collider.tag == "Feet" && collision.gameObject.GetComponent<PlayerScript>().playerID != playerID)
                {
                    collision.gameObject.GetComponent<PlayerScript>().TakeDamage(damage * FOOTSHOT_MULTIPLIER);
                }
                if (collision.collider.tag == "Leg" && collision.gameObject.GetComponent<PlayerScript>().playerID != playerID)
                {
                    collision.gameObject.GetComponent<PlayerScript>().TakeDamage(damage * LEGSHOT_MULTIPLIER);
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
