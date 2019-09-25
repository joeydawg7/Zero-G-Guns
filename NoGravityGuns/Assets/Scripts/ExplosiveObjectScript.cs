using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveObjectScript : MonoBehaviour
{

    public float health;

    public float explosionRadius = 15;
    public float explosionPower = 250;
    public float damageAtcenter = 40f;
    public float cameraShakeDuration = 0.25f;

    public List<GameObject> explodedChunks;
    PlayerScript playerLastHitBy;


    public void DamageExplosiveObject(float damage, PlayerScript playerLastHitBy)
    {
        this.playerLastHitBy = playerLastHitBy;
        health -= damage;

        if(health<=0)
        {
            Explode();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "ImpactObject")
        {
            //playerScript.DealColliderDamage(collision, gameObject.tag, null);
            DealColliderDamage(collision);
        }
        else if (collision.collider.tag == "Torso" || collision.collider.tag == "Head" || collision.collider.tag == "Feet" || collision.collider.tag == "Legs")
        {
            DealColliderDamage(collision);
            //playerScript.DealColliderDamage(collision, gameObject.tag, hitBy);
        }
    }


    void Explode()
    {
        Explosion explosion;

        GameObject go = ObjectPooler.Instance.SpawnFromPool("Explosion", transform.position, Quaternion.identity, transform.parent);

        explosion = go.GetComponent<Explosion>();

        explosion.Explode(playerLastHitBy, explosionRadius, explosionPower, damageAtcenter,cameraShakeDuration, 40f);


        gameObject.SetActive(false);
    }

    void DealColliderDamage(Collision2D collision)
    {
        float dmg = collision.relativeVelocity.magnitude;
        //reduces damage so its not bullshit
        dmg = dmg / 5;

        //dont bother dealing damage unless unmitigated damage indicates fast enough collision
        if (dmg > 20)
        {

            //caps damage
            if (dmg > 100)
                dmg = 100;

            DamageExplosiveObject(dmg, null);
        }
    }

    //draw the extents of the circle
    private void OnDrawGizmos()
    {
        //x
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x + explosionRadius, transform.position.y));
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x - explosionRadius, transform.position.y));
        //y
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y + explosionRadius));
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y - explosionRadius));
    }
}
