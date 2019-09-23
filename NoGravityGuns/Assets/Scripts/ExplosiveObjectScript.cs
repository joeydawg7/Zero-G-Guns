using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveObjectScript : MonoBehaviour
{

    public float health;

    public float explosionRadius = 15;
    public float explosionPower = 250;
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

 

    void Explode()
    {
        Explosion explosion;

        GameObject go = ObjectPooler.Instance.SpawnFromPool("Explosion", transform.position, Quaternion.identity, transform.parent);

        explosion = go.GetComponent<Explosion>();

        explosion.Explode(playerLastHitBy, explosionRadius, explosionPower, cameraShakeDuration);


        gameObject.SetActive(false);
    }
}
