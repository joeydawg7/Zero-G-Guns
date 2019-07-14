using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour, IPooledObject
{

    float damage;

    int playerID;

    PlayerScript.GunType bulletType;

    bool canImapact;
    bool noBounce = true;

    Rigidbody2D rb;
    Vector2 startingForce;

    bool canHurty;

    ObjectPooler objectPooler;


    public void OnObjectSpawn()
    {
        canImapact = false;
        rb = GetComponent<Rigidbody2D>();
        objectPooler = ObjectPooler.Instance;

    }

    public void SetStartingForce(Vector2 vel)
    {
        startingForce = new Vector2(vel.x, vel.y);
    }


    ParticleSystem somethingSexy;

    public void Construct(int playerID, float damage, GameObject player, Sprite bulletSprite, PlayerScript.GunType gunType, Vector3 dir)
    {
        this.playerID = playerID;
        this.damage = damage;
        this.bulletType = gunType;

        GetComponent<SpriteRenderer>().sprite = bulletSprite;

        foreach (var collider in player.GetComponentsInChildren<Collider2D>())
        {
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), collider, true);
        }

        rb.simulated = true;

        rb.AddRelativeForce(dir, ForceMode2D.Force);
        SetStartingForce(dir);

        canImapact = true;
        canHurty = true;

        GameObject temp = objectPooler.SpawnFromPool("BulletTrail", gameObject.transform.position, Quaternion.identity);
        somethingSexy = temp.GetComponent<ParticleSystem>();
        somethingSexy.transform.parent = transform;

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.gameObject.layer != LayerMask.NameToLayer("NonBulletCollide") && canImapact == true)
        {
            if (collision.collider.tag != "Bullet" || collision.collider.GetComponent<Bullet>().playerID != playerID)
            {

                //checks where we hit the other guy, and that it isnt self damage so we cant shoot ourselves in the knees
                if (collision.collider.tag == "Torso" && collision.gameObject.GetComponent<PlayerScript>().playerID != playerID && canHurty)
                {
                    collision.gameObject.GetComponent<PlayerScript>().TakeDamage(damage, PlayerScript.DamageType.torso, playerID, true);
                }
                if (collision.collider.tag == "Head" && collision.gameObject.GetComponent<PlayerScript>().playerID != playerID && canHurty)
                {
                    collision.gameObject.GetComponent<PlayerScript>().TakeDamage(damage, PlayerScript.DamageType.head, playerID, true);
                }
                if (collision.collider.tag == "Feet" && collision.transform.root.GetComponent<PlayerScript>().playerID != playerID && canHurty)
                {
                    collision.transform.root.GetComponent<PlayerScript>().TakeDamage(damage, PlayerScript.DamageType.feet, playerID, true);
                }
                if (collision.collider.tag == "Leg" && collision.transform.root.GetComponent<PlayerScript>().playerID != playerID && canHurty)
                {
                    collision.transform.root.gameObject.GetComponent<PlayerScript>().TakeDamage(damage, PlayerScript.DamageType.legs, playerID, true);
                }

                GameObject sparkyObj = objectPooler.SpawnFromPool("BulletImpact", transform.position, Quaternion.identity);
                sparkyObj.GetComponent<ParticleSystem>().Emit(10);
                sparkyObj.GetComponent<DisableOverTime>().DisableOverT(2f);

                rb.AddForce(Reflect(startingForce, collision.GetContact(0).normal));


                if (bulletType != PlayerScript.GunType.railGun || noBounce == false)
                {
                    canHurty = false;
                    StartCoroutine(DisableOverTime(0.05f));
                    
                    somethingSexy.Stop();
                    somethingSexy.GetComponent<DisableOverTime>().DisableOverT(3.1f);
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

    IEnumerator DisableOverTime(float t)
    {
        yield return new WaitForSeconds(t);
        gameObject.SetActive(false);
        rb.simulated = false;
    }


}
