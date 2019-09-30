using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chinese_Coil : MonoBehaviour
{   
    public ParticleSystem playerLighting;
    public AudioClip shockClip;
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
                player.TakeDamage(Random.Range(25, 50), PlayerScript.DamageType.torso, null, false);
                playerLighting.transform.parent = collision.gameObject.transform;
                playerLighting.transform.localPosition = Vector3.zero;
                playerLighting.Play();
                var audSource = this.gameObject.GetComponent<AudioSource>();                
                audSource.PlayOneShot(shockClip);

            }            
        }
    }
}
