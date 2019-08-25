using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour, IPooledObject
{

    float damage;

    PlayerScript.GunType bulletType;
    PlayerScript player;

    bool canImapact;
    bool canBounce;

    Rigidbody2D rb;
    Vector2 startingForce;
    ObjectPooler objectPooler;
    SpriteRenderer sr;
    ParticleSystem somethingSexy;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        canBounce = true;
    }

    //interface override what to set when the object is spawned
    public void OnObjectSpawn()
    {
        canImapact = false;
        rb = GetComponent<Rigidbody2D>();
        objectPooler = ObjectPooler.Instance;
    }

    void SetStartingForce(Vector2 vel)
    {
        startingForce = new Vector2(vel.x, vel.y);
    }

    //TODO: we dont need to pass anywhere near all these variables. bleh
    public void Construct(float damage, PlayerScript player, Sprite bulletSprite, PlayerScript.GunType gunType, Vector3 dir, int collisionLayer)
    {
        //who shot the bullet
        this.player = player;
        //how much it will hurt
        this.damage = damage;
        //what gun shot it
        this.bulletType = gunType;
 
        sr.sprite = bulletSprite;

        //stuff to ignore (kind of legacy)
        foreach (var collider in player.GetComponentsInChildren<Collider2D>())
        {
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), collider, true);
        }

        rb.simulated = true;

        SetStartingForce(dir);

        canImapact = true;

        //in theory give a special trail to rpg shots from regular bullets. i ended up using rpg trail for both though, probably will change in future
        if (gunType != PlayerScript.GunType.RPG)
        {
            rb.AddRelativeForce(dir, ForceMode2D.Force);
            GameObject temp = objectPooler.SpawnFromPool("RocketTrail", gameObject.transform.position, Quaternion.identity);
            somethingSexy = temp.GetComponent<ParticleSystem>();
            somethingSexy.transform.parent = transform;
        }
        else
        {
            rb.AddForce(dir, ForceMode2D.Force);
            GameObject temp2 = objectPooler.SpawnFromPool("RocketTrail", gameObject.transform.position, Quaternion.identity);
            somethingSexy = temp2.GetComponent<ParticleSystem>();
            somethingSexy.transform.parent = transform;
        }

        //dont change rocket collision layer, we dont want it colliding with other bullets
        if (bulletType != PlayerScript.GunType.RPG)
            gameObject.layer = collisionLayer;

        sr.enabled = true;
    }

    const int ROCKET_TOP_SPEED = 150;
    const float ROCKET_ACCELERATION_MOD = 250f;

    private void FixedUpdate()
    {
        //accelerate the rocket over time if it exists
        if (rb != null)
        {
            if (rb.simulated == true && bulletType == PlayerScript.GunType.RPG && rb.velocity.magnitude < ROCKET_TOP_SPEED)
            {
                Vector2 dir = rb.velocity;
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

                rb.AddForce(dir * ROCKET_ACCELERATION_MOD * Time.deltaTime, ForceMode2D.Force);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.collider.gameObject.layer != LayerMask.NameToLayer("NonBulletCollide") && canImapact == true)
        {
            //we've hit something that isnt a bullet, or the player that shot us
            if (collision.collider.tag != "Bullet" || collision.collider.GetComponent<Bullet>().player.playerID != player.playerID)
            {
                //default damage type is nothing, we don't know what we hit yet.
                PlayerScript.DamageType dmgType = PlayerScript.DamageType.none;

                //checks where we hit the other guy, deals our given damage to that location. 
                if (collision.collider.tag == "Torso")
                {
                    dmgType = PlayerScript.DamageType.torso;
                    collision.transform.root.gameObject.GetComponent<PlayerScript>().TakeDamage(damage, dmgType, player, true, bulletType);
                    collision.transform.GetComponentInChildren<ParticleSystem>().Emit(30);
                    canBounce = false;
                    GetComponent<Collider2D>().enabled = false;
                }
                if (collision.collider.tag == "Head")
                {
                    dmgType = PlayerScript.DamageType.head;
                    collision.transform.root.gameObject.GetComponent<PlayerScript>().TakeDamage(damage, dmgType, player, true, bulletType);
                    canBounce = false;
                    GetComponent<Collider2D>().enabled = false;
                }
                if (collision.collider.tag == "Feet")
                {
                    dmgType = PlayerScript.DamageType.feet;
                    collision.transform.root.gameObject.GetComponent<PlayerScript>().TakeDamage(damage, dmgType, player, true, bulletType);
                    canBounce = false;
                    GetComponent<Collider2D>().enabled = false;
                }
                if (collision.collider.tag == "Leg")
                {
                    dmgType = PlayerScript.DamageType.legs;
                    collision.transform.root.gameObject.GetComponent<PlayerScript>().TakeDamage(damage, dmgType, player, true, bulletType);
                    canBounce = false;
                    GetComponent<Collider2D>().enabled = false;
                }

                canImapact = false;

                //spawn some impact sparks from pool
                GameObject sparkyObj = objectPooler.SpawnFromPool("BulletImpact", transform.position, Quaternion.identity);
                sparkyObj.GetComponent<ParticleSystem>().Emit(10);
                //start a timer to kill it
                sparkyObj.GetComponent<DisableOverTime>().DisableOverT(2f);

                //if you an rcoket who hit something, blow up
                if (bulletType == PlayerScript.GunType.RPG)
                {
                    ExplodeBullet();
                    return;
                }

                //only bounce if you are a railgun bullet that hasnt hit a player, and only do it once. 
                if ((bulletType != PlayerScript.GunType.railGun && bulletType != PlayerScript.GunType.RPG || canBounce == false))
                {
                    KillBullet();
                    return;
                }              
                else if (dmgType != PlayerScript.DamageType.none)
                {
                    KillBullet();
                    return;
                }
                else
                {
                    canBounce = false;
                }

                rb.AddForce(Reflect(startingForce, collision.GetContact(0).normal));
            }
        }
    }


    //gets rid of a bullet gracefully
    void KillBullet()
    {
        StartCoroutine(DisableOverTime(0.02f));

        somethingSexy.Stop();
        somethingSexy.GetComponent<DisableOverTime>().DisableOverT(3.1f);
        somethingSexy.transform.parent = null;
    }

    //explodes a bullet "gracefully"
    void ExplodeBullet()
    {
        gameObject.GetComponent<CircleCollider2D>().enabled = false;
        Explosion explosion = transform.Find("ExplosionRadius").GetComponent<Explosion>();
        explosion.gameObject.SetActive(true);
        explosion.Explode(player);
        rb.simulated = false;
        rb.isKinematic = true;

        sr.enabled = false;
        StartCoroutine(DisableOverTime(0.6f));

        somethingSexy.Stop();
        somethingSexy.GetComponent<DisableOverTime>().DisableOverT(3.1f);
        somethingSexy.transform.parent = null;

    }

    //reflect out vector to determine bounce for railgun :D
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
