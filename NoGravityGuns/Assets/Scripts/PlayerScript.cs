using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.PlayerInput;
using UnityEngine.InputSystem.Users;
using Rewired;

public class PlayerScript : MonoBehaviour
{
    //Variables
    #region publics

    [Header("Health and Lives")]
    public int health;
    public int numLives;

    public PlayerCanvasScript playerCanvasScript;
    [HideInInspector]
    public Color32 playerColor;
    [HideInInspector]
    public Color32 deadColor;
    [HideInInspector]
    public int collisionLayer;

    [HideInInspector]
    public int playerID;
    public Player player;
    public Controller controller;


    public enum DamageType { head = 4, torso = 3, legs = 2, feet = 1, self = 5, explosive = 6 };

    [Header("Bools")]
    public bool isDead;
    public bool isInvulnerable;

    public ArmsScript armsScript;

    public HingeJoint2D leftLegHinge;
    public HingeJoint2D rightLegHinge;

    [Header("Spawning and kills")]
    public Vector3 spawnPoint;
    public Color32 invulnerabilityColorFlash;
    public float invulnerablityTime;
    [HideInInspector]
    public int numKills;
    public PlayerScript playerLastHitBy;
    int _roundWins;
    public int roundWins
    {
        set { _roundWins = value; }
        get { return _roundWins; }
    }

    //[Header("Particle Effects")]
    //public ParticleSystem HS_Flash;
    //public ParticleSystem HS_Streaks;
    //public ParticleSystem respawnFlash;
    //public ParticleSystem respawnBits;
    //TrailRenderer trail;
    #endregion
    #region Audio

    [HideInInspector]
    public AudioSource audioSource;
    [Header("Audio")]
    public AudioClip headShot;
    public List<AudioClip> standardShots;
    public AudioClip torsoImpact;
    public AudioClip legsImpact;
    public AudioClip headImpact;
    public AudioClip deathClip;
    public AudioClip respawnClip;
    #endregion
    #region hidden publics
    [HideInInspector]
    public string playerName;
    [HideInInspector]
    public string hexColorCode;
    [HideInInspector]
    public Rigidbody2D rb;
    [HideInInspector]
    public PlayerInput playerInput;
    [HideInInspector]
    public DamageType lastHitDamageType;
    #endregion
    #region privates
    //Private
    Quaternion targetRotation;
    Color32 defaultColor;
    float angle;
    float immuneToCollisionsTimer;
    SpriteRenderer[] legsSR;
    SpriteRenderer torsoSR;
    SpriteRenderer[] armsSR;
    Rigidbody2D[] legRBs;
    GameObject cameraParent;
    Quaternion spawnRotation;
    GameManager gameManager;
    #endregion
    #region constants
    const float HEADSHOT_MULTIPLIER = 2f;
    const float TORSOSHOT_MULTIPLIER = 1f;
    const float FOOTSHOT_MULTIPLIER = 0.5f;
    const float LEGSHOT_MULTIPLIER = 0.75f;
    const float EXPLOSION_MULTIPLIER = 0.5f;
    const int COLLIDER_DAMAGE_MITIGATOR = 5;
    #endregion
    //End Variables

    #region Awake, Update, Start
    private void Awake()
    {
        playerID = 0;

        health = 100;
        float barVal = ((float)health / 100f);
        isDead = false;
        spawnPoint = transform.position;
        spawnRotation = transform.rotation;

        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        playerInput = GetComponent<PlayerInput>();
        numKills = 0;

        //legFixers = new List<LegFixer>();
        //foreach (Transform child in legsParent)
        //{
        //    legFixers.Add(child.GetComponent<LegFixer>());
        //}
        torsoSR = gameObject.transform.root.GetComponent<SpriteRenderer>();
        armsSR = gameObject.GetComponentsInChildren<SpriteRenderer>();
        //torsoSR.color = playerColor;
        //defaultColor = torsoSR.color;

        //foreach (var SR in armsSR)
        //{
        //    SR.color = playerColor;
        //}

        playerLastHitBy = null;
        immuneToCollisionsTimer = 0;

        cameraParent = Camera.main.transform.parent.gameObject;

        armsScript.basePlayer = this;

        //rail = GetComponent<TrailRenderer>();
        //trail.emitting = false;
        gameManager = GameManager.Instance;

        lastHitDamageType = DamageType.self;


    }

