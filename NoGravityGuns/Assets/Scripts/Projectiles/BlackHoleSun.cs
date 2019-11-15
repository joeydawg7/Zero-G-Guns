using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHoleSun : MonoBehaviour, IPooledObject
{
    protected float damage;

    [HideInInspector]
    public PlayerScript player;

    protected bool canImapact;


    protected Rigidbody2D rb;
    protected Vector2 startingForce;
    protected ObjectPooler objectPooler;
    protected SpriteRenderer sr;
    protected ParticleSystem bulletTrail;

    protected AudioClip shrinkSound;

    protected Guns gun;

    protected string layer;

    private bool hitTarget;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();        
    }

    //interface override what to set when the object is spawned
    public void OnObjectSpawn()
    {
        canImapact = false;
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = new Vector2(0, 0);
        rb.angularVelocity = 0;
        rb.simulated = false;
        objectPooler = ObjectPooler.Instance;        
    }

    protected void SetStartingForce(Vector2 vel)
    {
        startingForce = new Vector2(vel.x, vel.y);
    }

    public virtual void Construct(float damage, PlayerScript player, Vector3 dir, Color32 color)
    {
        
        //who shot the bullet
        this.player = player;

        //how much it will hurt
        this.damage = damage;
        //what gun shot it
        this.gun = player.armsScript.currentWeapon;

        gameObject.layer = player.collisionLayer;

        //sr.sprite = sprite;
        //sr.color = color;

        //stuff to ignore (kind of legacy)
        foreach (var collider in player.GetComponentsInChildren<Collider2D>())
        {
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), collider, true);
        }

        rb.simulated = true;
        rb.isKinematic = false;
        GetComponent<Collider2D>().enabled = true;

        SetStartingForce(dir);

        canImapact = true;

        rb.AddForce(dir, ForceMode2D.Force);
        StartCoroutine(CollapseSun());

        SetPFXTrail("RocketTrail", false);

        //Vector2 directionOfMovement = rb.velocity;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }


    protected IEnumerator CollapseSun()
    {
        float timer = 0;
        while(timer < 1.00f)
        {
            this.gameObject.transform.localScale = this.gameObject.transform.localScale - new Vector3(Time.deltaTime, Time.deltaTime, Time.deltaTime);
            if(hitTarget)
            {
                this.gameObject.transform.localScale = Vector3.zero;
                break;
            }
           
            yield return null;
            timer += Time.deltaTime;                  
        }        
            MakeBlackHole();              
    }
    protected virtual void SetPFXTrail(string effectTag, bool setToPlayerColor)
    {
        //replace bullet trail if one exists
        if (bulletTrail != null)
        {
            bulletTrail.Stop();
            bulletTrail.GetComponent<DisableOverTime>().DisableOverT(0.0f);
            bulletTrail.transform.parent = null;
        }

        GameObject temp2 = objectPooler.SpawnFromPool(effectTag, gameObject.transform.position, Quaternion.identity);
        bulletTrail = temp2.GetComponent<ParticleSystem>();

        var main = bulletTrail.main;
        main.startColor = new ParticleSystem.MinMaxGradient(player.playerColor);

        bulletTrail.transform.parent = transform;
        bulletTrail.Play(true);

    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.collider.gameObject.layer != LayerMask.NameToLayer("NonBulletCollide") && canImapact == true)
        {
            //we've hit something that isnt a bullet, or the player that shot the original bullet
            if (collision.collider.tag != "Bullet" || collision.collider.GetComponent<Bullet>().player.playerID != player.playerID)
            {

                ExplosiveObjectScript explosiveObjectScript = collision.collider.gameObject.GetComponent<ExplosiveObjectScript>();

                if (explosiveObjectScript != null)
                {
                    if (explosiveObjectScript != null && damage > 0)
                    {
                        explosiveObjectScript.DamageExplosiveObject(damage, player);
                    }
                }

                //default damage type is nothing, we don't know what we hit yet.
                PlayerScript.DamageType dmgType = DamageBodyParts(collision);
                SpawnSparkEffect();

                canImapact = false;
                hitTarget = true;
            }
        }
    }

    protected virtual void SpawnSparkEffect()
    {
        //spawn some impact sparks from pool
        GameObject sparkyObj = objectPooler.SpawnFromPool("BulletImpact", transform.position, Quaternion.identity);
        sparkyObj.GetComponent<ParticleSystem>().Emit(10);
        //start a timer to kill it
        sparkyObj.GetComponent<DisableOverTime>().DisableOverT(2f);
    }

    protected virtual PlayerScript.DamageType DamageBodyParts(Collision2D collision)
    {
        //default damage type is nothing, we don't know what we hit yet.
        PlayerScript.DamageType dmgType = PlayerScript.DamageType.self;

        //we can get out of here early if there is no player script component on the root parent of whatever we hit, because that 100% is not a player :D
        PlayerScript hitPlayerScript = collision.transform.root.GetComponentInChildren<PlayerScript>();
        if (hitPlayerScript == null)
            return dmgType;

        //checks where we hit the other guy, deals our given damage to that location. 

        dmgType = PlayerScript.ParsePlayerDamage(collision.gameObject);

        if (dmgType != PlayerScript.DamageType.self)
        {
            hitPlayerScript.TakeDamage(damage, startingForce, dmgType, this.player, true, gun);
            // collision.transform.GetComponentInChildren<ParticleSystem>().Emit(30);
            GetComponent<Collider2D>().enabled = false;
        }
        //if (collision.collider.tag == "Torso")
        //{
        //    dmgType = PlayerScript.DamageType.torso;
        //    hitPlayerScript.TakeDamage(damage, startingForce, dmgType, this.player, true);
        //   // collision.transform.GetComponentInChildren<ParticleSystem>().Emit(30);
        //    GetComponent<Collider2D>().enabled = false;
        //}
        //if (collision.collider.tag == "Head")
        //{
        //    dmgType = PlayerScript.DamageType.head;
        //    hitPlayerScript.TakeDamage(damage, startingForce, dmgType, this.player, true);
        //    GetComponent<Collider2D>().enabled = false;
        //}
        //if (collision.collider.tag == "Feet")
        //{
        //    dmgType = PlayerScript.DamageType.feet;
        //    hitPlayerScript.TakeDamage(damage, startingForce, dmgType, this.player, true);
        //    GetComponent<Collider2D>().enabled = false;
        //}
        //if (collision.collider.tag == "Leg")
        //{
        //    dmgType = PlayerScript.DamageType.legs;
        //    hitPlayerScript.TakeDamage(damage, startingForce, dmgType, this.player, true);
        //    GetComponent<Collider2D>().enabled = false;
        //}

        return dmgType;
    }

    //gets rid of a blackHoleSun gracefully
    protected virtual void MakeBlackHole()
    {        
        GameObject blackHole = ObjectPooler.Instance.SpawnFromPool("BlackHole", this.gameObject.transform.position, Quaternion.identity);
        blackHole.GetComponent<BlackHole>().Construct(player, player.armsScript.currentWeapon);
        //blackHole.transform.parent = null;
        //blackHole.transform.localScale = new Vector3(5.0f, 5.0f, 5.0f);
        StartCoroutine(DisableOverTime(0.0f));

        //this.StopAllCoroutines();

        bulletTrail.transform.parent = null;
        bulletTrail.Stop();
        bulletTrail.GetComponent<DisableOverTime>().DisableOverT(3.1f);
    }

    protected virtual IEnumerator DisableOverTime(float t)
    {
        yield return new WaitForSeconds(t);
        gameObject.SetActive(false);
        rb.velocity = new Vector2(0, 0);
        rb.angularVelocity = 0;
        rb.simulated = false;

    }


}