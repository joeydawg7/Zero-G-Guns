using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.InputSystem.PlayerInput;
using UnityEngine.InputSystem.Controls;
using UnityEngine.Experimental.U2D.IK;
using XInputDotNetPure;
public class ArmsScript : MonoBehaviour
{
    #region Publics

    [Header("Debug")]
    public bool showAimingVector;
    [Header("----------")]

    [HideInInspector]
    public PlayerScript basePlayer;

    [Header("Gun")]
    public Guns currentWeapon;
    public float targetVectorLength;

    public Transform frontHandBone;
    public Transform backHandBone;
    public Transform frontupperArmBone;
    public Transform backUpperArmBone;

    [Header("Audio")]
    public AudioClip dryFire;

    public Transform IKTarget;
    // public Transform parentObject;
    public Transform handConnectionPoint;

    #endregion

    #region Hidden Publics
    //Hidden Publics
    [HideInInspector]
    public int currentAmmo;
    [HideInInspector]
    public bool isReloading;
    [HideInInspector]
    public GameObject currentGunGameObject;
    [HideInInspector]
    public AudioSource audioSource;
    [HideInInspector]
    public CameraShake cameraShake;
    //[HideInInspector]
   // public float timeSinceLastShot;
    [HideInInspector]
    public Transform bulletSpawn;
    //public float timeYouCanHoldGun;

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

    Vector3 frontArmPos;
    Vector3 backArmPos;

    LineRenderer laserPointerLine;
    


    #endregion

    #region Start, Awake, Update
    private void Awake()
    {
        //timeSinceLastShot = Time.time;

        facing = transform.rotation;
        if(currentWeapon)
        {
            //timeYouCanHoldGun = currentWeapon.time;
        }
        else
        {
            //EquipGun(ObjectPooler.Instance.defaultPistol);           
        }

        //timeYouCanHoldGun = currentWeapon.time;

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
        EquipGun(gameManager.pistol, true);

        frontArmPos = frontupperArmBone.position;
        //backArmPos = backUpperArmBone.position;

        transform.parent = transform.parent.root;
        //frontupperArmBone.parent = transform.parent.root;
        //backUpperArmBone.parent = transform.parent.root;
    }

    private void Update()
    {
        if (gameManager.isGameStarted)
        {
            //CountShotDelay();

            if (!basePlayer.isDead)
            {
                //AimController();
                //OnReload();
                ShootController();

                //if(currentWeapon != GameManager.Instance.pistol)
                //{
                //    ReduceTimeToHoldGun();
                //}

            }
        }
    }

    private void LateUpdate()
    {
        //lock pos of the pivot to the shoulder bone
        transform.position = frontupperArmBone.position;

        if (gameManager.isGameStarted)
        {
            if (!basePlayer.isDead)
            {
                AimController();
                DrawLaserPointer();
            }
        }
    }



    void DrawLaserPointer()
    {
        if (laserPointerLine && bulletSpawn)
        {

            Vector2 drawLineFromPosition = bulletSpawn.transform.position;

            //laserPointerLine.transform.position = bulletSpawn.transform.position;
            laserPointerLine.SetPosition(0, drawLineFromPosition);

            RaycastHit2D laserPointerRay = Physics2D.Raycast(drawLineFromPosition, -bulletSpawn.transform.right);

            if (laserPointerRay.collider != null 
                && ( (!laserPointerRay.collider.CompareTag("Player")) 
                || (!laserPointerRay.collider.CompareTag("Legs"))
                || (!laserPointerRay.collider.CompareTag("Arms"))
                || (!laserPointerRay.collider.CompareTag("Torso"))
                || (!laserPointerRay.collider.CompareTag("Torso"))
                || (!laserPointerRay.collider.CompareTag("Tail"))
                )
                )
            {
                Vector2 hitpoint = laserPointerRay.point;
                laserPointerLine.SetPosition(1, hitpoint);
            }
        }
    }