    private void Start()
    {
        rb.simulated = true;
    }

    private void Update()
    {

        if (GameManager.Instance.isGameStarted)
        {
            immuneToCollisionsTimer += Time.deltaTime;

        }

        //DEBUG: take damage to torso
        if (Input.GetKeyDown(KeyCode.K) && GameManager.Instance.debugManager.useDebugSettings)
            TakeDamage(50, new Vector2(0, 0), DamageType.torso, null, true);

        if (gameManager.isGameStarted)
        {
            if (!isDead)
            {

                //B button
                OnDrop();

                //A button
                OnFlail();

            }

            OnPause();


        }


    }
    #endregion

    #region Input Handler Functions
    public void OnDrop()
    {
        if (player.GetButtonDown("Drop") && armsScript.currentWeapon.name != "Pistol")
        {
            armsScript.EquipGun(GameManager.Instance.pistol);
        }
    }

    void OnFlail()
    {

        StartCoroutine(FlailLegs());

    }

    IEnumerator FlailLegs()
    {
        while (player.GetButton("Join"))
        {
            
            yield return null;
            
        }

    }

    void OnPause()
    {

        if (gameManager.isGameStarted && player.GetButtonDown("Start"))
        {

            Debug.Log(gameObject.name + " tried to pause");

            if (Time.timeScale > 0)
                Time.timeScale = 0;
            else
                Time.timeScale = 1;
            Debug.Log("timescale = " + Time.timeScale);
        }

    }
    #endregion

    #region Take Damage
    public void TakeDamage(float damage, Vector2 dir, DamageType damageType, PlayerScript PlayerWhoShotYou, bool playBulletSFX)
    {
        if (!isDead && !isInvulnerable)
        {


            //only reset if it wasnt a world kill
            if (PlayerWhoShotYou != null)
            {
                playerLastHitBy = PlayerWhoShotYou;
                //PlayerWhoShotYou.shotsHit++;
            }
            else
                playerLastHitBy = null;

            float unModdedDmg = damage;


            if (damage < 0)
            {
                SpawnFloatingDamageText(Mathf.RoundToInt(damage), DamageType.torso, "FloatAway");
                //Color.Green
            }
            else
            {
                //temp damage mod
                damage *= 1.2f;

                //TODO: amplify pushback from bullets direction
                //rb.AddForce(transform.right * dir * 0.01f, ForceMode2D.Impulse);

                switch (damageType)
                {
                    case DamageType.head:
                        damage *= HEADSHOT_MULTIPLIER;
                        SpawnFloatingDamageText(Mathf.RoundToInt(damage), DamageType.head, "Crit");
                        //Color.Red
                        Debug.Log("playing hs flash");
                        // HS_Flash.Play();
                        // HS_Streaks.Play();
                        //HS_Flash.Emit(Random.Range(35, 45));
                        if (playBulletSFX)
                            audioSource.PlayOneShot(headShot);
                        break;
                    case DamageType.torso:
                        damage *= TORSOSHOT_MULTIPLIER;
                        SpawnFloatingDamageText(Mathf.RoundToInt(damage), DamageType.torso, "FloatAway");
                        //Color.yellow
                        break;
                    case DamageType.legs:
                        damage *= LEGSHOT_MULTIPLIER;
                        SpawnFloatingDamageText(Mathf.RoundToInt(damage), DamageType.legs, "FloatAway");
                        // Color.black
                        break;
                    case DamageType.feet:
                        damage *= FOOTSHOT_MULTIPLIER;
                        SpawnFloatingDamageText(Mathf.RoundToInt(damage), DamageType.feet, "FloatAway");
                        //Color.gray
                        break;
                    case DamageType.explosive:
                        damage *= EXPLOSION_MULTIPLIER;
                        SpawnFloatingDamageText(Mathf.RoundToInt(damage), DamageType.explosive, "FloatAway");
                        break;
                    default:
                        break;
                }

                if (damageType != DamageType.head && playBulletSFX)
                    audioSource.PlayOneShot(standardShots[Random.Range(0, standardShots.Count)]);
            }

            health -= (int)damage;
            health = Mathf.Clamp(health, 0, 100);
            float barVal = ((float)health / 100f);
            playerCanvasScript.setHealth(barVal);

            if (health <= 0)
            {
                //add a kill to whoever shot you, as long as its not you
                if (playerLastHitBy != null && playerLastHitBy != this)
                {
                    playerLastHitBy.numKills++;
                    //playerLastHitBy.playerUIPanel.SetKills(playerLastHitBy.numKills);
                    //playerLastHitBy.playerUIPanel.AddKill(this);
                }
                //reduce points if you kill yourself
                else if (playerLastHitBy != null && playerLastHitBy == this)
                {
                    playerLastHitBy.numKills--;
                    //playerLastHitBy.playerUIPanel.AddKill(this);
                }

                Die(damageType);
            }

        }
    }


