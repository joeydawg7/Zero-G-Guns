using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chainsaw : Guns
{
    //120

    public Vector2 hitBoxSize;


    public LayerMask playerLayer;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireCube(transform.position, new Vector3(hitBoxSize.x, hitBoxSize.y, 5f));
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

        int damage = Random.Range(minDamagae, maxDamage);

        Vector2 startingForce = new Vector2(1f, 1f);

        PlayerScript.DamageType dmgType = PlayerScript.DamageType.torso;

        Collider[] colliders = Physics.OverlapBox(transform.position, hitBoxSize / 2, Quaternion.identity, playerLayer);

        Debug.Log(colliders.Length);

        if (colliders.Length > 0)
        {
            foreach (var c in colliders)
            {
                PlayerScript hitPlayer = c.GetComponentInChildren<PlayerScript>();

                if (hitPlayer && hitPlayer != player)
                {
                    Debug.Log("hit");
                    hitPlayer.TakeDamage(damage, startingForce, dmgType, player, true, gun);
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
