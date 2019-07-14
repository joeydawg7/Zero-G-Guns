using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPartImpact : MonoBehaviour
{

    public PlayerScript playerScript;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "ImpactObject")
        {
            playerScript.DealColliderDamage(collision, gameObject.tag, 0);
        }
        else if (collision.collider.tag == "Torso" || collision.collider.tag == "Head" || collision.collider.tag == "Feet" || collision.collider.tag == "Legs")
        {
            int hitByID = collision.transform.root.GetComponent<PlayerScript>().lastHitByID;
            playerScript.DealColliderDamage(collision, gameObject.tag, hitByID);
        }
    }

}
