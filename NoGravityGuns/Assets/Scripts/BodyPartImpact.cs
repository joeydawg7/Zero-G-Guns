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
            playerScript.DealColliderDamage(collision, gameObject.tag, null);
        }
        else if (collision.collider.tag == "Torso" || collision.collider.tag == "Head" || collision.collider.tag == "Feet" || collision.collider.tag == "Legs")
        {
            PlayerScript hitBy = collision.transform.root.GetComponent<PlayerScript>();
            playerScript.DealColliderDamage(collision, gameObject.tag, hitBy);
        }
    }

}