    #endregion

    #region Die and respawn
    public PlayerScript Die(DamageType damageType)
    {
        //cant die if yer dead
        if (!isDead)
        {
            //stop all other audio, death sfx is more important
            audioSource.Stop();
            audioSource.PlayOneShot(deathClip);

            //yeah you dead boy
            isDead = true;

            numLives--;

            

            //playerUIPanel.LoseStock();

            if (numLives <= 0)
            {
                //playerUIPanel.Disable();

                //playerCanvasScript.Destroy();
                GameManager.Instance.CheckForLastManStanding(transform, damageType);

            }
            // armsSR = armsScript.currentArms.GetComponent<SpriteRenderer>();
            
            torsoSR.color = deadColor;
           
            
            foreach (var SR in armsSR)
            {
                SR.color = deadColor;
            }
            //armsSR.color = deadColor;

            //foreach (var sr in legsSR)
            //{
            //    sr.color = deadColor;
            //}
            //armsScript.reloadTimer.SetActive(false);

            if (numLives > 0)
                StartCoroutine(WaitForRespawn());
            lastHitDamageType = damageType;
            playerCanvasScript.gameObject.SetActive(false);
        }

        return this;
    }

    IEnumerator WaitForRespawn()
    {
        lastHitDamageType = DamageType.self;
        //playerUIPanel.SetAmmoText("3...", 1);
        yield return new WaitForSeconds(1f);
        //playerUIPanel.SetAmmoText("2...", 1);
        yield return new WaitForSeconds(1f);
        // playerUIPanel.SetAmmoText("1...", 1);
        yield return new WaitForSeconds(1f);

        //turn of rigidbody so we dont get some crazy momentum from force moving
        rb.isKinematic = true;
        transform.position = spawnPoint;
        transform.rotation = spawnRotation;
        rb.isKinematic = false;
        
        //emit those PFX
        //var mainFlash = respawnFlash.main;
        //var mainBits = respawnBits.main;
        //Color c = playerColor;
        //mainFlash.startColor = c;
        //mainBits.startColor = c;
        //respawnFlash.Emit(1);
        //respawnBits.Emit(Random.Range(15, 30));

        ForcePushOnSpawn();

        //foreach (var legToFix in legFixers)
        //{
        //    legToFix.ResetLeg();
        //}

        health = 100;
        float barVal = ((float)health / 100f);
        audioSource.PlayOneShot(respawnClip);

        playerCanvasScript.gameObject.SetActive(true);
        playerCanvasScript.setHealth(barVal);


        isDead = false;
        //last thing you were hit by set back to world, just in case you suicide without help
        playerLastHitBy = null;

        armsScript.EquipGun(GameManager.Instance.pistol);
        armsScript.currentAmmo = armsScript.currentWeapon.clipSize;

        if (numLives <= 0)
        {
            gameObject.SetActive(false);
            yield break;
        }

        rb.rotation = 0;
        rb.velocity = new Vector2(0, 0);
        rb.angularVelocity = 0;

        foreach (var rb in legRBs)
        {
            rb.angularVelocity = 0;
            rb.velocity = new Vector2(0, 0);
            rb.rotation = 0;
            rb.gameObject.transform.localPosition = Vector3.zero;

        }

        //armsScript.SendGunText();

        StartCoroutine(RespawnInvulernability());

    }

