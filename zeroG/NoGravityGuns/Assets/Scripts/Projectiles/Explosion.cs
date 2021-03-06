﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour, IPooledObject
{
    float radius;
    float power;
    float damageAtCenter;

    PlayerScript playerWhoShot;

    public ParticleSystem smoke;
    public ParticleSystem explosionBits;
    public ParticleSystem chunks;

    public AudioSource audioSouce;
    public List<AudioClip> explosionClips;

    RPG gun;

    CameraShake cameraShake;

    new Collider2D collider;
 

    private void Awake()
    {
        
        audioSouce = GetComponent<AudioSource>();
        collider = GetComponent<Collider2D>();
    }

    //explode from a gun or bullet
    public void Explode(PlayerScript playerWhoShot, RPG rocketThatShot, bool playDefaultExplosionParticleEffect, bool playFireParticleEffect)
    {
        this.playerWhoShot = playerWhoShot;
        this.gun = rocketThatShot;

        power = gun.explosionPower;
        radius = gun.explosionRadius;
        damageAtCenter = gun.damageAtCenter;

        GrowExplosion(gun.cameraShakeDuration, gun.physicsObjectPushForceMod, playDefaultExplosionParticleEffect, playFireParticleEffect);
    }

    //explode with custom radius and strength
    public void Explode(PlayerScript playerWhoShot, float radius, float power, float damageAtCenter, float cameraShakeDuration,
        float physicsObjectForceMod, bool playDefaultExplosionParticleEffect, bool playFireParticleEffect)
    {
        this.playerWhoShot = playerWhoShot;
        this.radius = radius;
        this.power = power;
        this.damageAtCenter = damageAtCenter;

        GrowExplosion(cameraShakeDuration, physicsObjectForceMod, playDefaultExplosionParticleEffect, playFireParticleEffect);
    }


    void GrowExplosion(float cameraShakeDuration, float physicsObjectPushForceMod, bool playDefaultExplosionParticleEffect, bool playFireParticleEffect)
    {
        Vector3 originalScale = transform.localScale;
        Vector2 explosionPos = collider.bounds.center;

        cameraShake = Camera.main.GetComponent<CameraShake>();

        if (playDefaultExplosionParticleEffect)
        {
            smoke.Emit(2);
            explosionBits.Emit(Random.Range(20, 40));
            chunks.Emit(Random.Range(20, 40));

            if (playFireParticleEffect)
            {
                //spawn the flame effect from pool and start playing it
                ParticleSystem ps = ObjectPooler.Instance.SpawnFromPool("ExplosiveObjectFire", transform.position, Quaternion.identity, this.transform).GetComponentInChildren<ParticleSystem>();
                WindZone wz = ps.transform.parent.GetComponentInChildren<WindZone>();

                if (wz)
                    wz.windMain = 150;
                if (ps)
                    ps.transform.parent.SetParent(null);
            }

        }

        cameraShake.shakeDuration += cameraShakeDuration;

        //lets the circle with our given radius actually hurt people
        bool dealDamage = true;

        if (playDefaultExplosionParticleEffect)
            SoundPooler.Instance.PlaySoundEffect(explosionClips[Random.Range(0, explosionClips.Count)]);
            //audioSouce.PlayOneShot(explosionClips[Random.Range(0, explosionClips.Count)]);

        Collider2D[] colliders = Physics2D.OverlapCircleAll(explosionPos, radius);

        colliders = RemoveDuplicates(colliders);

        foreach (Collider2D hit in colliders)
        {
            Rigidbody2D rb = hit.GetComponent<Rigidbody2D>();

            //the target has no rigid body but its highest parent has one, use that instead
            if (rb == null && hit.transform.root.GetComponent<Rigidbody2D>() != null)
            {
                rb = hit.transform.root.GetComponent<Rigidbody2D>();
            }

           

            if (rb != null)
            {
                //explosion cant hit itself, or other explosions for that matter
                if (rb.tag != "Explosion" || rb.tag != "Bullet")
                {
                    //treat players different from other objects
                  //  if (rb.tag == "Player")
                 //   {
                        //note that this force is only applied to players torso... trying to add more than that caused some crazy effects for little gains in overall usefulness
                       
                   // }
                    //give impact objects a bit more push than other things
                    if (rb.tag == "ImpactObject" || rb.tag == "ExplosiveObject" || rb.tag == "Chunk")
                    {
                        rb.AddExplosionForce(power * physicsObjectPushForceMod, explosionPos, radius, ForceMode2D.Impulse);
                    }
                    else if (rb.tag == "Torso")
                    {
                        rb.AddExplosionForce(power * physicsObjectPushForceMod, damageAtCenter, explosionPos, radius, ForceMode2D.Force, playerWhoShot, dealDamage);
                    }
                }
            }
        }

        dealDamage = false;

        transform.parent = null;
        gameObject.DontDestroyOnLoad();

    }

    //draw the extents of the circle
    private void OnDrawGizmos()
    {
        Transform T= GetComponent<Transform>();
        Gizmos.color = Color.white;
        float theta = 0;
        float x = radius * Mathf.Cos(theta);
        float y = radius * Mathf.Sin(theta);
        Vector3 pos= T.position + new Vector3(x, 0, y);
        Vector3 newPos = pos;
        Vector3 lastPos = pos;
        for (theta = 0.1f; theta < Mathf.PI * 2; theta += 0.1f)
        {
            x = radius * Mathf.Cos(theta);
            y = radius * Mathf.Sin(theta);
            newPos = T.position + new Vector3(x, 0, y);
            Gizmos.DrawLine(pos, newPos);
            pos = newPos;
        }
        Gizmos.DrawLine(pos, lastPos);

    }

    public void OnObjectSpawn()
    {
        cameraShake = Camera.main.GetComponent<CameraShake>();
    }

    Collider2D[] RemoveDuplicates(Collider2D[] s)
    {
        HashSet<Collider2D> set = new HashSet<Collider2D>(s);
        Collider2D[] result = new Collider2D[set.Count];
        set.CopyTo(result);
        return result;
    }
}

