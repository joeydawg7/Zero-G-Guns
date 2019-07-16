using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerScript : MonoBehaviour
{

    public int health;

    [Header("Gui")]
    [HideInInspector]
    public PlayerUIPanel playerUIPanel;
    public Image healthBar;
    public TextMeshProUGUI statusText;
    //public TextMeshProUGUI floatingText;
    public Transform floatingTextSpawnPoint;
    public Color32 playerColor;
    public Color32 deadColor;
    public Sprite playerHead;

    [Header("Controller Stuff")]
    public int playerID;
    public string BButton;

    [HideInInspector]
    public enum DamageType { head, torso, legs, feet };
    [HideInInspector]
    public enum GunType { pistol, assaultRifle, LMG, shotgun, railGun, healthPack, collision };


    [Header("Bools")]
    public bool isDead;
    public bool isInvulnerable;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip headShot;
    public AudioClip standardShot;
    public AudioClip torsoImpact;
    public AudioClip legsImpact;
    public AudioClip headImpact;
    public AudioClip deathClip;
    public AudioClip respawnClip;

    [Header("armedArms")]
    public GameObject pistolArms;
    public GameObject assaultRifleArms;
    public GameObject LMGArms;
    public GameObject shotGunArms;
    public GameObject railGunArms;
    public ArmsScript armsScript;

    [Header("Armed Legs")]
    public GameObject legsCollider;

    [Header("Spawning and kills")]
    public Vector3 spawnPoint;
    public Color32 invulnerabilityColorFlash;
    public float invulnerablityTime;
    public int numKills;
    public int lastHitByID;

    [Header("Particle Effects")]
    public ParticleSystem HS_Flash;
    public ParticleSystem HS_Streaks;

    [HideInInspector]
    public string playerName;
    [HideInInspector]
    public string hexColorCode;

    public Rigidbody2D rb;
    public Transform legsParent;

    public int numLives;

    //Private
    Quaternion targetRotation;
    Color32 defaultColor;
    float angle;
    float immuneToCollisionsTimer;

    SpriteRenderer[] legsSR;
    SpriteRenderer torsoSR;
    SpriteRenderer armsSR;

    Rigidbody2D[] legRBs;


    const float HEADSHOT_MULTIPLIER = 2f;
    const float TORSOSHOT_MULTIPLIER = 1f;
    const float FOOTSHOT_MULTIPLIER = 0.5f;
    const float LEGSHOT_MULTIPLIER = 0.75f;

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

    private void Awake()
    {

        health = 100;
        float barVal = ((float)health / 100f);
        isDead = false;
        spawnPoint = transform.position;
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        numKills = 0;

        defaultColor = gameObject.GetComponent<SpriteRenderer>().color;
        lastHitByID = 0;
        immuneToCollisionsTimer = 0;



    }

    // Update is called once per frame
    void FixedUpdate()
    {


    }

    private void Update()
    {
        if (GameManager.Instance.isGameStarted && Input.GetButton(BButton) && armsScript.currentWeapon.GunType != GunType.pistol)
            EquipArms(GunType.pistol, GameManager.Instance.pistol);

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
            TakeDamage(50, DamageType.torso, 0, true, GunType.collision);



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

    public void TakeDamage(float damage, DamageType damageType, int attackerID, bool playBulletSFX, GunType gunType)
    {
        if (!isDead && !isInvulnerable)
        {
            //only reset if it wasnt a world kill
            if (attackerID != 0)
                lastHitByID = attackerID;

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
                        break;
                    case DamageType.torso:
                        damage *= TORSOSHOT_MULTIPLIER;
                        SpawnFloatingDamageText(Mathf.RoundToInt(damage), Color.yellow, "FloatAway");
                        break;
                    case DamageType.legs:
                        damage *= LEGSHOT_MULTIPLIER;
                        SpawnFloatingDamageText(Mathf.RoundToInt(damage), Color.black, "FloatAway");
                        break;
                    case DamageType.feet:
                        damage *= FOOTSHOT_MULTIPLIER;
                        SpawnFloatingDamageText(Mathf.RoundToInt(damage), Color.gray, "FloatAway");
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
                //PlayerScript[] players = GameObject.FindObjectsOfType<PlayerScript>();
                foreach (var player in GameManager.Instance.players)
                {
                    //find the real killer
                    if (player.playerID == lastHitByID)
                        player.numKills++;
                }

                SaveDamageData(gunType, Mathf.RoundToInt(damage), true);

                Die();
            }
            else
                SaveDamageData(gunType, Mathf.RoundToInt(damage), false);
        }

        if (health > 100)
            health = 100;
    }


    void SaveDamageData(GunType gunType, float dmg, bool dead)
    {
        GameManager gameManager = GameManager.Instance;

        switch (gunType)
        {
            case GunType.pistol:
                gameManager.pistolDamage += dmg;
                if (dead)
                    gameManager.pistolKills++;
                break;
            case GunType.assaultRifle:
                gameManager.assaultDamage += dmg;
                if (dead)
                    gameManager.assaultRifleKills++;
                break;
            case GunType.LMG:
                gameManager.minigunDamage += dmg;
                if (dead)
                    gameManager.minigunKills++;
                break;
            case GunType.shotgun:
                gameManager.shotGunDamage += dmg;
                if (dead)
                    gameManager.shotGunKills++;
                break;
            case GunType.railGun:
                gameManager.railgunDamage += dmg;
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
        lastHitByID = 0;

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

    public void SetControllerNumber(int number)
    {
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

        foreach (Transform child in transform)
        {
            if (child.tag == "Arms")
            {
                child.GetComponent<ArmsScript>().triggerAxis = "J" + playerID + "Trigger";
                child.GetComponent<ArmsScript>().horizontalAxis = "J" + playerID + "Horizontal";
                child.GetComponent<ArmsScript>().verticalAxis = "J" + playerID + "Vertical";
                child.GetComponent<ArmsScript>().XButton = "J" + playerID + "X";
                BButton = "J" + playerID + "B";

                child.GetComponent<ArmsScript>().SetChildrenWithAxis(playerID);
            }
        }

    }

    public void UnsetControllerNumber(int number)
    {
        playerID = 0;
        playerName = "";

        foreach (Transform child in transform)
        {
            if (child.tag == "Arms")
            {
                child.GetComponent<ArmsScript>().triggerAxis = "";
                child.GetComponent<ArmsScript>().horizontalAxis = "";
                child.GetComponent<ArmsScript>().verticalAxis = "";
                child.GetComponent<ArmsScript>().XButton = "";
                BButton = "";

                child.GetComponent<ArmsScript>().UnsetChildrenWithAxis(playerID);
            }
        }

    }


    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.collider.tag == "ImpactObject")
        {
            DealColliderDamage(collision, "Torso", 0);
        }
        else if (collision.collider.tag == "Torso" || collision.collider.tag == "Head" || collision.collider.tag == "Feet" || collision.collider.tag == "Legs")
        {
            int hitByID = collision.transform.root.GetComponent<PlayerScript>().lastHitByID;
            DealColliderDamage(collision, "Torso", hitByID);
        }

    }

    public void DealColliderDamage(Collision2D collision, string hitLocation, int hitByID)
    {
        float dmg = collision.relativeVelocity.magnitude;

        //dont bother dealing damage unless unmitigated damage indicates fast enough collision
        if (dmg > 50)
        {

            DamageType dmgType;
            AudioClip soundClipToPlay;

            switch (hitLocation)
            {
                case ("Torso"):
                    dmg *= TORSOSHOT_MULTIPLIER;
                    dmgType = DamageType.torso;
                    soundClipToPlay = torsoImpact;
                    break;
                case ("Leg"):
                    dmg *= LEGSHOT_MULTIPLIER;
                    dmgType = DamageType.legs;
                    soundClipToPlay = legsImpact;
                    break;
                case ("Head"):
                    dmg *= TORSOSHOT_MULTIPLIER;
                    dmgType = DamageType.head;
                    soundClipToPlay = headImpact;
                    break;
                case ("Foot"):
                    dmg *= FOOTSHOT_MULTIPLIER;
                    dmgType = DamageType.feet;
                    soundClipToPlay = legsImpact;
                    break;
                default:
                    dmg *= TORSOSHOT_MULTIPLIER;
                    dmgType = DamageType.torso;
                    soundClipToPlay = torsoImpact;
                    break;
            }

            //caps unmitigated damage
            if (dmg > 100)
                dmg = 100;

            if (immuneToCollisionsTimer >= 1)
            {
                //reduces damage so its not bullshit
                dmg = dmg / 6;
                immuneToCollisionsTimer = 0;
                audioSource.PlayOneShot(soundClipToPlay);
                TakeDamage(dmg, dmgType, hitByID, false, GunType.collision);
            }
        }
    }

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

        dmgToShow *= 2;

        floatingTextGo.transform.localScale = new Vector3(floatingTextGo.transform.localScale.x * ((float)dmgToShow / 50f), floatingTextGo.transform.localScale.y * ((float)dmgToShow / 50f),
            floatingTextGo.transform.localScale.z * ((float)dmgToShow / 50f));
    }


}