    IEnumerator RespawnInvulernability()
    {
        isInvulnerable = true;
        //armsSR = armsScript.currentArms.GetComponent<SpriteRenderer>();

        float invulnerabilityFlashIncriments = (float)invulnerablityTime / 12f;

        for (int i = 0; i < invulnerablityTime; i++)
        {
            torsoSR.color = invulnerabilityColorFlash;
            foreach (var SR in armsSR)
            {
                SR.color = invulnerabilityColorFlash;
            }
            //foreach (var sr in legsSR)
            //{
            //    sr.color = invulnerabilityColorFlash;
            //}

            yield return new WaitForSeconds(invulnerabilityFlashIncriments);
            torsoSR.color = defaultColor;
            foreach (var SR in armsSR)
            {
                SR.color = defaultColor;
            }
            //foreach (var sr in legsSR)
            //{
            //    sr.color = defaultColor;
            //}

            yield return new WaitForSeconds(invulnerabilityFlashIncriments);
            torsoSR.color = invulnerabilityColorFlash;
            foreach (var SR in armsSR)
            {
                SR.color = invulnerabilityColorFlash;
            }
            //foreach (var sr in legsSR)
            //{
            //    sr.color = invulnerabilityColorFlash;
            //}

            yield return new WaitForSeconds(invulnerabilityFlashIncriments);
            torsoSR.color = defaultColor;
            foreach (var SR in armsSR)
            {
                SR.color = defaultColor;
            }
            //foreach (var sr in legsSR)
            //{
            //    sr.color = defaultColor;
            //}
        }

        torsoSR.color = defaultColor;
        foreach (var SR in armsSR)
        {
            SR.color = defaultColor;
        }
        //foreach (var sr in legsSR)
        //{
        //    sr.color = defaultColor;
        //}

        isInvulnerable = false;
    }

