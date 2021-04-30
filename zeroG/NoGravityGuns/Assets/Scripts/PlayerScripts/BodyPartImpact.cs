using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPartImpact : MonoBehaviour
{
    [HideInInspector]
    public PlayerScript playerScript;

    private void Awake()
    {
        playerScript = transform.root.GetComponentInChildren<PlayerScript>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag != "Bullet" || collision.collider.tag != "RedBullet" || collision.collider.tag != "BlueBullet" ||
            collision.collider.tag != "YellowBullet" || collision.collider.tag != "GreenBullet")
        {
            if (playerScript)
                playerScript.DealColliderDamage(collision, gameObject, null);
        }
        //someone else bumped into you
        else if (collision.collider.tag == "Torso" || collision.collider.tag == "Head" || collision.collider.tag == "Feet" || collision.collider.tag == "Legs")
        {
            PlayerScript hitBy = collision.transform.root.GetComponent<PlayerScript>();
            playerScript.DealColliderDamage(collision, gameObject, hitBy);
        }
    }

}
