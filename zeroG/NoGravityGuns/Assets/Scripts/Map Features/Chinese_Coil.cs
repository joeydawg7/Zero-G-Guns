﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chinese_Coil : MonoBehaviour
{   
    public ParticleSystem playerLighting;
    public AudioClip shockClip;

    public float explosionRadius = 15;
    public float explosionPower = 400f;
    public float damageAtcenter = 10f;
    public float cameraShakeDuration = 0.25f;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player" || collision.tag == "Torso" || collision.tag == "Leg" || collision.tag == "Feet" || collision.tag == "Head")
        {
            PlayerScript player = collision.transform.root.GetComponentInChildren<PlayerScript>();

            if (player == null)
                return;

            if (!player.isDead)
            {
               // player.TakeDamage(Random.Range(10, 25), PlayerScript.DamageType.torso, null, false);

                Explosion explosion;

                //explode outwards when hit
                GameObject go = ObjectPooler.Instance.SpawnFromPool("Explosion", GetComponent<Collider2D>().bounds.center, Quaternion.identity, transform.parent);
                explosion = go.GetComponent<Explosion>();
                explosion.Explode(null, explosionRadius, explosionPower, damageAtcenter, cameraShakeDuration, 40f, false, false);

                /*
                playerLighting.transform.parent = collision.gameObject.transform;
                playerLighting.transform.localPosition = Vector3.zero;
                playerLighting.Play();
                */

                playerLighting.Play();

                var audSource = this.gameObject.GetComponent<AudioSource>();    
                
                if(!audSource.isPlaying)
                    audSource.PlayOneShot(shockClip);

            }            
        }
    }
}
