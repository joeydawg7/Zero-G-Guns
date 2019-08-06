using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    float radius = 18f;
    float power = 400f;

    PlayerScript playerWhoShot;


    public void Explode(PlayerScript playerWhoShot)
    {
        StartCoroutine(GrowExplosion(0.5f));
        this.playerWhoShot = playerWhoShot;
    }

    IEnumerator GrowExplosion(float time)
    {

        Vector3 originalScale = transform.localScale;
        Vector3 destinationScale = new Vector3(8.0f, 8.0f, 1.0f);
        Vector2 explosionPos = transform.position;

        bool dealDamage = true;

        float currentTime = 0.0f;

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
                if (rb.tag != "Explosion")
                {
                    if (rb.tag == "Player")
                    {
                        Rigidbody2DExt.AddExplosionForce(rb, power, explosionPos, radius, ForceMode2D.Force, playerWhoShot, dealDamage);
                        dealDamage = false;
                    }
                    else
                    {
                        Rigidbody2DExt.AddExplosionForce(rb, power, explosionPos, radius, ForceMode2D.Force);
                    }
                }
            }
        }

        do
        {
            transform.localScale = Vector3.Lerp(originalScale, destinationScale, currentTime / time);
            currentTime += Time.deltaTime;

            yield return null;
        } while (currentTime <= time);



    }

    private void OnDrawGizmos()
    {
        //x
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x + radius, transform.position.y));
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x - radius, transform.position.y));
        //y
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x , transform.position.y + radius));
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x , transform.position.y - radius));
    }

}


public static class Rigidbody2DExt
{

    //public static void AddExplosionForce(this Rigidbody2D rb, float explosionForce, Vector2 explosionPosition, float explosionRadius, float upwardsModifier = 0.0F, ForceMode2D mode = ForceMode2D.Force)
    //{
    //    var explosionDir = rb.position - explosionPosition;
    //    var explosionDistance = explosionDir.magnitude;

    //    // Normalize without computing magnitude again
    //    if (upwardsModifier == 0)
    //        explosionDir /= explosionDistance;
    //    else
    //    {
    //        // From Rigidbody.AddExplosionForce doc:
    //        // If you pass a non-zero value for the upwardsModifier parameter, the direction
    //        // will be modified by subtracting that value from the Y component of the centre point.
    //        explosionDir.y += upwardsModifier;
    //        explosionDir.Normalize();
    //    }
    //    Debug.Log("adding " + Mathf.Lerp(1f, explosionForce, (1 - explosionDistance)) * explosionDir + " force to " + rb.gameObject.name);

    //    rb.AddForce(Mathf.Lerp(1f, explosionForce, (1 - explosionDistance)) * explosionDir, mode);
    //}

    public static void AddExplosionForce(this Rigidbody2D body, float explosionForce, Vector3 explosionPosition, float explosionRadius, ForceMode2D mode, PlayerScript playerWhoShot, bool dealDamage)
    {
        var dir = (body.transform.position - explosionPosition);
        float wearoff = 1 - (dir.magnitude / explosionRadius);
        Vector2 force = dir.normalized * explosionForce * wearoff;
        body.AddForce(force, mode);

        float dmg = (explosionForce * wearoff) / 5f;

        if (body.transform.root.GetComponent<PlayerScript>() != null && dealDamage)
        {
            if (dmg > 0)
                body.transform.root.GetComponent<PlayerScript>().TakeDamage(dmg, PlayerScript.DamageType.torso, playerWhoShot, true, PlayerScript.GunType.RPG);
        }
    }

    public static void AddExplosionForce(this Rigidbody2D body, float explosionForce, Vector3 explosionPosition, float explosionRadius, ForceMode2D mode)
    {
        var dir = (body.transform.position - explosionPosition);
        float wearoff = 1 - (dir.magnitude / explosionRadius);
        Vector2 force = dir.normalized * explosionForce * wearoff;
        body.AddForce(force, mode);


    }

    //public static void AddExplosionForce(this Rigidbody2D body, float explosionForce, Vector3 explosionPosition, float explosionRadius, float upliftModifier)
    //{
    //    var dir = (body.transform.position - explosionPosition);
    //    float wearoff = 1 - (dir.magnitude / explosionRadius);
    //    Vector3 baseForce = dir.normalized * explosionForce * wearoff;
    //    body.AddForce(baseForce);

    //    float upliftWearoff = 1 - upliftModifier / explosionRadius;
    //    Vector3 upliftForce = Vector2.up * explosionForce * upliftWearoff;
    //    body.AddForce(upliftForce);
    //}
}