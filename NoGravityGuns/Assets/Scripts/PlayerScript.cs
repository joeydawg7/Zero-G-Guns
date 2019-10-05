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
    [Header("Debug")]
    //If true treats the player as a dummy to be shot... for testing only :D
    public bool isDummy;

    [Header("Health and Lives")]
    public int health;
    public int numLives;

    [Header("Gui")]
    public PlayerUIPanel playerUIPanel;
    public Transform floatingTextSpawnPoint;
    public Color32 playerColor;
    public Color32 deadColor;
    //public Sprite playerHead;
    //public Sprite playerPortrait;
    public Sprite healthBar;
    public int collisionLayer;
    //public Sprite killTag;

    [Header("Controller Stuff")]
    [HideInInspector]
    public int playerID;
    public InputUser user;
    [HideInInspector]
    public Player player;
    public Controller controller;

    [HideInInspector]
    public enum DamageType { head = 4, torso = 3, legs = 2, feet = 1 , self = 5};
    [HideInInspector]
    //public enum GunType { pistol, assaultRifle, LMG, shotgun, railGun, healthPack, RPG, mineLauncher, collision };

    [Header("Bools")]
    public bool isDead;
    public bool isInvulnerable;

    public ArmsScript armsScript;

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

    [Header("Particle Effects")]
    public ParticleSystem HS_Flash;
    public ParticleSystem HS_Streaks;
    public ParticleSystem respawnFlash;
    public ParticleSystem respawnBits;
    TrailRenderer trail;
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
    const int COLLIDER_DAMAGE_MITIGATOR = 5;
    #endregion
    #region data collection

    [HideInInspector]
    public float pistolTime;
    [HideInInspector]
    public float rifleTime;
    [HideInInspector]
    public float shotgunTime;
    [HideInInspector]
    public float railgunTime;
    [HideInInspector]
    public float miniGunTime;

    [HideInInspector]
    public float pistolDmg;
    [HideInInspector]
    public float rifleDmg;
    [HideInInspector]
    public float shotgunDmg;
    [HideInInspector]
    public float railgunDmg;
    [HideInInspector]
    public float miniGunDmg;

    [HideInInspector]
    public float shotsFired;
    [HideInInspector]
    public float shotsHit;
    [HideInInspector]
    public float headShots;
    [HideInInspector]
    public float torsoShots;
    [HideInInspector]
    public float legShots;
    [HideInInspector]
    public float footShots;
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
        torsoSR.color = playerColor;
        defaultColor = torsoSR.color;

        foreach (var SR in armsSR)
        {
            SR.color = playerColor;
        }

        playerLastHitBy = null;
        immuneToCollisionsTimer = 0;

        cameraParent = Camera.main.transform.parent.gameObject;

        armsScript.basePlayer = this;

        //rail = GetComponent<TrailRenderer>();
        //trail.emitting = false;
        gameManager = GameManager.Instance;

        //data
        shotsFired = 0;
        shotsHit = 0;
        headShots = 0;
        torsoShots = 0;
        legShots = 0;
        footShots = 0;

        if (playerUIPanel == null)
            Debug.LogError("No UI Panel Set");

        if (floatingTextSpawnPoint == null)
            Debug.LogError("No floating text spawn point set!");
        lastHitDamageType = DamageType.self;

    }

    private void Start()
    {
        rb.simulated = false;
    }

    private void Update()
    {

        if (GameManager.Instance.isGameStarted)
        {
            immuneToCollisionsTimer += Time.deltaTime;

        }

        //DEBUG: take damage to torso
        if (Input.GetKeyDown(KeyCode.K))
            TakeDamage(50, DamageType.torso, null, true);

        if (!isDummy)
        {
            //B button
            if (GameManager.Instance.isGameStarted && armsScript.currentWeapon.name != "Pistol")
                OnDrop();

            //StartButton
            if (GameManager.Instance.isGameStarted)
                OnPause();
        }

    }
    #endregion

    #region Input Handler Functions
    public void OnDrop()
    {
        if (!isDummy && player.GetButtonDown("Drop"))
            armsScript.EquipGun(GameManager.Instance.pistol);
    }

    void OnPause()
    {

        if (player.GetButtonDown("Start"))
        {
            if (Time.timeScale > 0)
                Time.timeScale = 0;
            else
                Time.timeScale = 1;
            Debug.Log("timescale = " + Time.timeScale);
        }

    }
    #endregion

    #region Equipping and unequipping


    #endregion

    #region Take Damage
    public void TakeDamage(float damage, DamageType damageType, PlayerScript PlayerWhoShotYou, bool playBulletSFX)
    {
        if (!isDead && !isInvulnerable)
        {

            //only reset if it wasnt a world kill
            if (PlayerWhoShotYou != null)
            {
                playerLastHitBy = PlayerWhoShotYou;
                PlayerWhoShotYou.shotsHit++;
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

                        if (PlayerWhoShotYou != null)
                            PlayerWhoShotYou.headShots++;
                        break;
                    case DamageType.torso:
                        damage *= TORSOSHOT_MULTIPLIER;
                        SpawnFloatingDamageText(Mathf.RoundToInt(damage), DamageType.torso, "FloatAway");
                        //Color.yellow
                        if (PlayerWhoShotYou != null)
                            PlayerWhoShotYou.torsoShots++;
                        break;
                    case DamageType.legs:
                        damage *= LEGSHOT_MULTIPLIER;
                        SpawnFloatingDamageText(Mathf.RoundToInt(damage), DamageType.legs, "FloatAway");
                        // Color.black
                        if (PlayerWhoShotYou != null)
                            PlayerWhoShotYou.legShots++;
                        break;
                    case DamageType.feet:
                        damage *= FOOTSHOT_MULTIPLIER;
                        SpawnFloatingDamageText(Mathf.RoundToInt(damage), DamageType.feet, "FloatAway");
                        //Color.gray
                        if (PlayerWhoShotYou != null)
                            PlayerWhoShotYou.footShots++;
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
            playerUIPanel.setHealth(barVal);

            if (health <= 0)
            {
                //add a kill to whoever shot you, show it in GUI... as long as its not you
                if (playerLastHitBy != null && playerLastHitBy != this)
                {
                    playerLastHitBy.numKills++;
                    //playerLastHitBy.playerUIPanel.SetKills(playerLastHitBy.numKills);
                    playerLastHitBy.playerUIPanel.AddKill(this);
                }
                //reduce points if you kill yourself
                else if (playerLastHitBy != null && playerLastHitBy == this)
                {
                    playerLastHitBy.numKills--;
                    playerLastHitBy.playerUIPanel.AddKill(this);
                }

                //if (gameManager.dataManager.AllowWriteToFile)
                //SaveDamageData(PlayerWhoShotYou.armsScript.currentWeapon, Mathf.RoundToInt(damage), true, PlayerWhoShotYou);

                Die(damageType);
            }
            else
            {
                //if (gameManager.dataManager.AllowWriteToFile)
                //SaveDamageData(PlayerWhoShotYou.armsScript.currentWeapon, Mathf.RoundToInt(damage), false, PlayerWhoShotYou);
            }
        }
    }


    void SaveDamageData(GunSO currentWeapon, float dmg, bool dead, PlayerScript playerWhoShotYou)
    {
        GameManager gameManager = GameManager.Instance;

        // currentWeapon.gunDamageTotal += dmg;


        /*
        switch (gunType)
        {
            case GunType.pistol:
                gameManager.pistolDamage += dmg;
                playerWhoShotYou.pistolDmg += dmg;
                if (dead)
                    gameManager.pistolKills++;
                break;
            case GunType.assaultRifle:
                gameManager.assaultDamage += dmg;
                playerWhoShotYou.rifleDmg += dmg;
                if (dead)
                    gameManager.assaultRifleKills++;
                break;
            case GunType.LMG:
                gameManager.minigunDamage += dmg;
                playerWhoShotYou.miniGunDmg += dmg;
                if (dead)
                    gameManager.minigunKills++;
                break;
            case GunType.shotgun:
                gameManager.shotGunDamage += dmg;
                playerWhoShotYou.shotgunDmg += dmg;
                if (dead)
                    gameManager.shotGunKills++;
                break;
            case GunType.railGun:
                gameManager.railgunDamage += dmg;
                playerWhoShotYou.railgunDmg += dmg;
                if (dead)
                    gameManager.railgunKills++;
                break;
            case GunType.healthPack:
                gameManager.healthPackHeals += dmg;
                break;
            case GunType.collision:
                gameManager.collisionDamage += dmg;
                if (dead)
                    gameManager.collisionKills++;
                break;
            default:
                break;
        }*/
    }
    #endregion

    #region Die and respawn
    public PlayerScript Die(DamageType damgaeType)
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

            if (!isDummy)
                playerUIPanel.LoseStock();

            if (numLives <= 0)
            {
                playerUIPanel.Disable();

                if (!isDummy)
                    playerUIPanel.Destroy();
                
                GameManager.Instance.CheckForLastManStanding(transform);
                
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
            lastHitDamageType = damgaeType;           
        }
            
        return this;
    }

    IEnumerator WaitForRespawn()
    {
        lastHitDamageType = DamageType.self;
        playerUIPanel.SetAmmoText("3...", 1);
        yield return new WaitForSeconds(1f);
        playerUIPanel.SetAmmoText("2...", 1);
        yield return new WaitForSeconds(1f);
        playerUIPanel.SetAmmoText("1...", 1);
        yield return new WaitForSeconds(1f);

        //turn of rigidbody so we dont get some crazy momentum from force moving
        rb.isKinematic = true;
        transform.position = spawnPoint;
        transform.rotation = spawnRotation;
        rb.isKinematic = false;

        //emit those PFX
        var mainFlash = respawnFlash.main;
        var mainBits = respawnBits.main;
        Color c = playerColor;
        mainFlash.startColor = c;
        mainBits.startColor = c;
        respawnFlash.Emit(1);
        respawnBits.Emit(Random.Range(15, 30));

        ForcePushOnSpawn();

        //foreach (var legToFix in legFixers)
        //{
        //    legToFix.ResetLeg();
        //}

        health = 100;
        float barVal = ((float)health / 100f);
        audioSource.PlayOneShot(respawnClip);


        playerUIPanel.setHealth(barVal);

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

        armsScript.SendGunText();

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
        switch (playerID)
        {
            case 1:
                playerName = "Red";
                hexColorCode = "#B1342F";
                break;
            case 2:
                playerName = "Blue";
                hexColorCode = "#2C7EC2";
                break;
            case 3:
                playerName = "Green";
                hexColorCode = "#13BC1E";
                break;
            case 4:
                playerName = "Yellow";
                hexColorCode = "#EA9602";
                break;
        }

        player = ReInput.players.GetPlayer(playerID - 1);

        player.controllers.maps.SetMapsEnabled(true, "Gameplay");
        player.controllers.maps.SetMapsEnabled(true, "UI");
        Debug.Log(player.name);

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
        playerUIPanel.setHealth(barVal);
        isDead = false;
        numKills = 0;

        /*
        assaultRifleArms.SetActive(false);
        shotGunArms.SetActive(false);
        LMGArms.SetActive(false);
        */

        armsScript.EquipGun(GameManager.Instance.pistol);

        //torsoSR = GetComponent<SpriteRenderer>();
        //armsSR = armsScript.currentArms.GetComponent<SpriteRenderer>();
        //legsSR = legsParent.GetComponentsInChildren<SpriteRenderer>();


        //legRBs = legsParent.GetComponentsInChildren<Rigidbody2D>();

        if (!isDummy)
        {
           // playerUIPanel.SetLives(numLives, playerHead);
            player.controllers.AddController(controller, true);
        }

        rb.simulated = true;


        RoundManager.Instance.SetPlayer(this);

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
                TakeDamage(dmg, dmgType, hitBy, false);
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
        GameObject floatingTextGo = ObjectPooler.Instance.SpawnFromPool("FloatingText", floatingTextSpawnPoint.transform.position, Quaternion.identity, floatingTextSpawnPoint);
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
