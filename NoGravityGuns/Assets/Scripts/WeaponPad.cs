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

    int potentialSpawnsCount;

    public Dictionary<GunSO, float> weightedPotentialGunsToSpawn;
    //WeightedRandomBag<GunSO> weightedRandomBag;

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
        weightedPotentialGunsToSpawn = new Dictionary<GunSO, float>();
        //weightedRandomBag = new WeightedRandomBag<GunSO>();

        potentialGunsToSpawn.Clear();
        var guns = Resources.LoadAll("ScriptableObjects/Guns", typeof(ScriptableObject)).Cast<GunSO>().ToArray();
        potentialSpawnsCount = guns.Count();

        foreach (var g in guns)
            potentialGunsToSpawn.Add(g);

        //set initial weighted value to equal across all weapons
        foreach (var g in guns)
            // weightedRandomBag.AddEntry(g, guns.Count() / 100f);
            weightedPotentialGunsToSpawn.Add(g, guns.Count() / 100f);


        if (!SpawnSelectedWeaponInstant)
        {
            timer = 0;
            timeToNextSpawn = Random.Range(5, 25f);
            //weaponToSpawn = potentialGunsToSpawn[Random.Range(0, potentialGunsToSpawn.Count)];
            SetNewWeaponToSpawn();
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

            SetNewWeaponToSpawn();

            //reset EVERYTHING
            GetComponent<SpriteRenderer>().sprite = emptyPad;
            timer = 0;
            timeToNextSpawn = Random.Range(5, 25f);

            currentWeapon = null;
            hasWeapon = false;

        }
    }

    void SetNewWeaponToSpawn()
    {
        float accumulatedWeight = 0;

        for (int i = 0; i < potentialGunsToSpawn.Count; i++)
        {
            GunSO g = potentialGunsToSpawn[i];

            weightedPotentialGunsToSpawn[g] = ((weightedPotentialGunsToSpawn.Count / 100f) * g.spawnRateOverTime.Evaluate(RoundManager.Instance.timeSinceRoundStarted / 60f) * 100f);

            if (weightedPotentialGunsToSpawn[g] < 0)
                weightedPotentialGunsToSpawn[g] = 0;

            accumulatedWeight += weightedPotentialGunsToSpawn[g];
            Debug.Log("evaluated at time " + RoundManager.Instance.timeSinceRoundStarted + " , evaluation: " + g.spawnRateOverTime.Evaluate(RoundManager.Instance.timeSinceRoundStarted / 60f) * 100f);
            Debug.Log(g.name + " current potential spawn rate: " + weightedPotentialGunsToSpawn[g]);
        }


        Debug.Log("accumulated weight: " + accumulatedWeight);
        weaponToSpawn = GetRandom(accumulatedWeight);
        Debug.Log("next weapon will be a: " + weaponToSpawn.name);
    }


    GunSO GetRandom(float accumulatedWeight)
    {

        System.Random rand = new System.Random();

        double rnd = rand.NextDouble()* accumulatedWeight;
        Debug.Log("rolled: " + rnd);
        return  weightedPotentialGunsToSpawn.First(i => (rnd -= i.Value) < 0).Key;

        //System.Random rand = new System.Random();

        //double r = rand.NextDouble() * accumulatedWeight;

        //Debug.Log("R = " + r);

        //foreach (var kvp in weightedPotentialGunsToSpawn)
        //{
        //    if (kvp.Value <= r)
        //    {
        //        return kvp.Key;
        //    }
        //}

        //Debug.LogError("No Guns found in weighted dictionary!");
        //return null;

        //return default(T); //should only happen when there are no entries
    }



}
