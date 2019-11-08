using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHole : MonoBehaviour
{
    private float scale;
    public float minDamage, maxDamage;

    private PlayerScript playerWhoShot;
    private Guns gunThatShot;
    private bool shrinkingPlayer;

    public AudioClip blackHoleForming, blackHoleCollapsing;
    public AudioClip blackHoleTeleport;
    private AudioSource theSource;
    
    // Start is called before the first frame update

    public virtual void Construct(PlayerScript player, Guns theGun)
    {
        playerWhoShot = player;
        gunThatShot = theGun;

        theSource = this.gameObject.GetComponent<AudioSource>();
        scale = 0.0f;
        this.gameObject.transform.localScale = Vector3.zero;
        shrinkingPlayer = false;
        StartCoroutine(GrowBlackHole());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator GrowBlackHole()
    {
        float timer = 0;
        if(blackHoleForming)
        {
            theSource.PlayOneShot(blackHoleForming);
        }
        
        while(this.gameObject.transform.localScale.x < 2.0f)
        {
            scale += Time.deltaTime * 2.0f;
            this.gameObject.transform.localScale = new Vector3(scale, scale, scale);
            yield return null;
        }
        while(timer < 3.0f)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        StartCoroutine(ShrinkBlackHole());
    }

    public IEnumerator ShrinkBlackHole()
    {
        if (blackHoleCollapsing)
        {
            theSource.PlayOneShot(blackHoleCollapsing);
        }

        while (this.gameObject.transform.localScale.x > 0.0f)
        {
            scale -= Time.deltaTime * 2.0f;
            this.gameObject.transform.localScale -= new Vector3(scale, scale, scale);
            yield return null;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if((collision.tag == "Arms" || collision.tag == "Leg" || collision.tag == "Torso" || collision.tag == "Head" || collision.tag == "Feet"))
        {
            if(!shrinkingPlayer)
            {                
                var player = collision.gameObject.GetComponent<PlayerScript>();                
                if(!player.isDead)
                {                    
                    StartCoroutine(ShrinkPlayer(player.gameObject, player.gameObject.transform.localScale));
                }               
            }          
        }
        else if(collision.gameObject.GetComponent<Rigidbody2D>() && collision.tag != "BlackHoleSun")
        {
           
            StartCoroutine(ShrinkPlayer(collision.gameObject, collision.gameObject.transform.localScale));
        }
    }

    public IEnumerator ShrinkPlayer(GameObject player, Vector3 oldScale)
    {
        shrinkingPlayer = true;
        var start = new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z);
        
        for(int i =0; i < 15.0f; i++)
        {
            player.transform.position = Vector3.Lerp(start, this.gameObject.transform.position, i / 15.0f);
            player.transform.Rotate(Vector3.back, 10.0f);
            yield return null;
        }
        
        while(player.transform.localScale.x > 0.0f)
        {
            player.transform.localScale -= new Vector3(Time.deltaTime * 2.0f, Time.deltaTime * 2.0f, Time.deltaTime * 2.0f);
            yield return null;
        }
        var teleportLocation = new Vector3(Random.Range(-50.0f, 50.0f), Random.Range(-50.0f, 50.0f), player.transform.position.z);
        //add in a partial affect to show where you will spawn.
        yield return new WaitForSeconds(0.125f);
        Teleport(player, oldScale, teleportLocation);
    }
    
    public void Teleport(GameObject player, Vector3 oldScale, Vector3 teleportLocation)
    {
        if(blackHoleTeleport)
        {
            theSource.PlayOneShot(blackHoleTeleport);
        }
        player.transform.position = teleportLocation;
        if(player.GetComponent<PlayerScript>())
        {
            player.GetComponent<PlayerScript>().TakeDamage(Random.Range(minDamage, maxDamage), Vector2.zero, PlayerScript.DamageType.blackhole, playerWhoShot, false, gunThatShot);
        }
        player.transform.localScale = oldScale;
        oldScale = Vector3.zero;
        shrinkingPlayer = false;   
    }
}
