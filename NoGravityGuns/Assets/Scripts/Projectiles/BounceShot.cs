using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceShot : Bullet
{
    bool canBounce;

    public override void Construct(float damage, PlayerScript player, Vector3 dir, Sprite sprite, GunSO gun)
    {
        base.Construct(damage, player, dir, sprite, gun);

        canBounce = true;
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);

        //only bounce if you are a railgun bullet that hasnt hit a player, and only do it once. 
        if (canBounce == false)
        {
            canImapact = false;
            KillBullet();
            return;
        }
        /*
        else if (dmgType != PlayerScript.DamageType.none)
        {
            canImapact = false;
            KillBullet();
            return;
        }*/
        else
        {
            canBounce = false;
        }

        rb.AddForce(Reflect(startingForce, collision.GetContact(0).normal));

    }

    protected override PlayerScript.DamageType DamageBodyParts(Collision2D collision)
    {
        //default damage type is nothing, we don't know what we hit yet.
        PlayerScript.DamageType dmgType = PlayerScript.DamageType.none;

        //checks where we hit the other guy, deals our given damage to that location. 
        if (collision.collider.tag == "Torso")
        {
            dmgType = PlayerScript.DamageType.torso;
            collision.transform.root.gameObject.GetComponent<PlayerScript>().TakeDamage(damage, dmgType, player, true);
            collision.transform.GetComponentInChildren<ParticleSystem>().Emit(30);
            canBounce = false;
            GetComponent<Collider2D>().enabled = false;
        }
        if (collision.collider.tag == "Head")
        {
            dmgType = PlayerScript.DamageType.head;
            collision.transform.root.gameObject.GetComponent<PlayerScript>().TakeDamage(damage, dmgType, player, true);
            canBounce = false;
            GetComponent<Collider2D>().enabled = false;
        }
        if (collision.collider.tag == "Feet")
        {
            dmgType = PlayerScript.DamageType.feet;
            collision.transform.root.gameObject.GetComponent<PlayerScript>().TakeDamage(damage, dmgType, player, true);
            canBounce = false;
            GetComponent<Collider2D>().enabled = false;
        }
        if (collision.collider.tag == "Leg")
        {
            dmgType = PlayerScript.DamageType.legs;
            collision.transform.root.gameObject.GetComponent<PlayerScript>().TakeDamage(damage, dmgType, player, true);
            canBounce = false;
            GetComponent<Collider2D>().enabled = false;
        }

        return dmgType;
    }

    //reflect out vector to determine bounce for railgun :D
    Vector2 Reflect(Vector2 vector, Vector2 normal)
    {
        return vector - 2 * Vector2.Dot(vector, normal) * normal;
    }

}
