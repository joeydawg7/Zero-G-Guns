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

    [Header("Controller Stuff")]
    public int playerID;
    public string BButton;

    [HideInInspector]
    public enum DamageType { head, torso, legs, feet };
    [HideInInspector]
    public enum GunType { pistol, assaultRifle, LMG, shotgun, railGun, healthPack };


    [Header("Bools")]
    public bool isDead;
    public bool isInvulnerable;

    [Header("AudioClips")]
    public AudioClip headShot;
    public AudioClip standardShot;

    [Header("armedArms")]
    public GameObject pistolArms;
    public GameObject assaultRifleArms;
    public GameObject LMGArms;
    public GameObject shotGunArms;
    public GameObject railGunArms;
    public ArmsScript armsScript;

    [Header("Spawning and kills")]
    public Vector3 spawnPoint;
    public Color32 invulnerabilityColorFlash;
    public float invulnerablityTime;
    public int numKills;

    [Header("Particle Effects")]
    public ParticleSystem HS_Flash;
    public ParticleSystem HS_Streaks;

    [HideInInspector]
    public string playerName;
    [HideInInspector]
    public string hexColorCode;

    public Rigidbody2D rb;


    public int numLives;

    //Private
    Quaternion targetRotation;
    Color32 defaultColor;
    AudioSource audioSource;
    float angle;
    int lastHitByID;
    float immuneToCollisionsTimer;


    const float HEADSHOT_MULTIPLIER = 2f;
    const float TORSOSHOT_MULTIPLIER = 1f;
    const float FOOTSHOT_MULTIPLIER = 0.5f;
    const float LEGSHOT_MULTIPLIER = 0.75f;



    private void Awake()
    {

        health = 100;
        float barVal = ((float)health / 100f);
        isDead = false;
        spawnPoint = transform.position;
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        numKills = 0;
        defaultColor = GetComponent<SpriteRenderer>().color;
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
            immuneToCollisionsTimer += Time.deltaTime;


        //DEBUG: take damage
        if (Input.GetKeyDown(KeyCode.K))
            TakeDamage(50, DamageType.torso, 0);


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
        StartCoroutine(RespawnInvulernability());
    }

    public void EquipArms(GunType gunType, GunSO gun)
    {
        HideAllArms();

        switch (gunType)
        {
            case GunType.pistol:
                pistolArms.SetActive(true);
                armsScript.EquipGun(gun, pistolArms);
                armsScript.currentArms = pistolArms;
                //code here to actually refill bullets to stop crap from hapening that is bad
                break;
            case GunType.railGun:
                railGunArms.SetActive(true);
                armsScript.EquipGun(gun, railGunArms);
                armsScript.currentArms = railGunArms;
                break;
            case GunType.assaultRifle:
                assaultRifleArms.SetActive(true);
                armsScript.EquipGun(gun, assaultRifleArms);
                armsScript.currentArms = assaultRifleArms;
                break;
            case GunType.LMG:
                LMGArms.SetActive(true);
                armsScript.EquipGun(gun, LMGArms);
                armsScript.currentArms = LMGArms;
                break;
            case GunType.shotgun:
                shotGunArms.SetActive(true);
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



    public void TakeDamage(float damage, DamageType damageType, int attackerID)
    {
        if (!isDead && !isInvulnerable)
        {
            lastHitByID = attackerID;

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
                        audioSource.PlayOneShot(headShot);
                        HS_Flash.Emit(1);
                        HS_Flash.Emit(Random.Range(35, 45));
                        break;
                    case DamageType.torso:
                        damage *= TORSOSHOT_MULTIPLIER;
                        SpawnFloatingDamageText(Mathf.RoundToInt(damage), Color.yellow, "FloatAway");
                        audioSource.PlayOneShot(standardShot);
                        break;
                    case DamageType.legs:
                        damage *= LEGSHOT_MULTIPLIER;
                        SpawnFloatingDamageText(Mathf.RoundToInt(damage), Color.black, "FloatAway");
                        audioSource.PlayOneShot(standardShot);
                        break;
                    case DamageType.feet:

                        damage *= FOOTSHOT_MULTIPLIER;
                        SpawnFloatingDamageText(Mathf.RoundToInt(damage), Color.gray, "FloatAway");
                        audioSource.PlayOneShot(standardShot);
                        break;
                    default:
                        break;
                }
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
                Die();
            }
        }

        if (health > 100)
            health = 100;
    }

    //overload to force custom SFX
    public void TakeDamage(float damage, DamageType damageType, int attackerID, AudioClip SFX)
    {
        audioSource.PlayOneShot(SFX);

        TakeDamage(damage, damageType, attackerID);
    }



    public PlayerScript Die()
    {
        if (!isDead)
        {
            isDead = true;
            numLives--;
            if(numLives<=0)
                GameManager.Instance.OnGameEnd();

            GetComponent<SpriteRenderer>().color = deadColor;
            armsScript.currentArms.GetComponent<SpriteRenderer>().color = deadColor;

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

        playerUIPanel.setHealth(barVal);

        rb.velocity = Vector2.zero;
        rb.rotation = 0;
        rb.simulated = false;
        rb.simulated = true;
        isDead = false;

        EquipArms(GunType.pistol, GameManager.Instance.pistol);
        StartCoroutine(RespawnInvulernability());

    }

    IEnumerator RespawnInvulernability()
    {
        isInvulnerable = true;

        float invulnerabilityFlashIncriments = (float)invulnerablityTime / 12f;
        SpriteRenderer armsSR = armsScript.currentArms.GetComponent<SpriteRenderer>();
        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        for (int i = 0; i < invulnerablityTime; i++)
        {
            sr.color = invulnerabilityColorFlash;
            armsSR.color = invulnerabilityColorFlash;
            yield return new WaitForSeconds(invulnerabilityFlashIncriments);
            sr.color = defaultColor;
            armsSR.color = defaultColor;
            yield return new WaitForSeconds(invulnerabilityFlashIncriments);
            sr.color = invulnerabilityColorFlash;
            armsSR.color = invulnerabilityColorFlash;
            yield return new WaitForSeconds(invulnerabilityFlashIncriments);
            sr.color = defaultColor;
            armsSR.color = defaultColor;
        }

        sr.color = defaultColor;
        armsSR.color = defaultColor;
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
            DealColliderDamage(collision);
        }
        else if (collision.collider.tag == "Torso" || collision.collider.tag == "Head" || collision.collider.tag == "Feet" || collision.collider.tag == "Legs")
        {
            collision.gameObject.GetComponent<PlayerScript>().lastHitByID = playerID;
            DealColliderDamage(collision);
        }

    }

    void DealColliderDamage(Collision2D collision)
    {
        float dmg = collision.relativeVelocity.magnitude;

        if (dmg > 100)
            dmg = 100;

        if (dmg > 50 && immuneToCollisionsTimer >= 1)
        {
            dmg = dmg / 4;
            immuneToCollisionsTimer = 0;
            TakeDamage(dmg, PlayerScript.DamageType.torso, 0);
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
