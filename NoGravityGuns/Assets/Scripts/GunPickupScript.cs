using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunPickupScript : MonoBehaviour
{

    public GunSO gun;

    // Start is called before the first frame update
    void Start()
    {
        //TODO: pick random gun from the list to contain and make the outside match accordingly
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            collision.GetComponentInChildren<ArmsScript>().currentWeapon = gun;
            Destroy(gameObject, 0.1f);
        }

    }


}
