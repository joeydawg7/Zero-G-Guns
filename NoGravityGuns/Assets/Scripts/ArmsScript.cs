using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.InputSystem.PlayerInput;
using UnityEngine.InputSystem.Controls;
using UnityEngine.Experimental.U2D.IK;

public class ArmsScript : MonoBehaviour
{
    #region Publics

    [Header("Debug")]
    public bool showAimingVector;
    [Header("----------")]

    [HideInInspector]
    public PlayerScript basePlayer;

    [Header("Gun")]
    public GunSO currentWeapon;
    public float targetVectorLength;

    public Transform handBone;
    public Transform frontupperArmBone;

    [Header("Audio")]
    public AudioClip dryFire;

    public Transform IKTarget;
    // public Transform parentObject;

    #endregion

    #region Hidden Publics
    //Hidden Publics
    [HideInInspector]
    public int currentAmmo;
    [HideInInspector]
    public bool isReloading;
    [HideInInspector]
    public GameObject currentGun;
    [HideInInspector]
    public AudioSource audioSource;
    [HideInInspector]
    public CameraShake cameraShake;
    [HideInInspector]
    public float timeSinceLastShot;
    [HideInInspector]
    public Transform bulletSpawn;

    #endregion

    #region Privates
    //private
    Quaternion facing;
    Quaternion rotation;
    Vector2 shootDir;
    Color32 startingColor;
    Coroutine reloadCoroutine;
    Coroutine rotateCoroutine;
    Vector3 dir;
    GameManager gameManager;
    int totalBulletsGunCanLoad;
    LimbSolver2D IKLimbSolver;
    bool flipped = false;
    

    #endregion

    #region Start, Awake, Update
    private void Awake()
    {
        timeSinceLastShot = 0;

        facing = transform.rotation;
        currentAmmo = currentWeapon.clipSize;
        totalBulletsGunCanLoad = currentWeapon.numBullets;

        audioSource = GetComponent<AudioSource>();

        shootDir = new Vector3(-4f, 1f, 0);
        // handBone.right = new Vector2(IKTarget.transform.localPosition.x, IKTarget.transform.localPosition.y * -1) - new Vector2(shootDir.x * -1, shootDir.y);

        cameraShake = Camera.main.GetComponent<CameraShake>();

        if (IKTarget != null)
            IKLimbSolver = IKTarget.parent.GetComponent<LimbSolver2D>();

    }

    private void Start()
    {
        gameManager = GameManager.Instance;
    }

    private void Update()
    {
        if (gameManager.isGameStarted)
        {
            CountShotDelay();

            if (!basePlayer.isDead && !basePlayer.isDummy)
            {
                AimController();
                OnReload();
                ShootController();
            }
        }
    }
    #endregion


    #region Input Handler Functions
    void AimController()
    {
        //cant aim if we cant get an aim value from base player
        if (basePlayer == null || basePlayer.player == null)
            return;

        Vector2 rawAim = basePlayer.player.GetAxis2D("Move Horizontal", "Move Vertical");

        transform.position = frontupperArmBone.position;

        //targetVectorLength = rawAim.x*10f;

        //if we are aiming somewhere update everything, else we will hold on last known direction
        if (rawAim.magnitude > 0f)
        {
            // aiming stuff
            shootDir = -Vector2.right * rawAim + Vector2.up * rawAim;
            shootDir = shootDir.normalized * targetVectorLength;

            IKTarget.transform.localPosition = shootDir;
            handBone.right = new Vector2(IKTarget.transform.localPosition.x, IKTarget.transform.localPosition.y * -1) - new Vector2(shootDir.x * -1, shootDir.y) * Vector2.right;
        }

    }

    private void OnDrawGizmos()
    {

        if (showAimingVector && GameManager.Instance.debugManager.useDebugSettings)
        {
            Gizmos.color = Color.magenta;
            // DrawHelperAtCenter(shootDir, Color.red, 1f);
            if (IKTarget != null)
                Gizmos.DrawLine(transform.position, IKTarget.transform.position);
        }

    }

    public void OnReload()
    {
        //if (basePlayer.player.GetButtonDown("Reload"))
        //{
        //    // if (!isReloading && currentAmmo < currentWeapon.clipSize)
        //    //reloadCoroutine = StartCoroutine(Reload());
        //}
    }

    void ShootController()
    {
        if (gameManager.isGameStarted && basePlayer.player.GetAxis("Shoot") > 0.5f)
        {
            OnShoot();
        }
    }
    #endregion

    #region Equip Guns, Shoot
    //counts time between shots 
    void CountShotDelay()
    {
        //delay shooting stuff
        timeSinceLastShot += Time.deltaTime;

    }

    //equips a new gun
    public void EquipGun(GunSO weaponToEquip)
    {
        //get rid of all the other guns
        HideAllGuns();

        //spawn new gun
        GameObject gunGo = GameObject.Instantiate(weaponToEquip.gunPrefab, handBone);

        //set gun position and hand position
        GunPositionValue gunPosValue = gunGo.GetComponent<GunPositionValue>();
        gunGo.transform.localPosition = gunPosValue.position;
        gunPosValue.SetHandPositions(this);


        //set weapon and bullet stats for new gun
        currentWeapon = weaponToEquip;
        totalBulletsGunCanLoad = weaponToEquip.numBullets;
        currentAmmo = weaponToEquip.clipSize;

        isReloading = false;

        //find the new bulelt spawn location (bleh)
        bulletSpawn = gunGo.transform.Find("BulletSpawner");
        //update UI
        SendGunText();
    }

    void HideAllGuns()
    {

        for (int i = 0; i < handBone.childCount; i++)
        {
            if (handBone.GetChild(i).tag == "Gun")
                Destroy(handBone.GetChild(i).gameObject);
        }
    }

    public void OnShoot()
    {
        if (basePlayer.isDead || Time.timeScale !=1)
            return;

        //dry fire effect
        if (isReloading)
        {
            if (timeSinceLastShot >= currentWeapon.recoilDelay)
            {
                audioSource.PlayOneShot(dryFire);
                timeSinceLastShot = 0;
            }
        }

        //gotta have bullets to shoot
        if (currentAmmo > 0)
        {
            //enough time has passed between shots and not paused
            if (timeSinceLastShot >= currentWeapon.recoilDelay && Time.timeScale != 0)
            {
                //add force to player in opposite direction of shot
                currentWeapon.KnockBack(basePlayer, dir);

                //shoot gun based on weapons fire function
                currentWeapon.Fire(basePlayer, dir);


                basePlayer.shotsFired++;
            }
        }
    }
    #endregion

    #region UIStuff
    public string AmmoText()
    {
        return currentAmmo + "/" + currentWeapon.clipSize + " (" + ((totalBulletsGunCanLoad < 2000) ? totalBulletsGunCanLoad.ToString() : "\u221E") + ")";
    }

    public void SendGunText()
    {
        basePlayer.playerUIPanel.SetGunText(currentWeapon);
        basePlayer.playerUIPanel.SetAmmoText(AmmoText(), ((float)currentAmmo / (float)currentWeapon.clipSize));
    }

    public void SendGunText(string s)
    {
        basePlayer.playerUIPanel.SetAmmoText(s, ((float)currentAmmo / (float)currentWeapon.clipSize));
    }
    #endregion

    #region Helpers
    //helper function to draw vectors with gizmos
    private void DrawHelperAtCenter(Vector3 direction, Color color, float scale)
    {
        Gizmos.color = color;
        Vector3 destination = transform.position + direction * scale;
        Gizmos.DrawLine(bulletSpawn.transform.position, destination);
    }
    #endregion

}
