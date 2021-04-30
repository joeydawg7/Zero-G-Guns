using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadPlayerScript : MonoBehaviour
{
    Rigidbody2D rb;

    //Spawn a rotting corpse for the player who perished
    public void OnDeadPlayerSpawn(Rigidbody2D oldRB, GameObject go)
    {
        rb = GetComponent<Rigidbody2D>();

        //turn off rb while we set things
        rb.isKinematic = true;

        //rb stuff
        rb.velocity = oldRB.velocity;
        rb.angularVelocity = oldRB.angularVelocity;

        //gameobject pos
        gameObject.transform.position = go.transform.position;

        //tranform
        transform.rotation = go.transform.rotation;
        transform.position = go.transform.position;


        //turn rb back on
        rb.isKinematic = false;

    }
}
