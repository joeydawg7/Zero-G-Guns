using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPad : MonoBehaviour
{
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
        timer = 0;
        timeToNextSpawn = Random.Range(5, 25f);
        weaponToSpawn = potentialGunsToSpawn[Random.Range(0, potentialGunsToSpawn.Count)];
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
        if ((collision.tag == "Torso" || collision.tag == "Head" || collision.tag == "Feet") && hasWeapon && currentWeapon != null && !collision.transform.parent.GetComponent<PlayerScript>().isDead)
        {
            PlayerScript player = collision.transform.parent.GetComponent<PlayerScript>();

            if (currentWeapon.GunType == PlayerScript.GunType.healthPack)
            {
                if (player.health < 100)
                {
                    player.TakeDamage(currentWeapon.GunDamage, PlayerScript.DamageType.torso, 0, healthKitSFX);
                }
            }
            else
            {
                player.EquipArms(currentWeapon.GunType);

            }

            GetComponent<SpriteRenderer>().sprite = emptyPad;
            timer = 0;
            timeToNextSpawn = Random.Range(5, 25f);
            weaponToSpawn = potentialGunsToSpawn[Random.Range(0, potentialGunsToSpawn.Count)];
            GetComponent<AudioSource>().PlayOneShot(pickupSFX);
            currentWeapon = null;
            hasWeapon = false;

        }
    }

   /* private void  (Collider2D collision)
    {
       
    }*/
}
