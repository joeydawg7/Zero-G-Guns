using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chainsaw : Guns
{

    //public Vector2 hitBoxSize;
    public float hitBoxRadius;
    public Vector2 hitBoxPos;

    public LayerMask playerLayer;


    public ParticleSystem sparks;
    public ParticleSystem fire;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere((Vector2)transform.position + hitBoxPos, hitBoxRadius);
    }

    public override void Fire(PlayerScript player)
    {
        if (CheckIfAbleToiFire(this))
        {
            player.StartCoroutine(DelayShotCoroutine(player, 0f, base.bulletSpeed, minDamageRange, maxDamageRange, this));
            ReduceBullets(player);
        }
    }


    public override IEnumerator DelayShotCoroutine(PlayerScript player, float delayBeforeShot, float bulletSpeed, int minDamage, int maxDamage, Guns gun)
    {
        return base.DelayShotCoroutine(player, delayBeforeShot, bulletSpeed, minDamage, maxDamage, gun);
    }

    public override void SpawnBullet(PlayerScript player, float bulletSpeed, int minDamagae, int maxDamage, Guns gun)
    {

        fire.Emit(1);

        int damage = Random.Range(minDamagae, maxDamage);

        Vector2 startingForce = new Vector2(1f, 1f);

        PlayerScript.DamageType dmgType = PlayerScript.DamageType.torso;

        Collider2D[] colliders = Physics2D.OverlapCircleAll((Vector2)transform.position + hitBoxPos, hitBoxRadius);

        if (colliders.Length > 0)
        {

            foreach (var c in colliders)
            {
                //gets the hit collider if its a player (must go from root down because player script is on hip)
                PlayerScript hitPlayer = c.transform.root.GetComponentInChildren<PlayerScript>();
                ExplosiveObjectScript explosiveObjectScript = c.GetComponent<ExplosiveObjectScript>();
                ArmsScript arms = player.armsScript;
                Rigidbody2D rb2D = c.GetComponent<Rigidbody2D>();

                //hit yourself, move on!
                if (hitPlayer && hitPlayer.playerID == player.playerID)
                {
                    continue;
                }

                //hits a player
                if (hitPlayer)
                {
                    hitPlayer.TakeDamage(damage, startingForce, dmgType, player, true, gun);

                    hitPlayer.rb.AddForce(arms.bulletSpawn.transform.right * knockBack * 1, ForceMode2D.Impulse);

                    sparks.Emit(1);
                }
                //hits an explosive object
                else if (explosiveObjectScript)
                {
                    rb2D.AddForce(arms.bulletSpawn.transform.right * knockBack * 1, ForceMode2D.Impulse);
                    explosiveObjectScript.DamageExplosiveObject(damage, player, arms.bulletSpawn.transform.right);

                    sparks.Emit(1);
                }
                // hits a non-explosive object but something we can push (not us!)
                else if (rb2D)
                {
                    rb2D.AddForce(arms.bulletSpawn.transform.right * knockBack * 1, ForceMode2D.Impulse);

                    sparks.Emit(1);
                }
            }
        }

    }

    public override void KnockBack(PlayerScript player, float knockBackModifier)
    {
        ArmsScript arms = player.armsScript;

        if (player.transform && arms.bulletSpawn)
        {
            player.rb.AddForce(arms.bulletSpawn.transform.right * knockBack * knockBackModifier, ForceMode2D.Impulse);
        }
    }

}
