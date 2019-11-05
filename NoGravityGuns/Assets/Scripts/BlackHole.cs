using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHole : MonoBehaviour
{
    private float scale;
    public float minDamage, maxDamage;

    private PlayerScript playerWhoShot;
    private Guns gunThatShot;
    // Start is called before the first frame update

    public virtual void Construct(PlayerScript player, Guns theGun)
    {
        playerWhoShot = player;
        gunThatShot = theGun;

        scale = 0.0f;
        this.gameObject.transform.localScale = Vector3.zero;
        StartCoroutine(GrowBlackHole());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator GrowBlackHole()
    {
        while(this.gameObject.transform.localScale.x < 1.0f)
        {
            scale += Time.deltaTime;
            this.gameObject.transform.localScale = new Vector3(scale, scale, scale);
            yield return null;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            var player = collision.gameObject.GetComponent<PlayerScript>();
            player.TakeDamage(Random.Range(minDamage, maxDamage), Vector2.zero, PlayerScript.DamageType.blackhole, playerWhoShot, false, gunThatShot);
        }
    }
}