    #endregion

    #region Input Handler Functions
    void AimController()
    {
        //cant aim if we cant get an aim value from base player
        if (basePlayer == null)
        {
            Debug.LogError("BasePlayer is null!");
            return;
        }

        if (basePlayer.player == null)
        {
            Debug.LogError("BasePlayer controller settings are null!");
            return;
        }

        Vector2 rawAim = new Vector2(0, 0);
        Vector2 rawAimLeft = basePlayer.player.GetAxis2D("Move Horizontal Left Stick", "Move Vertical Left Stick");
        Vector2 rawAimRight = basePlayer.player.GetAxis2D("Move Horizontal", "Move Vertical");

        //if there's any input from right stick use that one above all others
        if (rawAimRight.magnitude > 0)
        {
            rawAim = rawAimRight;
        }
        //else allow use of left stick
        else
        {
            rawAim = rawAimLeft;
        }

        //if we are aiming somewhere update everything, else we will hold on last known direction
        if (rawAim.magnitude > 0f && Time.timeScale == 1)
        {
            // draw line in direction of aiming vector
            shootDir = -Vector2.right * rawAim + Vector2.up * rawAim;
            //place IK target along that line at targetVectorLength distance (larger vector length = more accuracy but less good looking anim)
            shootDir = shootDir.normalized * targetVectorLength;
            IKTarget.transform.localPosition = shootDir;


        }

        //angle hand bone to point along shoot direction
        Vector2 frontHandBoneDirection = new Vector2(IKTarget.transform.localPosition.x, IKTarget.transform.localPosition.y * -1) - new Vector2(shootDir.x * -1, shootDir.y) * Vector2.right;
        //frontHandBone.right = frontHandBoneDirection;

        //Vector2 backandBoneDirection = new Vector2(IKTarget.transform.localPosition.x, IKTarget.transform.localPosition.y * -1) - new Vector2(shootDir.x * -1, shootDir.y) * Vector2.right;
        //backHandBone.right = backandBoneDirection;

        if (currentWeapon != null)
            currentWeapon.transform.right = -frontHandBoneDirection;

    }

    private void OnDrawGizmos()
    {

        if (showAimingVector)
        {
            Gizmos.color = Color.magenta;
            // DrawHelperAtCenter(shootDir, Color.red, 1f);
            if (IKTarget != null)
            {
                Gizmos.DrawLine(transform.position, IKTarget.transform.position);
                if (bulletSpawn != null)
                {
                    Gizmos.DrawLine(transform.position, -shootDir);
                }
            }
        }

        if (bulletSpawn)
        {
            Gizmos.DrawCube(bulletSpawn.transform.position, new Vector3(1, 1, 1));

        }

    }

    //public void OnReload()
    //{
        //if (basePlayer.player.GetButtonDown("Reload"))
        //{
        //    // if (!isReloading && currentAmmo < currentWeapon.clipSize)
        //    //reloadCoroutine = StartCoroutine(Reload());
        //}
    //}

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
    //void CountShotDelay()
    //{
    //    //delay shooting stuff
    //    timeSinceLastShot += Time.deltaTime;

    //}

    /// <summary>
    /// equips a new gun
    /// </summary>
    /// <param name="weaponToEquip"> the new weapon to equip</param>
    /// <param name="equipInstant">should I skip the usual delay and just equip this instantly?</param>
    public void EquipGun(Guns weaponToEquip, bool equipInstant)
    {
        //instant equip assumes time since last shot as being functionally infinite so player can grab weapon and shoot right away
        if (equipInstant)
        {
            weaponToEquip.timeSinceLastShot = Time.time - ((weaponToEquip.recoilDelay / 4.0f) * 3.0f);
        }
        else
        {
            weaponToEquip.timeSinceLastShot = Time.time +weaponToEquip.recoilDelay;
        }


        //delays the swapping of weapons so player will hold the old weapon for as long as the recoilDelay on the gun is before switching.
        EquipGun(weaponToEquip);


    }

