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
        if (collision.collider.tag != "Bullet" || collision.collider.tag != "RedBullet" || collision.collider.tag != "BlueBullet" ||
            collision.collider.tag != "YellowBullet" || collision.collider.tag != "GreenBullet")
        {
            //playerScript.DealColliderDamage(collision, gameObject.tag, null);
            DealColliderDamage(collision);
        }
        else if (collision.collider.tag == "Torso" || collision.collider.tag == "Head" || collision.collider.tag == "Feet" || collision.collider.tag == "Legs")
        {
            DealColliderDamage(collision);
        }
    }


    void Explode()
    {

        foreach (var chunk in explodedChunks)
        {
            chunk.SetActive(true);
            chunk.transform.parent = null;
        }

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

        if (collision.rigidbody != null)
        {

            if (collision.rigidbody.isKinematic == false)
                dmg *= collision.rigidbody.mass;
        }

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
        Gizmos.color = Color.white;
        float theta = 0;
        float x = explosionRadius * Mathf.Cos(theta);
        float y = explosionRadius * Mathf.Sin(theta);
        Vector3 pos = transform.position + new Vector3(x, y, 0);
        Vector3 newPos = pos;
        Vector3 lastPos = pos;
        for (theta = 0.1f; theta < Mathf.PI * 2; theta += 0.1f)
        {
            x = explosionRadius * Mathf.Cos(theta);
            y = explosionRadius * Mathf.Sin(theta);
            newPos = transform.position + new Vector3(x, y, 0);
            Gizmos.DrawLine(pos, newPos);
            pos = newPos;
        }
        Gizmos.DrawLine(pos, lastPos);

    }
}
