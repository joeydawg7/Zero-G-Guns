using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPad : MonoBehaviour
{
    public Sprite emptyPad;

    public bool hasWeapon;

    public GunSO weaponToSpawn;
    private GunSO currentWeapon;

    public float timeToNextSpawn;

    public float timer;

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
    }

    // Update is called once per frame
    void Update()
    {
        if(GameManager.Instance.isGameStarted)
            timer += Time.deltaTime;
        
        if(timer>= timeToNextSpawn)
        {
            hasWeapon = true;
            currentWeapon = weaponToSpawn;
            GetComponent<SpriteRenderer>().sprite = currentWeapon.weaponPad;
            
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Torso"  || collision.tag == "Head" || collision.tag == "Feet" && hasWeapon && currentWeapon != null)
        {
            //Debug.Log(collision.gameObject.name);

            collision.transform.parent.GetComponent<PlayerScript>().equipArms(currentWeapon.GunType);
            GetComponent<SpriteRenderer>().sprite = emptyPad;
            timer = 0;
            timeToNextSpawn = Random.Range(5, 25f);
        }
    }
}
