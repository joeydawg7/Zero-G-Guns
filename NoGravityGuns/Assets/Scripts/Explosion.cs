using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour, IPooledObject
{
    float radius;
    float power;

    PlayerScript playerWhoShot;

    public ParticleSystem smoke;
    public ParticleSystem explosionBits;
    public ParticleSystem chunks;

    public AudioSource audioSouce;
    public List<AudioClip> explosionClips;

    GunSO_explosiveShot gun;

    CameraShake cameraShake;

    private void Awake()
    {
        cameraShake = Camera.main.GetComponent<CameraShake>();
        audioSouce = GetComponent<AudioSource>();
    }

    //explode from a gun or bullet
    public void Explode(PlayerScript playerWhoShot, GunSO_explosiveShot gun)
    {
        this.playerWhoShot = playerWhoShot;
        this.gun = gun;

        power = gun.explosionPower;
        radius = gun.explosionRadius;

        GrowExplosion(gun.cameraShakeDuration, gun.physicsObjectPushForceMod);
    }

    //explode with custom radius and strength
    public void Explode(PlayerScript playerWhoShot, float radius, float power, float cameraShakeDuration, float physicsObjectForceMod)
    {
        this.playerWhoShot = playerWhoShot;
        this.radius = radius;
        this.power = power;


        GrowExplosion(cameraShakeDuration, physicsObjectForceMod);
    }


    void GrowExplosion(float cameraShakeDuration, float physicsObjectPushForceMod)
    {
        Vector3 originalScale = transform.localScale;
        Vector2 explosionPos = transform.position;

        
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
                        Rigidbody2DExt.AddExplosionForce(rb, power, explosionPos, radius, ForceMode2D.Force, playerWhoShot, dealDamage);
                    }
                    //give impact objects a bit more push than other things
                    else if (rb.tag == "ImpactObject" || rb.tag == "ExplosiveObject")
                    {
                        Rigidbody2DExt.AddExplosionForce(rb, power * physicsObjectPushForceMod, explosionPos, radius, ForceMode2D.Force);
                    }
                    else
                    {
                        Rigidbody2DExt.AddExplosionForce(rb, power, explosionPos, radius, ForceMode2D.Force);
                    }
                }
            }
        }

        dealDamage = false;

    }

    //draw the extents of the circle
    private void OnDrawGizmos()
    {
        //x
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x + radius, transform.position.y));
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x - radius, transform.position.y));
        //y
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y + radius));
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y - radius));
    }

    public void OnObjectSpawn()
    {
        cameraShake = Camera.main.GetComponent<CameraShake>();
    }
}

//class to add explosive force to a 2d rigidbody
public static class Rigidbody2DExt
{

    public static void AddExplosionForce(this Rigidbody2D body, float explosionForce, Vector3 explosionPosition, float explosionRadius, ForceMode2D mode, PlayerScript playerWhoShot, bool dealDamage)
    {
        var dir = (body.transform.position - explosionPosition);
        float wearoff = 1 - (dir.magnitude / explosionRadius);
        if (wearoff < 0)
            wearoff = 0;
        Vector2 force = dir.normalized * explosionForce * wearoff;
        body.AddForce(force, mode);

        float dmg = (explosionForce * wearoff) / 12f;

        if (body.transform.root.GetComponent<PlayerScript>() != null && dealDamage)
        {
            if (dmg > 0)
            {
                body.transform.root.GetComponent<PlayerScript>().TakeDamage(dmg, PlayerScript.DamageType.torso, playerWhoShot, true);
            }
        }
    }

    //overload that doesnt care about dealing damage to affected body part
    public static void AddExplosionForce(this Rigidbody2D body, float explosionForce, Vector3 explosionPosition, float explosionRadius, ForceMode2D mode)
    {
        var dir = (body.transform.position - explosionPosition);
        float wearoff = 1 - (dir.magnitude / explosionRadius);
        Vector2 force = dir.normalized * explosionForce * wearoff;
        body.AddForce(force, mode);

    }

}