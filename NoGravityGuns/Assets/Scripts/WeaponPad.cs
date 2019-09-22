using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WeaponPad : MonoBehaviour
{

    [Header("DEBUG")]
    public bool SpawnSelectedWeaponInstant;

    [Header("Non-Debug")]

    public Sprite emptyPad;

    public bool hasWeapon;

    public GunSO weaponToSpawn;
    public GunSO currentWeapon;
    float timeToNextSpawn;
    float timer;

    public List<GunSO> potentialGunsToSpawn;
    public AudioClip pickupSFX;
    public AudioClip healthKitSFX;

    private void Awake()
    {
        hasWeapon = false;
        currentWeapon = null;
    }

    // Start is called before the first frame update
    void Start()
    {
        potentialGunsToSpawn.Clear();
        var guns = Resources.LoadAll("ScriptableObjects/Guns", typeof(ScriptableObject)).Cast<GunSO>().ToArray();
        foreach (var g in guns)
            potentialGunsToSpawn.Add(g);

            if (!SpawnSelectedWeaponInstant)
        {
            timer = 0;
            timeToNextSpawn = Random.Range(5, 25f);
            weaponToSpawn = potentialGunsToSpawn[Random.Range(0, potentialGunsToSpawn.Count)];
        }
        else
        {
            hasWeapon = true;
            currentWeapon = weaponToSpawn;
            GetComponent<SpriteRenderer>().sprite = currentWeapon.weaponPad;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.isGameStarted)
            timer += Time.deltaTime;

        if (timer >= timeToNextSpawn)
        {
            hasWeapon = true;
            currentWeapon = weaponToSpawn;
            GetComponent<SpriteRenderer>().sprite = currentWeapon.weaponPad;

        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        //hit a player who isnt a floating corpse, and the pad has a weapon to give
        if ((collision.tag == "Torso" || collision.tag == "Head" || collision.tag == "Feet" || collision.tag == "Leg") && hasWeapon && currentWeapon != null && !collision.transform.root.GetComponent<PlayerScript>().isDead)
        {
            //just saving this for later
            PlayerScript player = collision.transform.root.GetComponent<PlayerScript>();

            //special case is a health pack which is not a gun
            if (currentWeapon.name == "HealthPack")
            {
                player.audioSource.PlayOneShot(healthKitSFX);
                player.TakeDamage(currentWeapon.GunDamage, PlayerScript.DamageType.torso, null, false);

            }
            else
            {
                //equip the new gun and play the sound
                player.EquipArms(currentWeapon);
                GetComponent<AudioSource>().PlayOneShot(pickupSFX);
            }

            //reset EVERYTHING
            GetComponent<SpriteRenderer>().sprite = emptyPad;
            timer = 0;
            timeToNextSpawn = Random.Range(5, 25f);
            weaponToSpawn = potentialGunsToSpawn[Random.Range(0, potentialGunsToSpawn.Count)];            
            currentWeapon = null;
            hasWeapon = false;

        }
    }

}
