using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    public int playerID { get; set; }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.gameObject.layer != LayerMask.NameToLayer("NonBulletCollide"))
        {
            if (collision.collider.tag != "Bullet" || collision.collider.GetComponent<Bullet>().playerID != playerID)
            {
                Destroy(gameObject);
            }
        }
    }

}
