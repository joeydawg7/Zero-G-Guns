﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceShot : Bullet
{
    public int maxBounces = 2;
    int bounces;
    public AudioClip bounceEffect;
    AudioSource audioSource;
    Vector2 dir;

    public override void Construct(float damage, PlayerScript player, Vector3 dir, Color32 color)
    {
        base.Construct(damage, player, dir, color);

        bounces = 0;

        this.dir = dir;
        SetPFXTrail("ElectricTrail", true);
        audioSource = GetComponent<AudioSource>();
    }



    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.gameObject.layer != LayerMask.NameToLayer("NonBulletCollide") && canImapact == true)
        {
            //we've hit something that isnt a bullet, or the player that shot the original bullet
            if (collision.collider.tag != "Bullet" || collision.collider.GetComponent<Bullet>().player.playerID != player.playerID)
            {

                ExplosiveObjectScript explosiveObjectScript = collision.collider.gameObject.GetComponent<ExplosiveObjectScript>();

                if (explosiveObjectScript != null)
                {
                    if (explosiveObjectScript != null && damage > 0)
                    {
                        explosiveObjectScript.DamageExplosiveObject(damage, player, collision.transform.position);
                    }
                }

                //default damage type is nothing, we don't know what we hit yet.
                PlayerScript.DamageType dmgType = DamageBodyParts(collision);

                //hit a player so stop bouncing
                if (dmgType != PlayerScript.DamageType.self)
                {
                    bounces = int.MaxValue;
                    canImapact = false;
                }

                SpawnSparkEffect();
            }
        }

        //only bounce if you are a railgun bullet that hasnt hit a player, and only do it once. 
        if (bounces >=maxBounces)
        {
            canImapact = false;
            KillBullet();
            return;
        }       
        //else if (dmgType != PlayerScript.DamageType.none)
        //{
        //    canImapact = false;
        //    KillBullet();
        //    return;
        //}
        else
        {
            bounces++;
        }

        SoundPooler.Instance.PlaySoundEffect(bounceEffect);
        //Vector2 v =  Vector2.Reflect(rb.velocity, collision.GetContact(0).normal);
        // float rot = 90 - Mathf.Atan2(v.z, v.x) * Mathf.Rad2Deg;
        //  transform.eulerAngles = new Vector3(0, 0, rot);
        //  rb.AddForce(v, ForceMode2D.Impulse);

    }

    protected override PlayerScript.DamageType DamageBodyParts(Collision2D collision)
    {
        //default damage type is nothing, we don't know what we hit yet.
        PlayerScript.DamageType dmgType = PlayerScript.DamageType.self;


        //we can get out of here early if there is no player script component on the root parent of whatever we hit, because that 100% is not a player :D
        PlayerScript hitPlayerScript = collision.transform.root.GetComponentInChildren<PlayerScript>();
        if (hitPlayerScript == null)
            return dmgType;

        //figures out where we hit the guy
        dmgType = PlayerScript.ParsePlayerDamage(collision.gameObject);

        if(dmgType != PlayerScript.DamageType.self)
        {
            hitPlayerScript.TakeDamage(damage, dir, dmgType, player, true, gun);
            //collision.transform.GetComponentInChildren<ParticleSystem>().Emit(30);
            GetComponent<Collider2D>().enabled = false;
        }


        return dmgType;
    }

    //reflect out vector to determine bounce for railgun :D
    //Vector2 Reflect(Vector2 vector, Vector2 normal)
    //{
    //    return vector - 2 * Vector2.Dot(vector, normal) * normal;
    //}

    protected override void SpawnSparkEffect()
    {
        //spawn some impact sparks from pool
        GameObject sparkyObj = objectPooler.SpawnFromPool("BulletImpact", transform.position, Quaternion.identity);

        ParticleSystem ps = sparkyObj.GetComponent<ParticleSystem>();

        var main = ps.main;
        main.startColor = new ParticleSystem.MinMaxGradient(player.playerColor);
        ps.Emit(10);

        //start kill timer
        sparkyObj.GetComponent<DisableOverTime>().DisableOverT(2f);
    }

}
