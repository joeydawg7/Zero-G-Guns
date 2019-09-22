﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour, IPooledObject
{
    protected float damage;

    [HideInInspector]
    public PlayerScript player;

    protected bool canImapact;
    protected bool canBounce;

    protected Rigidbody2D rb;
    protected Vector2 startingForce;
    protected ObjectPooler objectPooler;
    protected SpriteRenderer sr;
    protected ParticleSystem somethingSexy;

    protected GunSO gun;

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
        rb.velocity = new Vector2(0, 0);
        rb.angularVelocity = 0;
        rb.simulated = false;
        objectPooler = ObjectPooler.Instance;
    }

    protected void SetStartingForce(Vector2 vel)
    {
        startingForce = new Vector2(vel.x, vel.y);
    }

    public virtual void Construct(float damage, PlayerScript player, Vector3 dir, Sprite sprite)
    {
        //who shot the bullet
        this.player = player;

        //how much it will hurt
        this.damage = damage;
        //what gun shot it
        this.gun = player.armsScript.currentWeapon;
        //this.bulletType = player.armsScript.currentWeapon.GunType;

        int collisionLayer = player.collisionLayer;

        sr.sprite = sprite;

        //stuff to ignore (kind of legacy)
        foreach (var collider in player.GetComponentsInChildren<Collider2D>())
        {
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), collider, true);
        }

        rb.simulated = true;
        rb.isKinematic = false;
        GetComponent<CircleCollider2D>().enabled = true;

        SetStartingForce(dir);

        canImapact = true;

        rb.AddForce(dir, ForceMode2D.Force);
        GameObject temp2 = objectPooler.SpawnFromPool("RocketTrail", gameObject.transform.position, Quaternion.identity);
        somethingSexy = temp2.GetComponent<ParticleSystem>();
        somethingSexy.transform.parent = transform;

    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.collider.gameObject.layer != LayerMask.NameToLayer("NonBulletCollide") && canImapact == true)
        {
            //we've hit something that isnt a bullet, or the player that shot us
            if (collision.collider.tag != "Bullet" || collision.collider.GetComponent<Bullet>().player.playerID != player.playerID)
            {
                //default damage type is nothing, we don't know what we hit yet.
                PlayerScript.DamageType dmgType = DamageBodyParts(collision);
                SpawnSparkEffect();

                //only bounce if you are a railgun bullet that hasnt hit a player, and only do it once. 
                if ((gun.name != "RailGun" || canBounce == false))
                {
                    canImapact = false;
                    KillBullet();
                    return;
                }
                else if (dmgType != PlayerScript.DamageType.none)
                {
                    canImapact = false;
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

    protected virtual void SpawnSparkEffect()
    {
        //spawn some impact sparks from pool
        GameObject sparkyObj = objectPooler.SpawnFromPool("BulletImpact", transform.position, Quaternion.identity);
        sparkyObj.GetComponent<ParticleSystem>().Emit(10);
        //start a timer to kill it
        sparkyObj.GetComponent<DisableOverTime>().DisableOverT(2f);
    }

    protected virtual PlayerScript.DamageType DamageBodyParts(Collision2D collision)
    {
        //default damage type is nothing, we don't know what we hit yet.
        PlayerScript.DamageType dmgType = PlayerScript.DamageType.none;

        //checks where we hit the other guy, deals our given damage to that location. 
        if (collision.collider.tag == "Torso")
        {
            dmgType = PlayerScript.DamageType.torso;
            collision.transform.root.gameObject.GetComponent<PlayerScript>().TakeDamage(damage, dmgType, player, true);
            collision.transform.GetComponentInChildren<ParticleSystem>().Emit(30);
            canBounce = false;
            GetComponent<Collider2D>().enabled = false;
        }
        if (collision.collider.tag == "Head")
        {
            dmgType = PlayerScript.DamageType.head;
            collision.transform.root.gameObject.GetComponent<PlayerScript>().TakeDamage(damage, dmgType, player, true);
            canBounce = false;
            GetComponent<Collider2D>().enabled = false;
        }
        if (collision.collider.tag == "Feet")
        {
            dmgType = PlayerScript.DamageType.feet;
            collision.transform.root.gameObject.GetComponent<PlayerScript>().TakeDamage(damage, dmgType, player, true);
            canBounce = false;
            GetComponent<Collider2D>().enabled = false;
        }
        if (collision.collider.tag == "Leg")
        {
            dmgType = PlayerScript.DamageType.legs;
            collision.transform.root.gameObject.GetComponent<PlayerScript>().TakeDamage(damage, dmgType, player, true);
            canBounce = false;
            GetComponent<Collider2D>().enabled = false;
        }

        return dmgType;
    }

    //gets rid of a bullet gracefully
   protected void KillBullet()
    {
        StartCoroutine(DisableOverTime(0.02f));

        somethingSexy.Stop();
        somethingSexy.GetComponent<DisableOverTime>().DisableOverT(3.1f);
        somethingSexy.transform.parent = null;
    }

    //reflect out vector to determine bounce for railgun :D
    Vector2 Reflect(Vector2 vector, Vector2 normal)
    {
        return vector - 2 * Vector2.Dot(vector, normal) * normal;
    }

    protected IEnumerator DisableOverTime(float t)
    {
        yield return new WaitForSeconds(t);
        gameObject.SetActive(false);
        rb.velocity = new Vector2(0, 0);
        rb.angularVelocity = 0;
        rb.simulated = false;

    }


}
