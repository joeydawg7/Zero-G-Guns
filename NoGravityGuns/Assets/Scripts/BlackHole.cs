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


    GameObject cameraParent;
    RippleController rippleController;

    BlackHoleColourController theWornHole;
    // Start is called before the first frame update

    public virtual void Construct(PlayerScript player, Guns theGun)
    {
        cameraParent = Camera.main.transform.parent.gameObject;
        rippleController = cameraParent.GetComponentInChildren<RippleController>();
        playerWhoShot = player;
        gunThatShot = theGun;

        theSource = this.gameObject.GetComponent<AudioSource>();
        scale = 0.0f;
        this.gameObject.transform.localScale = Vector3.zero;
        shrinkingPlayer = false;
        theWornHole = this.gameObject.GetComponentInChildren<BlackHoleColourController>();
        theWornHole.SetBlackHoleColour(player);
        StartCoroutine(GrowBlackHole());        
    }

    // Update is called once per frame
    void Update()
    {
        theWornHole.gameObject.transform.Rotate(new Vector3(0.0f, 0.0f, -1.0f), 360 * Time.deltaTime);
    }

    public IEnumerator GrowBlackHole()
    {
        float timer = 0;
        if(blackHoleForming)
        {
            theSource.PlayOneShot(blackHoleForming);
        }

        Vector2 rippleVector = new Vector2(transform.position.x, transform.position.z);

        rippleController.Ripple(rippleVector);

        Debug.Log("is this being called mutliple times?!");

        while(this.gameObject.transform.localScale.x < 2.0f)
        {
            scale += Time.deltaTime * 2.0f;
            this.gameObject.transform.localScale = new Vector3(scale, scale, scale);
            yield return null;
        }
        while(timer < 3.0f)
        {
            if(!shrinkingPlayer)
            {
                timer += Time.deltaTime;
            }            
            yield return null;
        }
        StartCoroutine(ShrinkBlackHole());
    }

    bool isHoleCollapsing = false;

    public IEnumerator ShrinkBlackHole()
    {
        isHoleCollapsing = true;

        if (blackHoleCollapsing)
        {
            theSource.PlayOneShot(blackHoleCollapsing);
        }

       // rippleController.Ripple(transform.position);
        float progression = 0.0f;
        while (progression < 1.0f)
        {
            progression += Time.deltaTime;
            this.transform.localScale = Vector3.Lerp(new Vector3(2.0f, 2.0f, 2.0f), Vector3.zero, progression);          
            yield return null;
        }
        this.transform.localScale = Vector3.zero;

        while(shrinkingPlayer)
        {
            yield return null;

        }
        gameObject.SetActive(false);
        transform.parent = null;
        isHoleCollapsing = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if ((collision.gameObject.tag == "Arms" || collision.gameObject.tag == "Leg" || collision.gameObject.tag == "Torso" || collision.gameObject.tag == "Head" || collision.gameObject.tag == "Feet"))
        {
            if (!shrinkingPlayer)
            {
                shrinkingPlayer = true;
                var player = collision.gameObject.GetComponent<PlayerScript>();
                if (player)
                {
                    if (!player.isDead)
                    {
                        StartCoroutine(ShrinkPlayer(collision.gameObject, player.gameObject.transform.localScale));
                    }
                    else
                    {
                        shrinkingPlayer = false;
                    }
                }
                else
                {
                    shrinkingPlayer = false;
                }
            }
            Physics2D.IgnoreCollision(this.gameObject.GetComponent<Collider2D>(), collision.gameObject.GetComponent<Collider2D>(), true);
        }
        else if (collision.gameObject.GetComponent<Rigidbody2D>() && collision.gameObject.tag != "BlackHoleSun" && !collision.gameObject.GetComponent<WheelJoint2D>() && collision.gameObject.tag != "BlackHole")
        {
            StartCoroutine(ShrinkPlayer(collision.gameObject, collision.gameObject.transform.localScale));
            Physics2D.IgnoreCollision(this.gameObject.GetComponent<Collider2D>(), collision.gameObject.GetComponent<Collider2D>(), true);
        }
    }

    public IEnumerator ShrinkPlayer(GameObject player, Vector3 oldScale)
    {
        shrinkingPlayer = true;
        var start = new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z);
        var rb = player.gameObject.GetComponent<Rigidbody2D>();
        rb.velocity = Vector3.zero;
        rb.angularVelocity = 0.0f;
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
        yield return new WaitForSeconds(0.065f);        
        cameraParent.GetComponentInChildren<RippleController>().Ripple(teleportLocation, 15, 0.88f);         
        yield return new WaitForSeconds(0.065f);
        Teleport(player, oldScale, teleportLocation);
    }
    
    public void Teleport(GameObject player, Vector3 oldScale, Vector3 teleportLocation)
    {
        if(blackHoleTeleport)
        {
            theSource.PlayOneShot(blackHoleTeleport);
        }
        player.transform.position = teleportLocation;
        var rb = player.gameObject.GetComponent<Rigidbody2D>();
        rb.velocity = Vector3.zero;
        rb.angularVelocity = 0.0f;
        if (player.GetComponent<PlayerScript>())
        {
            player.GetComponent<PlayerScript>().TakeDamage(Random.Range(minDamage, maxDamage), Vector2.zero, PlayerScript.DamageType.blackhole, playerWhoShot, false, gunThatShot);
        }
        player.transform.localScale = oldScale;
        //player.transform.parent = null;
        oldScale = Vector3.zero;
        shrinkingPlayer = false;   
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        var rb = this.gameObject.GetComponent<Rigidbody2D>();
        if(Mathf.Abs(rb.velocity.x) > 2.0f)
        {
            if (rb.velocity.x > 0)
            {
                rb.velocity = new Vector2(2.0f, rb.velocity.y); 
            }
            else
            {
                rb.velocity = new Vector2(-2.0f, rb.velocity.y);
            }
        }
        if (Mathf.Abs(rb.velocity.y) > 2.0f)
        {
            if (rb.velocity.y > 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, 2.0f);
            }
            else
            {
                rb.velocity = new Vector2(rb.velocity.x, -2.0f);
            }
        }

    }
}
