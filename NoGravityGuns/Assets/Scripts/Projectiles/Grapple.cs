using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grapple : Bullet
{
    DistanceJoint2D distanceJoint;

    public override void Construct(float damage, PlayerScript player, Vector3 dir, Sprite sprite, GunSO gun)
    {
        base.Construct(damage, player, dir, sprite, gun);

        distanceJoint = GetComponent<DistanceJoint2D>();
        distanceJoint.enabled = false;

        //TODO: make this a rope trail :D
        /*
        GameObject temp2 = objectPooler.SpawnFromPool("RocketTrail", gameObject.transform.position, Quaternion.identity);
        somethingSexy = temp2.GetComponent<ParticleSystem>();
        somethingSexy.transform.parent = transform;
        */
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        //TODO: make this more efficient, no need to check player impact location if we already know its not a player we've hit
        if (collision.collider.gameObject.layer != LayerMask.NameToLayer("NonBulletCollide") && canImapact == true)
        {
            //we've hit something that isnt a bullet, or the player that shot the original bullet
            if (collision.collider.tag != "Bullet" || collision.collider.GetComponent<Bullet>().player.playerID != player.playerID)
            {

                ExplosiveObjectScript explosiveObjectScript = collision.collider.gameObject.GetComponent<ExplosiveObjectScript>();

                if (explosiveObjectScript != null)
                {
                    //ExplosiveObjectScript explosiveObjectScript = collision.collider.gameObject.GetComponent<ExplosiveObjectScript>();

                    if (explosiveObjectScript != null && damage > 0)
                    {
                        explosiveObjectScript.DamageExplosiveObject(damage, player);
                    }
                }

                //default damage type is nothing, we don't know what we hit yet.
                PlayerScript.DamageType dmgType = DamageBodyParts(collision);
                SpawnSparkEffect();

                Rigidbody2D hitRB = collision.gameObject.GetComponent<Rigidbody2D>();

                if (hitRB == null)
                {
                    distanceJoint.enabled = true;
                    rb.isKinematic = true;                    
                    rb.velocity = new Vector2(0, 0);
                    rb.angularVelocity = 0;
                    distanceJoint.connectedBody = player.rb;
                    distanceJoint.distance = Vector2.Distance(player.transform.position, transform.position);
                }
                else
                {
                    transform.parent = hitRB.transform;

                    distanceJoint.enabled = true;
                    rb.isKinematic = true;
                    rb.velocity = new Vector2(0, 0);
                    rb.angularVelocity = 0;
                    GetComponent<CircleCollider2D>().enabled = false;
                    distanceJoint.connectedBody = player.rb;
                    distanceJoint.distance = Vector2.Distance(player.transform.position, transform.position);
                }


            }
        }
    }

}
