﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DOT_Bullet : MonoBehaviour, IPooledObject
{
    public float duration;
    public float frequency; 

    [HideInInspector]
    public float damage;
    

    [HideInInspector]
    public PlayerScript player;

    protected bool canImapact;


    protected Rigidbody2D rb;
    protected Vector2 startingForce;
    protected ObjectPooler objectPooler;
    protected SpriteRenderer sr;
    protected ParticleSystem bulletTrail;

    protected Guns gun;

    protected string layer;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
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

    public virtual void Construct(float damage, PlayerScript player, Vector3 dir, Color32 color)
    {
        //who shot the bullet
        this.player = player;

        //how much it will hurt
        this.damage = damage;
        //what gun shot it
        this.gun = player.armsScript.currentWeapon;

        gameObject.layer = player.collisionLayer;

        //sr.sprite = sprite;
        sr.color = color;

        //stuff to ignore (kind of legacy)
        foreach (var collider in player.GetComponentsInChildren<Collider2D>())
        {
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), collider, true);
        }

        rb.simulated = true;
        rb.isKinematic = false;
        GetComponent<Collider2D>().enabled = true;

        SetStartingForce(dir);

        canImapact = true;

        rb.AddForce(dir, ForceMode2D.Force);

        SetPFXTrail("RocketTrail", false);

        //Vector2 directionOfMovement = rb.velocity;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

    }

    protected virtual void SetPFXTrail(string effectTag, bool setToPlayerColor)
    {
        //replace bullet trail if one exists
        if (bulletTrail != null)
        {
            bulletTrail.Stop();
            bulletTrail.GetComponent<DisableOverTime>().DisableOverT(0.0f);
            bulletTrail.transform.parent = null;
        }

        GameObject temp2 = objectPooler.SpawnFromPool(effectTag, gameObject.transform.position, Quaternion.identity);
        bulletTrail = temp2.GetComponent<ParticleSystem>();

        var main = bulletTrail.main;
        main.startColor = new ParticleSystem.MinMaxGradient(player.playerColor);

        bulletTrail.transform.parent = transform;
        bulletTrail.Play(true);

    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.collider.gameObject.layer != LayerMask.NameToLayer("NonBulletCollide") && canImapact == true)
        {
            //we've hit something that isnt a bullet
            if (collision.collider.tag != "Bullet")
            {

                //default damage type is nothing, we don't know what we hit yet.
                PlayerScript.DamageType dmgType = DamageBodyParts(collision);
                SpawnSparkEffect();

                canImapact = false;
                KillBullet();
            }
        }
    }

    protected virtual void SpawnSparkEffect()
    {
        //spawn some impact sparks from pool
        GameObject sparkyObj = objectPooler.SpawnFromPool("BulletImpact", transform.position, Quaternion.identity);


        ParticleSystem ps = sparkyObj.GetComponent<ParticleSystem>();
        ps.Emit(10);

        var main = ps.main;
        main.startColor = new ParticleSystem.MinMaxGradient(player.playerColor);

        //start a timer to kill it
        sparkyObj.GetComponent<DisableOverTime>().DisableOverT(2f);
    }

    protected virtual PlayerScript.DamageType DamageBodyParts(Collision2D collision)
    {
        //default damage type is nothing, we don't know what we hit yet.
        PlayerScript.DamageType dmgType = PlayerScript.DamageType.self;

        //we can get out of here early if there is no player script component on the root parent of whatever we hit, because that 100% is not a player :D
        PlayerScript hitPlayerScript = collision.transform.root.GetComponentInChildren<PlayerScript>();
        if (hitPlayerScript == null)
            return dmgType;

        //checks where we hit the other guy, deals our given damage to that location. 

        //dmgType = PlayerScript.ParsePlayerDamage(collision.gameObject);
        dmgType = PlayerScript.DamageType.torso;
        if (dmgType != PlayerScript.DamageType.self)
        {
            //hitPlayerScript.TakeDamage(damage, startingForce, dmgType, this.player, true, gun);

            hitPlayerScript.DamageOverTime
                (duration, frequency, Random.Range(damage-2.0f,damage+2.0f), startingForce, dmgType, this.player, true, gun);
            // collision.transform.GetComponentInChildren<ParticleSystem>().Emit(30);
            GetComponent<Collider2D>().enabled = false;
        }

        return dmgType;
    }

    //gets rid of a bullet gracefully
    protected virtual void KillBullet()
    {
        StartCoroutine(DisableOverTime(0.02f));

        bulletTrail.transform.parent = null;
        bulletTrail.Stop();

        bulletTrail.GetComponent<DisableOverTime>().DisableOverT(3.1f);

    }

    protected virtual IEnumerator DisableOverTime(float t)
    {
        yield return new WaitForSeconds(t);
        gameObject.SetActive(false);
        rb.velocity = new Vector2(0, 0);
        rb.angularVelocity = 0;
        rb.simulated = false;

    }


}