using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerScript : MonoBehaviour
{

    public int health;

    [Header("Gui")]
    public Image healthBar;
    public TextMeshProUGUI statusText;
    

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


    [HideInInspector]
    public string playerName;
    [HideInInspector]
    public string hexColorCode;

    //Private
    Quaternion targetRotation;
    Color32 defaultColor;
    Rigidbody2D rb;
    AudioSource audioSource;
    float angle;

    const float HEADSHOT_MULTIPLIER = 2f;
    const float TORSOSHOT_MULTIPLIER = 1f;
    const float FOOTSHOT_MULTIPLIER = 0.5f;
    const float LEGSHOT_MULTIPLIER = 0.75f;


    private void Awake()
    {

        health = 100;
        float barVal = ((float)health / 100f);
        healthBar.fillAmount = barVal;
        isDead = false;
        statusText.text = "";
        spawnPoint = transform.position;
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        numKills = 0;
        defaultColor = GetComponent<SpriteRenderer>().color;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isDead)
        {

        }

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
        healthBar.fillAmount = barVal;
        isDead = false;
        statusText.text = playerName;
        numKills = 0;

        assaultRifleArms.SetActive(false);
        shotGunArms.SetActive(false);
        LMGArms.SetActive(false);
        armsScript.gunAndAmmo.alpha = 1;
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

        armsScript.gunAndAmmo.text = armsScript.GetGunsAndAmmoText();


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
            switch (damageType)
            {
                case DamageType.head:
                    damage *= HEADSHOT_MULTIPLIER;
                    audioSource.PlayOneShot(headShot);
                    break;
                case DamageType.torso:
                    damage *= TORSOSHOT_MULTIPLIER;
                    audioSource.PlayOneShot(standardShot);
                    break;
                case DamageType.legs:
                    damage *= LEGSHOT_MULTIPLIER;
                    audioSource.PlayOneShot(standardShot);
                    break;
                case DamageType.feet:
                    damage *= FOOTSHOT_MULTIPLIER;
                    audioSource.PlayOneShot(standardShot);
                    break;
                default:
                    break;
            }

            health -= (int)damage;
            float barVal = ((float)health / 100f);

            healthBar.fillAmount = barVal;

            if (health <= 0)
            {
                PlayerScript[] players = GameObject.FindObjectsOfType<PlayerScript>();

                foreach (var player in players)
                {
                    //find the real killer
                    if (player.playerID == attackerID)
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

        if (!isDead && !isInvulnerable)
        {
            switch (damageType)
            {
                case DamageType.head:
                    damage *= HEADSHOT_MULTIPLIER;
                    break;
                case DamageType.torso:
                    damage *= TORSOSHOT_MULTIPLIER;
                    break;
                case DamageType.legs:
                    damage *= LEGSHOT_MULTIPLIER;
                    break;
                case DamageType.feet:
                    damage *= FOOTSHOT_MULTIPLIER;
                    break;
                default:
                    break;
            }

            health -= (int)damage;
            float barVal = ((float)health / 100f);

            healthBar.fillAmount = barVal;

            if (health <= 0)
            {
                PlayerScript[] players = GameObject.FindObjectsOfType<PlayerScript>();

                foreach (var player in players)
                {
                    //find the real killer
                    if (player.playerID == attackerID)
                        player.numKills++;
                }
                Die();
            }
        }

        if (health > 100)
            health = 100;
    }



    public PlayerScript Die()
    {
        if (!isDead)
        {
            isDead = true;

            StartCoroutine(WaitForRespawn());
        }

        return this;
    }

    IEnumerator WaitForRespawn()
    {
        statusText.text = "Respawning in 3...";
        yield return new WaitForSeconds(1f);
        statusText.text = "Respawning in 2...";
        yield return new WaitForSeconds(1f);
        statusText.text = "Respawning in 1...";
        yield return new WaitForSeconds(1f);
        statusText.text = "";
        transform.position = spawnPoint;
        health = 100;
        float barVal = ((float)health / 100f);

        healthBar.fillAmount = barVal;

        rb.velocity = Vector2.zero;
        rb.rotation = 0;
        isDead = false;

        EquipArms(GunType.pistol, GameManager.Instance.pistol);
        StartCoroutine(RespawnInvulernability());

    }

    IEnumerator RespawnInvulernability()
    {
        isInvulnerable = true;

        float invulnerabilityFlashIncriments = (float)invulnerablityTime / 6f;

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
        switch(playerID)
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
            float dmg = Mathf.Abs(rb.velocity.x + rb.velocity.y);

            if (dmg > 50)
                dmg = 50;

            if (dmg > 25)
            {
            	dmg = dmg/2;
                TakeDamage(dmg, PlayerScript.DamageType.torso, 0);
            }

        }
    }


}
