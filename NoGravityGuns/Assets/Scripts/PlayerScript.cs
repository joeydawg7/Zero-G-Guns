﻿using System.Collections;
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
    public TextMeshProUGUI floatingText;
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

    //Private
    Quaternion targetRotation;
    Color32 defaultColor;  
    AudioSource audioSource;
    float angle;
    int lastHitByID;
    bool immuneToCollisions;

    const float HEADSHOT_MULTIPLIER = 2f;
    const float TORSOSHOT_MULTIPLIER = 1f;
    const float FOOTSHOT_MULTIPLIER = 0.5f;
    const float LEGSHOT_MULTIPLIER = 0.75f;



    private void Awake()
    {

        health = 100;
        float barVal = ((float)health / 100f);
        //playerUIPanel.setHealth(barVal);
        isDead = false;
        //playerUIPanel.setStatusText("");
        spawnPoint = transform.position;
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        numKills = 0;
        defaultColor = GetComponent<SpriteRenderer>().color;
        lastHitByID = 0;
        immuneToCollisions = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if (Input.GetKeyDown(KeyCode.K))
        {
            TakeDamage(50, DamageType.torso, 0);
        }
    }

    private void Update()
    {
        if (GameManager.Instance.isGameStarted && Input.GetButton(BButton) && armsScript.currentWeapon.GunType != GunType.pistol)
            EquipArms(GunType.pistol, GameManager.Instance.pistol);

    }

    public void OnGameStart()
    {
        /*
        if (playerID < 1)
        {
            Destroy(gameObject);       
        }*/

        health = 100;
        float barVal = ((float)health / 100f);
        playerUIPanel.setHealth(barVal);
        isDead = false;
        statusText.text = playerName;
        numKills = 0;

        assaultRifleArms.SetActive(false);
        shotGunArms.SetActive(false);
        LMGArms.SetActive(false);
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
                //code here to actually refill bullets to stop crap from hapening that is bad
                break;
            case GunType.railGun:
                railGunArms.SetActive(true);
                armsScript.EquipGun(gun, railGunArms);
                break;
            case GunType.assaultRifle:
                assaultRifleArms.SetActive(true);
                armsScript.EquipGun(gun, assaultRifleArms);
                break;
            case GunType.LMG:
                LMGArms.SetActive(true);
                armsScript.EquipGun(gun, LMGArms);
                break;
            case GunType.shotgun:
                shotGunArms.SetActive(true);
                armsScript.EquipGun(gun, shotGunArms);
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
                damage = Mathf.Abs(damage);
                SpawnFloatingDamageText("+" + Mathf.RoundToInt(damage).ToString(), Color.green, "FloatAway");
                damage = damage * -1;
            }
            else
            {
                switch (damageType)
                {
                    case DamageType.head:

                        damage *= HEADSHOT_MULTIPLIER;
                        SpawnFloatingDamageText(Mathf.RoundToInt(damage).ToString(), Color.red, "Crit");
                        audioSource.PlayOneShot(headShot);
                        HS_Flash.Emit(1);
                        HS_Flash.Emit(Random.Range(35, 45));
                        break;
                    case DamageType.torso:
                        damage *= TORSOSHOT_MULTIPLIER;
                        SpawnFloatingDamageText(Mathf.RoundToInt(damage).ToString(), Color.yellow, "FloatAway");
                        audioSource.PlayOneShot(standardShot);
                        break;
                    case DamageType.legs:
                        damage *= LEGSHOT_MULTIPLIER;
                        SpawnFloatingDamageText(Mathf.RoundToInt(damage).ToString(), Color.black, "FloatAway");
                        audioSource.PlayOneShot(standardShot);
                        break;
                    case DamageType.feet:

                        damage *= FOOTSHOT_MULTIPLIER;
                        SpawnFloatingDamageText(Mathf.RoundToInt(damage).ToString(), Color.gray, "FloatAway");
                        audioSource.PlayOneShot(standardShot);
                        break;
                    default:
                        break;
                }
            }

            health -= (int)damage;
            float barVal = ((float)health / 100f);
            playerUIPanel.setHealth(barVal);

            immuneToCollisions = false;

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
            GetComponent<SpriteRenderer>().color = deadColor;

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
        isDead = false;

        EquipArms(GunType.pistol, GameManager.Instance.pistol);
        StartCoroutine(RespawnInvulernability());

    }

    IEnumerator RespawnInvulernability()
    {
        isInvulnerable = true;

        float invulnerabilityFlashIncriments = (float)invulnerablityTime / 12f;

        for (int i = 0; i < invulnerablityTime; i++)
        {
            GetComponent<SpriteRenderer>().color = invulnerabilityColorFlash;
            yield return new WaitForSeconds(invulnerabilityFlashIncriments);
            GetComponent<SpriteRenderer>().color = defaultColor;
            yield return new WaitForSeconds(invulnerabilityFlashIncriments);
            GetComponent<SpriteRenderer>().color = invulnerabilityColorFlash;
            yield return new WaitForSeconds(invulnerabilityFlashIncriments);
            GetComponent<SpriteRenderer>().color = defaultColor;
        }

        GetComponent<SpriteRenderer>().color = defaultColor;
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

        //horizontalAxis = "J" + playerID + "Horizontal";
        //verticalAxis = "J" + playerID + "Vertical";
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

        if (dmg > 50 && !immuneToCollisions)
        {
            dmg = dmg / 4;
            immuneToCollisions = true;
            TakeDamage(dmg, PlayerScript.DamageType.torso, 0);
        }
    }

    void SpawnFloatingDamageText(string textToShow, Color32 color, string animType)
    {
        TextMeshProUGUI floatTxt = Instantiate(floatingText, floatingTextSpawnPoint);
        floatTxt.text = textToShow.ToString();

        floatingText.transform.position = new Vector2(Random.Range(-1.5f, 1.5f), Random.Range(-1.5f, 1.5f));
        floatTxt.color = color;
        floatTxt.GetComponent<Animator>().SetTrigger(animType);

        //floatingText.transform.localScale = new Vector3(floatingText.transform.localScale.x *1, floatingText.transform.localScale.y *1, floatingText.transform.localScale.z * 1);
    }


}
