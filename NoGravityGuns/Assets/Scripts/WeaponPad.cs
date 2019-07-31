using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPad : MonoBehaviour
{

    [Header("DEBUG")]
    public bool SpawnSelectedWeaponInstant;

    [Header("Non-Debug")]

    public Sprite emptyPad;

    public bool hasWeapon;

    public GunSO weaponToSpawn;
    public GunSO currentWeapon;

    public float timeToNextSpawn;

    public float timer;

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
        if ((collision.tag == "Torso" || collision.tag == "Head" || collision.tag == "Feet" || collision.tag == "Leg") && hasWeapon && currentWeapon != null && !collision.transform.root.GetComponent<PlayerScript>().isDead)
        {
            PlayerScript player = collision.transform.root.GetComponent<PlayerScript>();

            if (currentWeapon.GunType == PlayerScript.GunType.healthPack)
            {
                player.audioSource.PlayOneShot(healthKitSFX);
                player.TakeDamage(currentWeapon.GunDamage, PlayerScript.DamageType.torso, null, false, PlayerScript.GunType.healthPack);

            }
            else
            {
                player.EquipArms(currentWeapon.GunType, currentWeapon);
                GetComponent<AudioSource>().PlayOneShot(pickupSFX);
            }

            GetComponent<SpriteRenderer>().sprite = emptyPad;
            timer = 0;
            timeToNextSpawn = Random.Range(5, 25f);
            weaponToSpawn = potentialGunsToSpawn[Random.Range(0, potentialGunsToSpawn.Count)];            
            currentWeapon = null;
            hasWeapon = false;

        }
    }

}