//class to add explosive force to a 2d rigidbody
public static class Rigidbody2DExt
{

    const float EXPLOSION_DAMAGE_MITIGATOR = 11f;

    public static void AddExplosionForce(this Rigidbody2D body, float explosionForce, float damageAtCenter,  Vector3 explosionPosition, float explosionRadius, ForceMode2D mode, PlayerScript playerWhoShot, bool dealDamage)
    {
        var dir = (body.transform.position - explosionPosition);

        float wearoff = 1 - (dir.magnitude / explosionRadius);
        body.AddForce(dir.normalized * (wearoff <= 0f ? 0f : explosionForce) * wearoff);

        //Debug.Log(body.gameObject.name + ": " + wearoff + " " + force);

        PlayerScript playerScript = body.transform.root.GetComponentInChildren<PlayerScript>();

        if(playerScript!=null)
            DealDamage(body, damageAtCenter, wearoff, dealDamage, playerWhoShot);
    }

    //overload that doesnt care about dealing damage to affected body
    public static void AddExplosionForce(this Rigidbody2D body, float explosionForce, Vector3 explosionPosition, float explosionRadius, ForceMode2D mode)
    {
        var dir = (body.transform.position - explosionPosition);

        //Debug.Log(dir);

        //if(dir== new Vector3(0,0,0))
        //{
        //    dir = new Vector3(Random.Range(-20,20), Random.Range(-20,20), 0f);
        //}

        float wearoff = 1f - (dir.magnitude / explosionRadius);

        body.AddForce(dir.normalized * (wearoff <= 0f ? 0f : explosionForce) * wearoff);

        //Debug.Log(dir.normalized.x * wearoff * wearoff);

        Vector3 force = dir.normalized * (wearoff <= 0f ? 0f : explosionForce) * wearoff;

       // Debug.Log(body.gameObject.name + ": " + dir + ", normalized: " + dir.normalized + " "  + wearoff + " sum force: " + force);

    }

    static void DealDamage(Rigidbody2D body, float damageAtCenter, float wearoff, bool dealDamage, PlayerScript playerWhoShot)
    {
        float dmg = damageAtCenter * wearoff;

        //Debug.Log("wearoff = " + wearoff);
       // Debug.Log("Explosion dealt " + dmg + " damage");

        PlayerScript HitplayerScript = body.transform.root.GetComponentInChildren<PlayerScript>();

        PlayerScript.DamageType damageType = PlayerScript.DamageType.explosive; //PlayerScript.ParsePlayerDamage(body.gameObject);

        //headshot explosions are way too strong
        if (damageType == PlayerScript.DamageType.head)
            damageType = PlayerScript.DamageType.torso;

        if (HitplayerScript != null && dealDamage )
        {
            if (dmg > 0)
            {
                //sets collision damage timer to half, so if you fly for longer than half a second youll take collisions but otherwise you wont take full damage from begin right next to a wall
                //HitplayerScript.immuneToCollisionsTimer = 0.75f;
                HitplayerScript.explosionTimer = 1f;
                //actually deal the damage
                HitplayerScript.TakeDamage(dmg, new Vector2(0,0), damageType, playerWhoShot, true, null);
            }
        }
    }

}