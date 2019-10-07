using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceShot : Bullet
{
    bool canBounce;

    Vector2 dir;

    public override void Construct(float damage, PlayerScript player, Vector3 dir, Color32 color, GunSO gun)
    {
        base.Construct(damage, player, dir, color, gun);

        canBounce = true;

        this.dir = dir;
        SetPFXTrail("RocketTrail", true);

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
                        explosiveObjectScript.DamageExplosiveObject(damage, player);
                    }
                }

                //default damage type is nothing, we don't know what we hit yet.
                PlayerScript.DamageType dmgType = DamageBodyParts(collision);
                SpawnSparkEffect();

                canImapact = false;
            }
        }

        //only bounce if you are a railgun bullet that hasnt hit a player, and only do it once. 
        if (canBounce == false)
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
            canBounce = false;
        }

        rb.AddForce(Reflect(startingForce, collision.GetContact(0).normal));

    }

    protected override PlayerScript.DamageType DamageBodyParts(Collision2D collision)
    {
        //default damage type is nothing, we don't know what we hit yet.
        PlayerScript.DamageType dmgType = PlayerScript.DamageType.self;

        //we can get out of here early if there is no player script component on the root parent of whatever we hit, because that 100% is not a player :D
        PlayerScript hitPlayerScript = collision.transform.root.GetComponentInChildren<PlayerScript>();
        if (hitPlayerScript == null)
            return dmgType;

        //checks where we hit the other guy, deals our given damage to that location. 
        if (collision.collider.tag == "Torso")
        {
            dmgType = PlayerScript.DamageType.torso;
            hitPlayerScript.TakeDamage(damage,dir, dmgType, player, true);
            //collision.transform.GetComponentInChildren<ParticleSystem>().Emit(30);
            GetComponent<Collider2D>().enabled = false;
        }
        if (collision.collider.tag == "Head")
        {
            dmgType = PlayerScript.DamageType.head;
            hitPlayerScript.TakeDamage(damage, dir, dmgType, player, true);
            GetComponent<Collider2D>().enabled = false;
        }
        if (collision.collider.tag == "Feet")
        {
            dmgType = PlayerScript.DamageType.feet;
            hitPlayerScript.TakeDamage(damage, dir, dmgType, player, true);     
            GetComponent<Collider2D>().enabled = false;
        }
        if (collision.collider.tag == "Leg")
        {
            dmgType = PlayerScript.DamageType.legs;
            hitPlayerScript.TakeDamage(damage, dir, dmgType, player, true);
            GetComponent<Collider2D>().enabled = false;
        }

        canBounce = false;

        return dmgType;
    }

    //reflect out vector to determine bounce for railgun :D
    Vector2 Reflect(Vector2 vector, Vector2 normal)
    {
        return vector - 2 * Vector2.Dot(vector, normal) * normal;
    }

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
