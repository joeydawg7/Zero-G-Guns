using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetScript : MonoBehaviour
{
    TargetsManager targetsManager;

    private void Start()
    {
        targetsManager = FindObjectOfType<TargetsManager>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.tag == "Bullet")
        {
            targetsManager.TargetDestroyed();
            Destroy(gameObject);
        }
    }
}
