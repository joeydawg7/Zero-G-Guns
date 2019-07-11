using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    float damage;
    public GameObject sparkyBoom;

    int playerID;

    PlayerScript.GunType bulletType;

    bool canImapact;
    bool noBounce = true;

    public GameObject somethingSexy;

    Rigidbody2D rb;
    Vector2 startingForce;

    private void Awake()
    {
        canImapact = false;
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        //kill the gameobject after 20 seconds in case it makes it this far without hitting a wall
        Destroy(gameObject, 20f);
    }

    public void SetStartingForce(Vector2 vel)
    {
        startingForce = new Vector2 (vel.x, vel.y);
    }


    public void Construct(int playerID, float damage, GameObject player, Sprite bulletSprite, PlayerScript.GunType gunType)
    {
        this.playerID = playerID;
        this.damage = damage;
        this.bulletType = gunType;

        GetComponent<SpriteRenderer>().sprite = bulletSprite;

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

                //checks where we hit the other guy, and that it isnt self damage so we cant shoot ourselves in the knees
                if (collision.collider.tag == "Torso" && collision.gameObject.GetComponent<PlayerScript>().playerID != playerID )
                {
                    collision.gameObject.GetComponent<PlayerScript>().TakeDamage(damage, PlayerScript.DamageType.torso, playerID);
                }
                if (collision.collider.tag == "Head" && collision.gameObject.GetComponent<PlayerScript>().playerID != playerID)
                {
                    collision.gameObject.GetComponent<PlayerScript>().TakeDamage(damage, PlayerScript.DamageType.head, playerID);
                }
                if (collision.collider.tag == "Feet" && collision.gameObject.GetComponent<PlayerScript>().playerID != playerID)
                {
                    collision.gameObject.GetComponent<PlayerScript>().TakeDamage(damage, PlayerScript.DamageType.feet, playerID);
                }
                if (collision.collider.tag == "Leg" && collision.gameObject.GetComponent<PlayerScript>().playerID != playerID)
                {
                    collision.gameObject.GetComponent<PlayerScript>().TakeDamage(damage, PlayerScript.DamageType.legs , playerID);
                }

                GameObject sparkyObj = GameObject.Instantiate(sparkyBoom);

                sparkyObj.transform.position = transform.position;
                sparkyObj.GetComponent<ParticleSystem>().Emit(10);


                Destroy(sparkyObj, 2f);


                rb.AddForce(Reflect(startingForce, collision.contacts[0].normal));


                if (bulletType != PlayerScript.GunType.railGun || noBounce == false)
                {
                    Destroy(gameObject, 0.16f);
                    somethingSexy.GetComponent<ParticleSystem>().Stop();
                    somethingSexy.GetComponent<KillOverTime>().StartKilling();
                    somethingSexy.transform.parent = null;
                }
                else
                {
                    noBounce = false;
                }
            }
        }
    }


     Vector2 Reflect(Vector2 vector, Vector2 normal)
    {
        return vector - 2 * Vector2.Dot(vector, normal) * normal;
    }


}
