using System.Collections;
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

    GunSO_explosiveShot gun;

    CameraShake cameraShake;

    new Collider2D collider;

    private void Awake()
    {
        cameraShake = Camera.main.GetComponent<CameraShake>();
        audioSouce = GetComponent<AudioSource>();
        collider = GetComponent<Collider2D>();
    }

    //explode from a gun or bullet
    public void Explode(PlayerScript playerWhoShot, GunSO_explosiveShot gun)
    {
        this.playerWhoShot = playerWhoShot;
        this.gun = gun;

        power = gun.explosionPower;
        radius = gun.explosionRadius;
        damageAtCenter = gun.damageAtCenter;

        GrowExplosion(gun.cameraShakeDuration, gun.physicsObjectPushForceMod);
    }

    //explode with custom radius and strength
    public void Explode(PlayerScript playerWhoShot, float radius, float power, float damageAtCenter, float cameraShakeDuration, float physicsObjectForceMod)
    {
        this.playerWhoShot = playerWhoShot;
        this.radius = radius;
        this.power = power;
        this.damageAtCenter = damageAtCenter;

        GrowExplosion(cameraShakeDuration, physicsObjectForceMod);
    }


    void GrowExplosion(float cameraShakeDuration, float physicsObjectPushForceMod)
    {
        Vector3 originalScale = transform.localScale;
        Vector2 explosionPos = collider.bounds.center;
      
        smoke.Emit(2);
        explosionBits.Emit(Random.Range(20, 40));
        chunks.Emit(Random.Range(20, 40));

        cameraShake.shakeDuration += cameraShakeDuration;

        //lets the circle with our given radius actually hurt people
        bool dealDamage = true;

        audioSouce.PlayOneShot(explosionClips[Random.Range(0, explosionClips.Count)]);

        Collider2D[] colliders = Physics2D.OverlapCircleAll(explosionPos, radius);

        foreach (Collider2D hit in colliders)
        {
            Rigidbody2D rb = hit.GetComponent<Rigidbody2D>();

            //the the target has no rigid body but its highest parent has one, use that instead
            if (rb == null && hit.transform.root.GetComponent<Rigidbody2D>() != null)
            {
                rb = hit.transform.root.GetComponent<Rigidbody2D>();
            }

            if (rb != null)
            {
                //explosion cant hit itself, or other explosions for that matter
                if (rb.tag != "Explosion")
                {
                    //treat players different from other objects
                    if (rb.tag == "Player")
                    {
                        //note that this force is only applied to players torso... trying to add more than that caused some crazy effects for little gains in overall usefulness
                        rb.AddExplosionForce(power, damageAtCenter, explosionPos, radius, ForceMode2D.Force, playerWhoShot, dealDamage);
                    }
                    //give impact objects a bit more push than other things
                    else if (rb.tag == "ImpactObject" || rb.tag == "ExplosiveObject" || rb.tag == "Chunk")
                    {                      
                        rb.AddExplosionForce(power * physicsObjectPushForceMod, explosionPos, radius, ForceMode2D.Force);
                    }
                    else
                    {
                        rb.AddExplosionForce(power, explosionPos, radius, ForceMode2D.Force);
                    }
                }
            }
        }

        dealDamage = false;

        transform.parent = null;
        DontDestroyOnLoad(gameObject);

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

        Debug.Log(body.gameObject.name + ": " + dir + ", normalized: " + dir.normalized + " "  + wearoff + " sum force: " + force);

    }

    static void DealDamage(Rigidbody2D body, float damageAtCenter, float wearoff, bool dealDamage, PlayerScript playerWhoShot)
    {
        float dmg = damageAtCenter * wearoff;

        if (body.transform.root.GetComponent<PlayerScript>() != null && dealDamage)
        {
            if (dmg > 0)
            {
                body.transform.root.GetComponent<PlayerScript>().TakeDamage(dmg, PlayerScript.DamageType.torso, playerWhoShot, true);
            }
        }
    }

}