    public Vector3 GetBulletSpawnPos()
    {
        //find the new bulelt spawn location (bleh)
        bulletSpawn = currentWeapon.gameObject.transform.Find("BulletSpawner");

        return bulletSpawn.position;
    }

    void EquipGun(Guns weaponToEquip)
    {
        if(weaponToEquip)
        {
            //get rid of all the other guns
            HideAllGuns();

            //spawn new gun
            GameObject gunGo = GameObject.Instantiate(weaponToEquip.gunPrefab, frontHandBone);

            currentGunGameObject = gunGo;

            //set gun position and hand position
            GunPositionValue gunPosValue = gunGo.GetComponent<GunPositionValue>();
            gunGo.transform.localPosition = gunPosValue.position;
            gunPosValue.SetHandPositions(this);


            //set weapon and bullet stats for new gun
            currentWeapon = gunGo.GetComponent<Guns>();
            //timeYouCanHoldGun = weaponToEquip.time;
            currentAmmo = weaponToEquip.clipSize;

            isReloading = false;


            GetBulletSpawnPos();

            laserPointerLine = currentWeapon.GetComponentInChildren<LineRenderer>();
            if (laserPointerLine)
            {

                //laserPointerLine.transform.position = bulletSpawn.transform.position;
                laserPointerLine.transform.SetParent(null);
                laserPointerLine.transform.localScale = new Vector3(1, 1, 1);
                laserPointerLine.startColor = basePlayer.playerColor;
                laserPointerLine.endColor = basePlayer.playerColor;
            }

            weaponToEquip.timeSinceLastShot = -weaponToEquip.recoilDelay;
            currentWeapon.canFire = true;

            //if (basePlayer)
            //{
            //    if (weaponToEquip == GameManager.Instance.pistol)
            //        basePlayer.playerCanvasScript.ShowGunTimer();
            //    else
            //        basePlayer.playerCanvasScript.HideGunTimer();
            //}

        }       
    }

    public void HideAllGuns()
    {

        for (int i = 0; i < frontHandBone.childCount; i++)
        {
            if (frontHandBone.GetChild(i).tag == "Gun")
            {
                frontHandBone.GetChild(i).gameObject.GetComponent<Guns>().StopAllCoroutines();
                Destroy(frontHandBone.GetChild(i).gameObject);
            }
        }
    }

    public void OnShoot()
    {
        if (basePlayer.isDead || Time.timeScale != 1)
            return;

        //dry fire effect
        //if (isReloading)
        //{
        //    if (timeSinceLastShot >= currentWeapon.recoilDelay)
        //    {
        //        audioSource.PlayOneShot(dryFire);
        //        timeSinceLastShot = Time.time;
        //        EquipGun(ObjectPooler.Instance.defaultPistol);
        //    }
        //}

        currentWeapon.Fire(basePlayer);       
    }

    //void ReduceTimeToHoldGun()
    //{
    //    timeYouCanHoldGun -= Time.deltaTime;

    //    currentWeapon.CheckForGunTimeout(basePlayer);
    //}

    #endregion

    #region UIStuff
    public string AmmoText()
    {
        // return currentAmmo + "/" + currentWeapon.clipSize + " (" + ((totalBulletsGunCanLoad < 2000) ? totalBulletsGunCanLoad.ToString() : "\u221E") + ")";
        return "";
    }

    //public void SendGunText()
    //{
    //    basePlayer.playerUIPanel.SetGunText(currentWeapon);
    //    basePlayer.playerUIPanel.SetAmmoText(AmmoText(), ((float)currentAmmo / (float)currentWeapon.clipSize));
    //}

    //public void SendGunText(string s)
    //{
    //    basePlayer.playerUIPanel.SetAmmoText(s, ((float)currentAmmo / (float)currentWeapon.clipSize));
    //}
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
