using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class WeaponPad : MonoBehaviour
{

    [Header("DEBUG")]
    public bool SpawnSelectedWeaponInstant;

    [Header("Non-Debug")]

    public SpriteRenderer barSprite;
    public SpriteRenderer gunSprite;

    public bool hasWeapon;

    public Guns weaponToSpawn;
    public Guns currentWeapon;
    float timeToNextSpawn;
    float timer;

    [HideInInspector]
    public List<Guns> potentialGunsToSpawn;
    public AudioClip pickupSFX;
    public AudioClip healthKitSFX;
    public AudioClip pickupDisabled;

    private static Color emptyPadBarsColour = new Color(0, 1.0f, 1.0f);
    private static Color fullPadBarsColour = new Color(0.25f, 1.0f, 0.0f);
    private static Color disabledPadBarsColour = Color.red;

    

    private void Awake()
    {
        hasWeapon = false;
        currentWeapon = null;
        barSprite.color = new Color(0, 1.0f, 1.0f);

        try
        {
            potentialGunsToSpawn = ObjectPooler.Instance.potentialGunsToSpawn;
        }
        catch
        {
            SceneManager.LoadSceneAsync("PersistentScene", LoadSceneMode.Single);
            if(LoadingBar.Instance)
                LoadingBar.Instance.StartLoadingBar();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //debug spawn weapons
        if (SpawnSelectedWeaponInstant && GameManager.Instance.debugManager.useDebugSettings)
        {
            hasWeapon = true;
            currentWeapon = weaponToSpawn;
            barSprite.color = emptyPadBarsColour;
            gunSprite.sprite = weaponToSpawn.theGunSprite;
        }
        //spawn normally
        else
        {
            timer = 0;
            timeToNextSpawn = Random.Range(5f, 10f);
            weaponToSpawn = potentialGunsToSpawn[Random.Range(0, potentialGunsToSpawn.Count)];

          
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.isGameStarted && Time.timeScale ==1)
            timer += Time.deltaTime;

        if (timer >= timeToNextSpawn)
        {
            hasWeapon = true;
            currentWeapon = weaponToSpawn;
            barSprite.color = fullPadBarsColour;
            gunSprite.sprite = weaponToSpawn.theGunSprite;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {

        if (Time.timeScale <= 0)
            return;

        //hit a player who isnt a floating corpse, and the pad has a weapon to give
        if ((collision.tag == "Torso" || collision.tag == "Head" || collision.tag == "Feet" || collision.tag == "Leg") && hasWeapon && currentWeapon != null)
        {
            PlayerScript player = collision.transform.root.GetComponentInChildren<PlayerScript>();

            //they dead we dont need to deal with this crap
            if (player.isDead)
                return;

            //special case is a health pack which is not a gun
            if (currentWeapon.name == "HealthPack")
            {
                player.audioSource.PlayOneShot(healthKitSFX);
                player.TakeDamage(currentWeapon.GunDamage(currentWeapon.minDamageRange, currentWeapon.maxDamageRange), new Vector2(0,0), PlayerScript.DamageType.torso, null, false, null);

            }
            else
            {
                //equip the new gun and play the sound
                player.armsScript.EquipGun(currentWeapon,true);
                GetComponent<AudioSource>().PlayOneShot(pickupSFX);
            }

            //reset EVERYTHING
            gunSprite.sprite = null;
            barSprite.color = emptyPadBarsColour;
            timer = 0;
            timeToNextSpawn = Random.Range(5, 25f);
            weaponToSpawn = potentialGunsToSpawn[Random.Range(0, potentialGunsToSpawn.Count)];
            currentWeapon = null;
            hasWeapon = false;
        }
        else if ((collision.tag == "Torso" || collision.tag == "Head" || collision.tag == "Feet" || collision.tag == "Leg") && !hasWeapon)
        {

            PlayerScript player = collision.transform.root.GetComponentInChildren<PlayerScript>();

            //they dead we dont need to deal with this crap
            if (player.isDead  || Time.timeScale < 1)
                return;

            if (!player.audioSource.isPlaying)
            {
                player.audioSource.PlayOneShot(pickupDisabled);
            }
            barSprite.color = disabledPadBarsColour;
            timeToNextSpawn += Time.deltaTime;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (Time.timeScale <= 0)
            return;

        if ((collision.tag == "Torso" || collision.tag == "Head" || collision.tag == "Feet" || collision.tag == "Leg"))
        {

            PlayerScript player = collision.transform.root.GetComponentInChildren<PlayerScript>();

            //they dead we dont need to deal with this crap
            if (player.isDead)
                return;

            barSprite.color = emptyPadBarsColour;
        }
    }

}