    private void ForcePushOnSpawn()
    {
        int power = 100;
        int radius = 15;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius);
        foreach (Collider2D hit in colliders)
        {
            Rigidbody2D rb = hit.GetComponent<Rigidbody2D>();

            if (rb == null && hit.transform.root.GetComponent<Rigidbody2D>() != null)
            {
                rb = hit.transform.root.GetComponent<Rigidbody2D>();
            }

            if (rb != null)
            {

                //treat players different from other objects
                if (rb.tag == "Player")
                {
                    //note that this force is only applied to players torso... trying to add more than that caused some crazy effects for little gains in overall usefulness
                    Rigidbody2DExt.AddExplosionForce(rb, power, transform.position, radius, ForceMode2D.Force);
                }
                //give impact objects a bit more push than other things
                else if (rb.tag != "Bullet" || rb.tag != "RedBullet" || rb.tag != "BlueBullet" ||
           rb.tag != "YellowBullet" || rb.tag != "GreenBullet")
                {
                    Rigidbody2DExt.AddExplosionForce(rb, power, transform.position, radius, ForceMode2D.Force);
                }
                else
                {
                    Rigidbody2DExt.AddExplosionForce(rb, power, transform.position, radius, ForceMode2D.Force);
                }
            }
        }

    }
    #endregion

    #region Controller Setting / unsetting, OnStart
    public void SetController(int number, Controller controller)
    {
        this.controller = controller;
        playerID = number;

        player = ReInput.players.GetPlayer(playerID);
        player.controllers.AddController(controller, true);

        Debug.Log(player.descriptiveName);
        Debug.Log(controller.name);

        //player.controllers.maps.SetMapsEnabled(true, "Gameplay");
        //player.controllers.maps.SetMapsEnabled(false, "UI");

        armsScript.basePlayer = this;

    }

    public void AddRoundWin()
    {
        roundWins++;
    }

    public void UnsetController()
    {
        this.controller = null;
        player = null;
        playerID = 0;
        playerName = "";
    }

    public void OnGameStart()
    {
        health = 100;
        float barVal = ((float)health / 100f);
        playerCanvasScript.setHealth(barVal);
        isDead = false;
        numKills = 0;

        armsScript.EquipGun(GameManager.Instance.pistol);

        //torsoSR = GetComponent<SpriteRenderer>();
        //armsSR = armsScript.currentArms.GetComponent<SpriteRenderer>();
        //legsSR = legsParent.GetComponentsInChildren<SpriteRenderer>();


        //legRBs = legsParent.GetComponentsInChildren<Rigidbody2D>();


        // playerUIPanel.SetLives(numLives, playerHead);
        player.controllers.AddController(controller, true);


        rb.simulated = true;


        //RoundManager.Instance.SetPlayer(this);

        StartCoroutine(RespawnInvulernability());

    }
    #endregion

    //collision check and damage mutlipliers / modifiers
    #region collision Damage
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("hit something!");
        if (collision.collider.tag == "ImpactObject" || collision.collider.tag == "ExplosiveObject" || collision.collider.tag == "Chunk")
        {
            DealColliderDamage(collision, gameObject, null);
        }
        else if ((collision.collider.tag == "Torso" && collision.gameObject != this.gameObject) || (collision.collider.tag == "Head" && collision.gameObject != this.gameObject)
            || (collision.collider.tag == "Feet" && collision.gameObject != this.gameObject) || (collision.collider.tag == "Legs" && collision.gameObject != this.gameObject))
        {
            PlayerScript hitBy = collision.transform.root.GetComponentInChildren<PlayerScript>();
            DealColliderDamage(collision, gameObject, hitBy);
        }

    }

    public static DamageType ParsePlayerDamage(GameObject hitObject)
    {
        DamageType damageType = DamageType.self;

        string tag = hitObject.tag;

        switch (tag)
        {
            case "Head":
                damageType = DamageType.head;
                break;
            case "Torso":
                damageType = DamageType.torso;
                break;
            case "Leg":
                damageType = DamageType.legs;
                break;
            case "Feet":
                damageType = DamageType.feet;
                break;
            default:
                damageType = DamageType.self;
                break;
        }

        return damageType;
    }

    public void DealColliderDamage(Collision2D collision, GameObject hitLocation, PlayerScript hitBy)
    {
        //float dmg = collision.relativeVelocity.magnitude;

        //float dmg = collision.relativeVelocity.normalized.magnitude;
        //tor3.Dot(col.contacts[0].normal,col.relativeVelocity) * rigidbody.mass

        float dmg = Vector3.Dot(collision.contacts[0].normal, collision.relativeVelocity);

        if (collision.rigidbody != null && collision.rigidbody.mass <= 1)
            dmg *= collision.rigidbody.mass;

        //reduces damage so its not bullshit
        dmg = dmg / COLLIDER_DAMAGE_MITIGATOR;

        //dont bother dealing damage unless unmitigated damage indicates fast enough collision
        if (dmg > 25)
        {

            if (collision.rigidbody != null)
            {

                if (collision.rigidbody.isKinematic == false)
                    dmg *= collision.rigidbody.mass;
            }

            DamageType dmgType = PlayerScript.ParsePlayerDamage(hitLocation);

            AudioClip soundClipToPlay;

            if (dmgType == DamageType.legs || dmgType == DamageType.feet)
                soundClipToPlay = legsImpact;
            else
                soundClipToPlay = torsoImpact;

            Debug.Log(hitLocation.name + ": " + dmg);

            //caps damage
            if (dmg > 100)
                dmg = 100;

            if (immuneToCollisionsTimer >= 1)
            {
                immuneToCollisionsTimer = 0;
                audioSource.PlayOneShot(soundClipToPlay);
                TakeDamage(dmg, new Vector2(0, 0), dmgType, hitBy, false);
            }
        }
    }
    #endregion

    #region Floating Damage Text
    public struct FloatingDamageStuff
    {
        public readonly GameObject floatingDamageGameObject;
        public float damage;
        public float timer;
        public DamageType damageType;

        public FloatingDamageStuff(GameObject floatingDamage, float damage, DamageType damageType)
        {
            this.floatingDamageGameObject = floatingDamage;
            this.damage = damage;
            this.timer = 0;
            this.damageType = damageType;
        }
    }

    public FloatingDamageStuff floatingDamage;

    void SpawnFloatingDamageText(float dmgToShow, DamageType damageType, string animType)
    {
        Color32 color;

        dmgToShow = Mathf.RoundToInt(dmgToShow);

        if (dmgToShow == 0)
            return;

        switch (damageType)
        {
            case DamageType.self:
                color = Color.green;
                break;
            case DamageType.head:
                color = Color.red;
                break;
            case DamageType.torso:
                color = Color.yellow;
                break;
            case DamageType.legs:
                color = Color.black;
                break;
            case DamageType.feet:
                color = Color.grey;
                break;
            default:
                color = Color.yellow;
                break;
        }

        if (dmgToShow < 0)
            color = Color.green;

        //run a different function if we already have some floating text in existance, unless its a heal then treat it as new text
        if (floatingDamage.floatingDamageGameObject != null && dmgToShow > 0)
        {
            //we should be making new text if the current floaty text is a heal
            if (floatingDamage.damage > 0)
            {
                AddToFloatingDamage(dmgToShow, damageType, color, animType);
                return;
            }
        }

        //if we're not adding to an old damage text, we need to spawn a new one form a pool
        GameObject floatingTextGo = ObjectPooler.Instance.SpawnFromPool("FloatingText", playerCanvasScript.floatingDamageTextSpawnPoint.transform.position, Quaternion.identity,
            playerCanvasScript.floatingDamageTextSpawnPoint);
        floatingTextGo.SetActive(true);
        floatingTextGo.transform.localPosition = new Vector3(0, 0, 0);
        floatingTextGo.transform.localScale = new Vector3(1, 1, 1);
        TextMeshProUGUI floatTxt = floatingTextGo.GetComponent<TextMeshProUGUI>();

        floatingDamage = new FloatingDamageStuff(floatingTextGo, dmgToShow, damageType);

        //we did negative damage so show it as a heal
        if (dmgToShow < 0)
        {
            dmgToShow = Mathf.Abs(dmgToShow);
            floatTxt.text = "+" + dmgToShow.ToString();
        }
        else
            floatTxt.text = dmgToShow.ToString();

        //offset position to give a degree of motion to the thing
        floatingTextGo.transform.position = new Vector2(Random.Range(-1.5f, 1.5f), Random.Range(-1.5f, 1.5f));
        floatTxt.color = color;
        floatTxt.GetComponent<Animator>().SetTrigger(animType);

        //temp multiply damage by 2 to affect how big we scale it. trenchfoot math :D
        dmgToShow *= 2;

        //pretty sure this doesnt do anything because of how worldspace canvases work but im also kind of afraid to delete it?
        floatingTextGo.transform.localScale = new Vector3(floatingTextGo.transform.localScale.x * ((float)dmgToShow / 50f), floatingTextGo.transform.localScale.y * ((float)dmgToShow / 50f),
            floatingTextGo.transform.localScale.z * ((float)dmgToShow / 50f));
    }

    //adds the new damage value to our current floating damage and resets animation instead of stacking more text
    void AddToFloatingDamage(float dmg, DamageType damageType, Color color, string animType)
    {
        //add to damage to get new value
        floatingDamage.floatingDamageGameObject.GetComponent<TextMeshProUGUI>().text = (dmg + floatingDamage.damage).ToString();
        floatingDamage.damage = dmg + floatingDamage.damage;

        //change the damage type if its value is more... stacks from bottom up so a headshot is higher priority than foot, etc.
        //this way if multiple hits land on a target the most important hit determines the color of impact instead of it being 100% random :D
        if (damageType > floatingDamage.damageType)
            floatingDamage.damageType = damageType;

        switch (floatingDamage.damageType)
        {
            case DamageType.self:
                color = Color.green;
                break;
            case DamageType.head:
                color = Color.red;
                break;
            case DamageType.torso:
                color = Color.yellow;
                break;
            case DamageType.legs:
                color = Color.black;
                break;
            case DamageType.feet:
                color = Color.grey;
                break;
            default:
                color = Color.yellow;
                break;
        }

        //sets the color for real
        floatingDamage.floatingDamageGameObject.GetComponent<TextMeshProUGUI>().color = color;

        //offset position to give a degree of motion to the thing
        floatingDamage.floatingDamageGameObject.transform.position = new Vector2(Random.Range(-1.5f, 1.5f), Random.Range(-1.5f, 1.5f));
        floatingDamage.floatingDamageGameObject.GetComponent<Animator>().SetTrigger(animType);

    }
    #endregion

}
