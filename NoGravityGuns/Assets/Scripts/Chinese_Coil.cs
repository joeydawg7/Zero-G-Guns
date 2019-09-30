using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chinese_Coil : MonoBehaviour
{   
    public ParticleSystem playerLighting;
    public AudioClip shockClip;

    public float explosionRadius = 15;
    public float explosionPower = 250;
    public float damageAtcenter = 20f;
    public float cameraShakeDuration = 0.25f;


    // Start is called before the first frame update
    void Start()
    {   
        
    }

    private void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            PlayerScript player = collision.gameObject.GetComponent<PlayerScript>();

            if (!player.isDead)
            {
                player.TakeDamage(Random.Range(10, 25), PlayerScript.DamageType.torso, null, false);

                Explosion explosion;

                //explode outwards when hit
                GameObject go = ObjectPooler.Instance.SpawnFromPool("Explosion", transform.position, Quaternion.identity, transform.parent);
                explosion = go.GetComponent<Explosion>();
                explosion.Explode(null, explosionRadius, explosionPower, damageAtcenter, cameraShakeDuration, 40f);


                playerLighting.transform.parent = collision.gameObject.transform;
                playerLighting.transform.localPosition = Vector3.zero;
                playerLighting.Play();
                var audSource = this.gameObject.GetComponent<AudioSource>();                
                audSource.PlayOneShot(shockClip);

            }            
        }
    }
}
