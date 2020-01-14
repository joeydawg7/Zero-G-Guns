using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetScript : MonoBehaviour
{
    TargetsManager targetsManager;
    float health = 1f;
    PlayerScript playerLastHitBy;
    ParticleSystem deathEffect;
    private void Start()
    {
        targetsManager = FindObjectOfType<TargetsManager>();
        deathEffect = GetComponentInChildren<ParticleSystem>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //first check if you got hit by a bullet, if so take damage from it
        if (collision.collider.tag == "Bullet")
        {
            Bullet bullet = collision.collider.GetComponent<Bullet>();
            DamageTarget(bullet.damage, bullet.player);
        }
        //else assume its some kind of physics collision and see if it hurts
        else
        {
            if (collision.collider.tag != "Bullet" || collision.collider.tag != "RedBullet" || collision.collider.tag != "BlueBullet" ||
                collision.collider.tag != "YellowBullet" || collision.collider.tag != "GreenBullet")
            {
                DealColliderDamage(collision);
            }
            else if (collision.collider.tag == "Torso" || collision.collider.tag == "Head" || collision.collider.tag == "Feet" || collision.collider.tag == "Legs")
            {
                DealColliderDamage(collision);
            }
        }
       
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

            DamageTarget(dmg, null);
        }
    }

    public void DamageTarget(float damage, PlayerScript playerLastHitBy)
    {
        this.playerLastHitBy = playerLastHitBy;
        health -= damage;

        if (health <= 0)
        {
            deathEffect.transform.parent = null;
            deathEffect.Play(true);
            targetsManager.TargetDestroyed(deathEffect.transform);
            Destroy(gameObject);
        }
    }
}
