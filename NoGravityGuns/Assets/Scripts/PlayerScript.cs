using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerScript : MonoBehaviour
{

    public int health;

    public Image healthBar;

    public bool isDead;
    public TextMeshProUGUI statusText;
    public Vector3 spawnPoint;

    public float turnSpeed;

    Rigidbody2D rb;
    AudioSource audioSource;
    float angle;

    public string horizontalAxis;
    public string verticalAxis;

    Quaternion targetRotation;

    public int playerID;

    public int numKills;

    const float HEADSHOT_MULTIPLIER = 2f;
    const float TORSOSHOT_MULTIPLIER = 1f;
    const float FOOTSHOT_MULTIPLIER = 0.5f;
    const float LEGSHOT_MULTIPLIER = 0.75f;

    public enum DamageType { head, torso, legs, feet };

    public enum GunType { pistol, assaultRifle, LMG, shotgun, healthPack };

    public Sprite spriteColor;

    [Header("AudioClips")]
    public AudioClip headShot;
    public AudioClip standardShot;

    [Header("armedArms")]
    public GameObject pistolArms;
    public GameObject assaultRifleArms;
    public GameObject LMGArms;
    public GameObject shotGunArms;

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

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isDead)
        {
            //do left stick rotaty

            //transform.Rotate(0, 0, -Input.GetAxis(horizontalAxis)* turnSpeed);
            /*Vector2 shootDir = Vector2.right * Input.GetAxis(horizontalAxis) + Vector2.up * Input.GetAxis(verticalAxis);

            float angle =  Vector2.SignedAngle(transform.position, shootDir.normalized);

            rb.MoveRotation(rb.rotation + angle *  Time.fixedDeltaTime);
            //rb.MoveRotation(-Input.GetAxis(horizontalAxis) * turnSpeed);
            */
        }

        if(Input.GetKeyDown(KeyCode.K))
        {
            TakeDamage(50, DamageType.torso, 0);
        }
    }

    public void OnGameStart()
    {
        if (playerID < 1)
        {
            Destroy(gameObject);
        }

        health = 100;
        float barVal = ((float)health / 100f);
        healthBar.fillAmount = barVal;
        isDead = false;
        statusText.text = "";
        numKills = 0;

        assaultRifleArms.SetActive(false);
        shotGunArms.SetActive(false);
        LMGArms.SetActive(false);
    }

    public void EquipArms(GunType gunType)
    {
        HideAllArms();

        switch (gunType)
        {
            case GunType.pistol:
                pistolArms.SetActive(true);
                pistolArms.GetComponent<ArmsScript>().PickupWeapon();
                pistolArms.GetComponent<ArmsScript>().isReloading = false;
                break;
            case GunType.assaultRifle:
                assaultRifleArms.SetActive(true);
                assaultRifleArms.GetComponent<ArmsScript>().PickupWeapon();
                assaultRifleArms.GetComponent<ArmsScript>().isReloading = false;
                break;
            case GunType.LMG:
                LMGArms.SetActive(true);
                LMGArms.GetComponent<ArmsScript>().PickupWeapon();
                LMGArms.GetComponent<ArmsScript>().isReloading = false;
                break;
            case GunType.shotgun:
                shotGunArms.SetActive(true);
                shotGunArms.GetComponent<ArmsScript>().PickupWeapon();
                shotGunArms.GetComponent<ArmsScript>().isReloading = false;
                break;
            default:
                break;
        }


    }

    void HideAllArms()
    {
        foreach (Transform child in transform)
        {
            if (child.tag == "Arms")
            {
                child.gameObject.GetComponent<ArmsScript>().SetOnEquip();
                child.gameObject.SetActive(false);
            }
        }
    }


    //void CalculateDirection()
    //{
    //    angle = Mathf.Atan2(Input.GetAxis(horizontalAxis), Input.GetAxis(verticalAxis));
    //    angle = Mathf.Rad2Deg * angle;
    //    angle += Camera.main.transform.eulerAngles.y;
    //}

    //void Rotate()
    //{
    //    targetRotation = Quaternion.Euler(0, 0, -angle);
    //    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
    //}


    private void OnDrawGizmos()
    {
        Vector2 shootDir = Vector2.right * Input.GetAxis(horizontalAxis) + Vector2.up * Input.GetAxis(verticalAxis);

        float angle = Vector2.SignedAngle(transform.position, shootDir);

        Ray ray = new Ray();
        ray.origin = transform.position;
        ray.direction = shootDir;
        Gizmos.DrawRay(ray);
    }


    public void TakeDamage(float damage, DamageType damageType, int attackerID)
    {
        if (!isDead)
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

        if (!isDead)
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

        StopAllCoroutines();
        EquipArms(GunType.pistol);

    }

    public void SetControllerNumber(int number)
    {
        playerID = number;

        //horizontalAxis = "J" + playerID + "Horizontal";
        //verticalAxis = "J" + playerID + "Vertical";
        foreach (Transform child in transform)
        {
            if (child.tag == "Arms")
            {
                child.GetComponent<ArmsScript>().triggerAXis = "J" + playerID + "Trigger";
                child.GetComponent<ArmsScript>().horizontalAxis = "J" + playerID + "Horizontal";
                child.GetComponent<ArmsScript>().verticalAxis = "J" + playerID + "Vertical";
            }
        }

    }

    //TODO: track mouse location in relation to center point of char
    //TODO: add negative force on click
    //TODO: raycast direction on click, check for hittables
    //TODO: deal dmg knockback, etc,
}
