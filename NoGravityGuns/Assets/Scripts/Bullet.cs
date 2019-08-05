using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour, IPooledObject
{

    float damage;

    int playerID;

    public PlayerScript.GunType bulletType;

    bool canImapact;
    bool canBounce = true;

    Rigidbody2D rb;
    Vector2 startingForce;

    bool canHurty;

    ObjectPooler objectPooler;

    public PlayerScript player;

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

    public void Construct(int playerID, float damage, PlayerScript player, Sprite bulletSprite, PlayerScript.GunType gunType, Vector3 dir, int collisionLayer)
    {
        this.playerID = playerID;
        this.damage = damage;
        this.bulletType = gunType;
        this.player = player;

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

        if (bulletType != PlayerScript.GunType.RPG)
            gameObject.layer = collisionLayer;

    }


    private void FixedUpdate()
    {
        if (rb != null)
        {
           
            if (rb.simulated == true && bulletType == PlayerScript.GunType.RPG && rb.velocity.magnitude < 150)
            {
                Vector2 dir = rb.velocity;
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

                //trench foot 250 babeeee
                rb.AddForce(dir * 250 * Time.deltaTime, ForceMode2D.Force);

            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.collider.gameObject.layer != LayerMask.NameToLayer("NonBulletCollide") && canImapact == true)
        {

            if (collision.collider.tag != "Bullet" || collision.collider.GetComponent<Bullet>().playerID != playerID)
            {

                PlayerScript.DamageType dmgType = PlayerScript.DamageType.none;


                //checks where we hit the other guy, and that it isnt self damage so we cant shoot ourselves in the knees
                if (collision.collider.tag == "Torso")
                {
                    dmgType = PlayerScript.DamageType.torso;
                    collision.transform.root.gameObject.GetComponent<PlayerScript>().TakeDamage(damage, dmgType, player, true, bulletType);
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



                GameObject sparkyObj = objectPooler.SpawnFromPool("BulletImpact", transform.position, Quaternion.identity);
                sparkyObj.GetComponent<ParticleSystem>().Emit(10);
                sparkyObj.GetComponent<DisableOverTime>().DisableOverT(2f);


                //only bounce if you are a railgun bullet that hasnt hit a player, and only do it once. 
                if ((bulletType != PlayerScript.GunType.railGun && bulletType != PlayerScript.GunType.RPG || canBounce == false))
                {
                    KillBullet();
                }
                else if (bulletType == PlayerScript.GunType.RPG)
                {
                    ExplodeBullet();
                }
                else if (dmgType != PlayerScript.DamageType.none)
                {
                    KillBullet();
                }
                else
                {
                    canBounce = false;
                }

                rb.AddForce(Reflect(startingForce, collision.GetContact(0).normal));
            }
        }
    }

    void KillBullet()
    {
        canHurty = false;
        StartCoroutine(DisableOverTime(0.02f));

        somethingSexy.Stop();
        somethingSexy.GetComponent<DisableOverTime>().DisableOverT(3.1f);
        somethingSexy.transform.parent = null;
    }

    void ExplodeBullet()
    {
        canHurty = false;
        gameObject.GetComponent<CircleCollider2D>().enabled = false;
        Explosion explosion = transform.Find("ExplosionRadius").GetComponent<Explosion>();
        explosion.gameObject.SetActive(true);
        explosion.Explode(player);
        rb.simulated = false;
        rb.isKinematic = true;

        StartCoroutine(DisableOverTime(0.3f));

        somethingSexy.Stop();
        somethingSexy.GetComponent<DisableOverTime>().DisableOverT(3.1f);
        somethingSexy.transform.parent = null;

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
