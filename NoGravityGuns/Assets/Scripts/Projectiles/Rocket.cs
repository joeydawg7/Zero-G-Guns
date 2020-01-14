using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

public class Rocket : Bullet
{

    Vector3 dir;

    float timeInFlight;
    float rocketTopSpeed;
    float rocketAccelerationMod;

    public void Construct(float damage, PlayerScript player, Vector3 dir, Color32 color, RPG thRPG)
    {
        //call the base version first, the rest of the stuff we do after
        base.Construct(damage, player, dir, color);

        if (gun.GetType() != typeof(RPG))
        {
            Debug.LogError("Tried to spawn a rocket but wasn't given rocket stats!");

            return;
        }

        timeInFlight = 0f;
        rocketAccelerationMod = 0;
        rocketTopSpeed = 0;

        //dont change rocket collision layer, we dont want it colliding with other bullets
        sr.enabled = true;

        RPG gunExplosive = (RPG)gun;

        rocketTopSpeed = gunExplosive.rocketMaxSpeed;
        rocketAccelerationMod = gunExplosive.rocketAccelerationMod;

        this.dir = dir;

        rb.AddForce(dir , ForceMode2D.Force);   
        


    }



    private void FixedUpdate()
    {
        //accelerate the rocket over time if it exists
        if (rb != null)
        {
            if (rb.velocity.sqrMagnitude < rocketTopSpeed)
            {
                timeInFlight += Time.deltaTime;

                //Vector2 dir = rb.velocity;
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);


                rb.AddForce(dir * rocketAccelerationMod, ForceMode2D.Force);
            }

        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawLine(transform.position , dir * rocketAccelerationMod);
    }


    protected override void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.collider.gameObject.layer != LayerMask.NameToLayer("NonBulletCollide") && canImapact == true)
        {

            if (collision.collider.tag == "Chunk")
                return;


            //we've hit something that isnt a bullet, or the player that shot us
            if (collision.collider.tag != "Bullet" || collision.collider.GetComponent<Bullet>().player.playerID != player.playerID)
            {
                //default damage type is nothing, we don't know what we hit yet.
                PlayerScript.DamageType dmgType = DamageBodyParts(collision);

                ExplosiveObjectScript explosiveObjectScript = collision.collider.gameObject.GetComponent<ExplosiveObjectScript>();

                if (explosiveObjectScript != null)
                {
                    //ExplosiveObjectScript explosiveObjectScript = collision.collider.gameObject.GetComponent<ExplosiveObjectScript>();

                    if (explosiveObjectScript != null && damage > 0)
                    {
                        explosiveObjectScript.DamageExplosiveObject(damage, player, collision.transform.position);
                    }
                }

                //spawn some impact sparks from pool
                SpawnSparkEffect();

                RPG rocketShot = (RPG)gun;

                //if you are a rcoket who hit something, blow up
                ExplodeBullet(false, rocketShot);

            }
        }
    }


    //explodes a bullet "gracefully"
    public void ExplodeBullet(bool explodeInstantly, RPG rocketThatShot)
    {
        gameObject.GetComponent<Collider2D>().enabled = false;
        GameObject explosionGO = ObjectPooler.Instance.SpawnFromPool("Explosion", transform.position, Quaternion.identity);
        Explosion explosion = explosionGO.GetComponent<Explosion>();

        if (explosion != null)
        {
            explosion.gameObject.SetActive(true);
            explosion.Explode(player, rocketThatShot);
            rb.simulated = false;
            rb.isKinematic = true;
        }
        else
            Debug.LogError("Failed to spawn explosion from object pool!");

        sr.enabled = false;
        StartCoroutine(DisableOverTime(0.6f));

        if (!explodeInstantly)
        {
            bulletTrail.Stop();
            bulletTrail.GetComponent<DisableOverTime>().DisableOverT(3.1f);
            bulletTrail.transform.parent = null;
        }
    }

}
