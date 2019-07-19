using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class PlayerScript : MonoBehaviour
{
    #region publics
    [Header("Health and Lives")]
    public int health;
    public int numLives;

    [Header("Gui")]
    [HideInInspector]
    public PlayerUIPanel playerUIPanel;
    public Image healthBar;
    public TextMeshProUGUI statusText;
    public Transform floatingTextSpawnPoint;
    public Color32 playerColor;
    public Color32 deadColor;
    public Sprite playerHead;
    public int collisionLayer;

    [Header("Controller Stuff")]
    public int playerID;
    public string BButton;
    public PlayerControls controls;

    [HideInInspector]
    public enum DamageType { head, torso, legs, feet };
    [HideInInspector]
    public enum GunType { pistol, assaultRifle, LMG, shotgun, railGun, healthPack, collision };


    [Header("Bools")]
    public bool isDead;
    public bool isInvulnerable;

    [Header("armedArms")]
    public GameObject pistolArms;
    public GameObject assaultRifleArms;
    public GameObject LMGArms;
    public GameObject shotGunArms;
    public GameObject railGunArms;
    public ArmsScript armsScript;

    [Header("Armed Legs")]
    public GameObject legsCollider;
    public Transform legsParent;

    [Header("Spawning and kills")]
    public Vector3 spawnPoint;
    public Color32 invulnerabilityColorFlash;
    public float invulnerablityTime;
    public int numKills;
    public PlayerScript playerLastHitBy;

    [Header("Particle Effects")]
    public ParticleSystem HS_Flash;
    public ParticleSystem HS_Streaks;
    #endregion
    #region Audio
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip headShot;
    public AudioClip standardShot;
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
    #endregion
    #region privates
    //Private
    Quaternion targetRotation;
    Color32 defaultColor;
    float angle;
    float immuneToCollisionsTimer;
    SpriteRenderer[] legsSR;
    SpriteRenderer torsoSR;
    SpriteRenderer armsSR;
    Rigidbody2D[] legRBs;
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

    private void Awake()
    {

        health = 100;
        float barVal = ((float)health / 100f);
        isDead = false;
        spawnPoint = transform.position;
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        
        defaultColor = gameObject.GetComponent<SpriteRenderer>().color;
        playerLastHitBy = null;
        immuneToCollisionsTimer = 0;

        //data inital settings
        shotsFired = 0;
        shotsHit = 0;
        headShots = 0;
        torsoShots = 0;
        legShots = 0;
        footShots = 0;
        numKills = 0;

    }

    private void OnEnable()
    {
        if (GameManager.Instance.isGameStarted && controls != null)
            controls.Gameplay.Enable();
    }

    private void OnDisable()
    {
        if (GameManager.Instance.isGameStarted && controls != null)
            controls.Gameplay.Disable();
    }

    void TryWeaponDrop()
    {
        if (GameManager.Instance.isGameStarted && armsScript.currentWeapon.GunType != GunType.pistol)
            EquipArms(GunType.pistol, GameManager.Instance.pistol);
    }

    private void Update()
    {

        if (GameManager.Instance.isGameStarted)
        {
            immuneToCollisionsTimer += Time.deltaTime;

            if (armsScript.currentArms == pistolArms)
            {
                pistolTime += Time.deltaTime;
            }
            else if (armsScript.currentArms == assaultRifleArms)
            {
                rifleTime += Time.deltaTime;
            }
            else if (armsScript.currentArms == shotGunArms)
            {
                shotgunTime += Time.deltaTime;
            }
            else if (armsScript.currentArms == railGunArms)
            {
                railgunTime += Time.deltaTime;
            }
            else if (armsScript.currentArms == LMGArms)
            {
                miniGunTime += Time.deltaTime;
            }

        }

        //DEBUG: take damage
        if (Input.GetKeyDown(KeyCode.K))
            TakeDamage(50, DamageType.torso, null, true, GunType.collision);


    }

    public void OnGameStart()
    {

        health = 100;
        float barVal = ((float)health / 100f);
        playerUIPanel.setHealth(barVal);
        isDead = false;
        statusText.text = playerName;
        numKills = 0;

        assaultRifleArms.SetActive(false);
        shotGunArms.SetActive(false);
        LMGArms.SetActive(false);
        EquipArms(GunType.pistol, GameManager.Instance.pistol);

        torsoSR = GetComponent<SpriteRenderer>();
        armsSR = armsScript.currentArms.GetComponent<SpriteRenderer>();
        legsSR = GetComponentsInChildren<SpriteRenderer>();
        legRBs = legsParent.GetComponentsInChildren<Rigidbody2D>();

        playerUIPanel.SetLives(numLives, playerHead);

        if (controls != null)
            controls.Gameplay.Enable();

        StartCoroutine(RespawnInvulernability());
    }

    public void EquipArms(GunType gunType, GunSO gun)
    {
        HideAllArms();


        switch (gunType)
        {
            case GunType.pistol:
                pistolArms.SetActive(true);
                pistolArms.GetComponent<SpriteRenderer>().color = defaultColor;
                armsScript.EquipGun(gun, pistolArms);
                armsScript.currentArms = pistolArms;
                break;
            case GunType.railGun:
                railGunArms.SetActive(true);
                railGunArms.GetComponent<SpriteRenderer>().color = defaultColor;
                armsScript.EquipGun(gun, railGunArms);
                armsScript.currentArms = railGunArms;
                break;
            case GunType.assaultRifle:
                assaultRifleArms.SetActive(true);
                assaultRifleArms.GetComponent<SpriteRenderer>().color = defaultColor;
                armsScript.EquipGun(gun, assaultRifleArms);
                armsScript.currentArms = assaultRifleArms;
                break;
            case GunType.LMG:
                LMGArms.SetActive(true);
                LMGArms.GetComponent<SpriteRenderer>().color = defaultColor;
                armsScript.EquipGun(gun, LMGArms);
                armsScript.currentArms = LMGArms;
                break;
            case GunType.shotgun:
                shotGunArms.SetActive(true);
                shotGunArms.GetComponent<SpriteRenderer>().color = defaultColor;
                armsScript.EquipGun(gun, shotGunArms);
                armsScript.currentArms = shotGunArms;
                break;
            default:
                break;
        }

        armsScript.SendGunText();


    }

    void HideAllArms()
    {
        foreach (Transform child in armsScript.gameObject.transform)
        {
            if (child.tag == "Gun")
            {
                child.gameObject.SetActive(false);

            }
        }
    }

    //every type of damage dealt to player is parsed in this region, including healing
    #region take damage 
    public void TakeDamage(float damage, DamageType damageType, PlayerScript PlayerWhoShotYou, bool playBulletSFX, GunType gunType)
    {
        if (!isDead && !isInvulnerable && GameManager.Instance.isGameStarted)
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
                SpawnFloatingDamageText(Mathf.RoundToInt(damage), Color.green, "FloatAway");
            }
            else
            {
                switch (damageType)
                {
                    case DamageType.head:
                        damage *= HEADSHOT_MULTIPLIER;
                        SpawnFloatingDamageText(Mathf.RoundToInt(damage), Color.red, "Crit");
                        HS_Flash.Emit(1);
                        HS_Flash.Emit(Random.Range(35, 45));
                        if (playBulletSFX)
                            audioSource.PlayOneShot(headShot);

                        if (PlayerWhoShotYou != null)
                            PlayerWhoShotYou.headShots++;
                        break;
                    case DamageType.torso:
                        damage *= TORSOSHOT_MULTIPLIER;
                        SpawnFloatingDamageText(Mathf.RoundToInt(damage), Color.yellow, "FloatAway");

                        if (PlayerWhoShotYou != null)
                            PlayerWhoShotYou.torsoShots++;
                        break;
                    case DamageType.legs:
                        damage *= LEGSHOT_MULTIPLIER;
                        SpawnFloatingDamageText(Mathf.RoundToInt(damage), Color.black, "FloatAway");

                        if (PlayerWhoShotYou != null)
                            PlayerWhoShotYou.legShots++;
                        break;
                    case DamageType.feet:
                        damage *= FOOTSHOT_MULTIPLIER;
                        SpawnFloatingDamageText(Mathf.RoundToInt(damage), Color.gray, "FloatAway");

                        if (PlayerWhoShotYou != null)
                            PlayerWhoShotYou.footShots++;
                        break;
                    default:
                        break;
                }

                if (damageType != DamageType.head && playBulletSFX)
                    audioSource.PlayOneShot(standardShot);
            }


            health -= (int)damage;
            float barVal = ((float)health / 100f);
            playerUIPanel.setHealth(barVal);

            if (health <= 0)
            {
                if (playerLastHitBy != null)
                    playerLastHitBy.numKills++;

                SaveDamageData(gunType, Mathf.RoundToInt(damage), true, PlayerWhoShotYou);

                Die();
            }
            else
                SaveDamageData(gunType, Mathf.RoundToInt(damage), false, PlayerWhoShotYou);
        }

        if (health > 100)
            health = 100;
    }


    void SaveDamageData(GunType gunType, float dmg, bool dead, PlayerScript playerWhoShotYou)
    {
        GameManager gameManager = GameManager.Instance;

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
        }
    }
    #endregion


    public PlayerScript Die()
    {
        if (!isDead)
        {
            audioSource.Stop();

            isDead = true;
            numLives--;
            audioSource.PlayOneShot(deathClip);
            playerUIPanel.SetLives(numLives, playerHead);
            if (numLives <= 0)
            {
                Camera.main.transform.parent.GetComponent<CameraController>().RemovePlayerFromCameraTrack(gameObject);
                GameManager.Instance.CheckForLastManStanding();
            }
            armsSR = armsScript.currentArms.GetComponent<SpriteRenderer>();

            torsoSR.color = deadColor;
            armsSR.color = deadColor;

            foreach (var sr in legsSR)
            {
                sr.color = deadColor;
            }

            armsScript.reloadTimer.SetActive(false);

            StartCoroutine(WaitForRespawn());
        }

        return this;
    }

    IEnumerator WaitForRespawn()
    {
        playerUIPanel.setStatusText("Respawning in 3...");
        yield return new WaitForSeconds(1f);
        playerUIPanel.setStatusText("Respawning in 2...");
        yield return new WaitForSeconds(1f);
        playerUIPanel.setStatusText("Respawning in 1...");
        yield return new WaitForSeconds(1f);
        playerUIPanel.setStatusText(playerName);
        transform.position = spawnPoint;
        health = 100;
        float barVal = ((float)health / 100f);
        audioSource.PlayOneShot(respawnClip);

        playerUIPanel.setHealth(barVal);

        isDead = false;
        //last thing you were hit by set back to world, just in case you suicide without help
        playerLastHitBy = null;

        EquipArms(GunType.pistol, GameManager.Instance.pistol);


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

        StartCoroutine(RespawnInvulernability());

    }

    IEnumerator RespawnInvulernability()
    {
        isInvulnerable = true;
        armsSR = armsScript.currentArms.GetComponent<SpriteRenderer>();

        float invulnerabilityFlashIncriments = (float)invulnerablityTime / 12f;

        for (int i = 0; i < invulnerablityTime; i++)
        {
            torsoSR.color = invulnerabilityColorFlash;
            armsSR.color = invulnerabilityColorFlash;
            foreach (var sr in legsSR)
            {
                sr.color = invulnerabilityColorFlash;
            }

            yield return new WaitForSeconds(invulnerabilityFlashIncriments);
            torsoSR.color = defaultColor;
            armsSR.color = defaultColor;
            foreach (var sr in legsSR)
            {
                sr.color = defaultColor;
            }

            yield return new WaitForSeconds(invulnerabilityFlashIncriments);
            torsoSR.color = invulnerabilityColorFlash;
            armsSR.color = invulnerabilityColorFlash;
            foreach (var sr in legsSR)
            {
                sr.color = invulnerabilityColorFlash;
            }

            yield return new WaitForSeconds(invulnerabilityFlashIncriments);
            torsoSR.color = defaultColor;
            armsSR.color = defaultColor;
            foreach (var sr in legsSR)
            {
                sr.color = defaultColor;
            }
        }

        torsoSR.color = defaultColor;
        armsSR.color = defaultColor;
        foreach (var sr in legsSR)
        {
            sr.color = defaultColor;
        }

        isInvulnerable = false;
    }

    public void SetController(PlayerControls playerControls, int number)
    {

        this.controls = playerControls;

        playerID = number;
        switch (playerID)
        {
            case 1:
                playerName = "Red Player";
                hexColorCode = "#B1342F";
                break;
            case 2:
                playerName = "Blue Player";
                hexColorCode = "#2C7EC2";
                break;
            case 3:
                playerName = "Green Player";
                hexColorCode = "#13BC1E";
                break;
            case 4:
                playerName = "Yellow Player";
                hexColorCode = "#EA9602";
                break;
        }

        controls.Gameplay.Drop.performed += context => TryWeaponDrop();

        transform.Find("Arms").GetComponent<ArmsScript>().ArmsControllerSettings();

    }

    public void UnsetController()
    {
        playerID = 0;
        playerName = "";
        controls = null;
    }

    //collision check and damage mutlipliers / modifiers
    #region collision Damage
    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.collider.tag == "ImpactObject")
        {
            DealColliderDamage(collision, "Torso", null);
        }
        else if (collision.collider.tag == "Torso" || collision.collider.tag == "Head" || collision.collider.tag == "Feet" || collision.collider.tag == "Legs")
        {
            PlayerScript hitBy = collision.transform.root.GetComponent<PlayerScript>();
            DealColliderDamage(collision, "Torso", hitBy);
        }

    }

    public void DealColliderDamage(Collision2D collision, string hitLocation, PlayerScript hitBy)
    {
        float dmg = collision.relativeVelocity.magnitude;
        //reduces damage so its not bullshit
        dmg = dmg / COLLIDER_DAMAGE_MITIGATOR;

        //dont bother dealing damage unless unmitigated damage indicates fast enough collision
        if (dmg > 10)
        {

            DamageType dmgType;
            AudioClip soundClipToPlay;

            switch (hitLocation)
            {
                case ("Torso"):
                    dmgType = DamageType.torso;
                    soundClipToPlay = torsoImpact;
                    break;
                case ("Leg"):
                    dmgType = DamageType.legs;
                    soundClipToPlay = legsImpact;
                    break;
                case ("Head"):
                    dmgType = DamageType.torso;
                    soundClipToPlay = torsoImpact;
                    break;
                case ("Foot"):
                    dmgType = DamageType.feet;
                    soundClipToPlay = legsImpact;
                    break;
                default:
                    dmgType = DamageType.torso;
                    soundClipToPlay = torsoImpact;
                    break;
            }

            //caps damage
            if (dmg > 100)
                dmg = 100;

            if (immuneToCollisionsTimer >= 1)
            {
                immuneToCollisionsTimer = 0;
                audioSource.PlayOneShot(soundClipToPlay);
                TakeDamage(dmg, dmgType, hitBy, false, GunType.collision);
            }
        }
    }
    #endregion


    void SpawnFloatingDamageText(int dmgToShow, Color32 color, string animType)
    {

        GameObject floatingTextGo = ObjectPooler.Instance.SpawnFromPool("FloatingText", floatingTextSpawnPoint.transform.position, Quaternion.identity, floatingTextSpawnPoint);
        floatingTextGo.transform.localPosition = new Vector3(0, 0, 0);
        floatingTextGo.transform.localScale = new Vector3(1, 1, 1);
        TextMeshProUGUI floatTxt = floatingTextGo.GetComponent<TextMeshProUGUI>();

        if (dmgToShow < 0)
        {
            dmgToShow = Mathf.Abs(dmgToShow);
            floatTxt.text = "+" + dmgToShow.ToString();
        }
        else
            floatTxt.text = dmgToShow.ToString();

        floatingTextGo.transform.position = new Vector2(Random.Range(-1.5f, 1.5f), Random.Range(-1.5f, 1.5f));
        floatTxt.color = color;
        floatTxt.GetComponent<Animator>().SetTrigger(animType);

        //temp multiply damage by 2 to affect how big we scale it. trenchfoot math :D
        dmgToShow *= 2;

        floatingTextGo.transform.localScale = new Vector3(floatingTextGo.transform.localScale.x * ((float)dmgToShow / 50f), floatingTextGo.transform.localScale.y * ((float)dmgToShow / 50f),
            floatingTextGo.transform.localScale.z * ((float)dmgToShow / 50f));
    }


}
