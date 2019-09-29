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
    public PlayerScript basePlayer;

    [Header("Gun")]
    public GunSO currentWeapon;
    //public GameObject reloadTimer;

    public Transform bulletSpawn;

    public float targetVectorLength;

    public Transform handBone;

    [Header("Audio")]
    public AudioClip dryFire;

    public Transform IKTarget;
    public Transform parentObject;

    #endregion

    #region Hidden Publics
    //Hidden Publics
    [HideInInspector]
    public int currentAmmo;
    [HideInInspector]
    public bool isReloading;
    [HideInInspector]
    public GameObject currentArms;
    [HideInInspector]
    public AudioSource audioSource;
    [HideInInspector]
    public CameraShake cameraShake;
    [HideInInspector]
    public float timeSinceLastShot;

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

    

    #endregion

    #region Start, Awake, Update
    private void Awake()
    {
        timeSinceLastShot = 0;

        facing = transform.rotation;
        currentAmmo = currentWeapon.clipSize;
        totalBulletsGunCanLoad = currentWeapon.numBullets;

        audioSource = GetComponent<AudioSource>();

        shootDir = new Vector3(0, 0, 0);

        cameraShake = Camera.main.GetComponent<CameraShake>();

        if(IKTarget!=null)  
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

    //  const float TARGET_VECTOR_LENGTH = 15f;

  // float flipStatus=1;

    #region Input Handler Functions
    void AimController()
    {
        Vector2 rawAim = basePlayer.player.GetAxis2D("Move Horizontal", "Move Vertical");

        //if we are aiming somewhere update everything, else we will hold on last known direction
        if (rawAim.magnitude > 0f)
        {
            // aiming stuff
            shootDir = -Vector2.right * rawAim + Vector2.up * rawAim;
            shootDir = shootDir.normalized * targetVectorLength;
            Transform ikParent = IKTarget.transform.parent;



            if (shootDir.x >= 0)
            {
                parentObject.rotation = Quaternion.Euler(parentObject.rotation.x, 180f, parentObject.rotation.z);
                IKLimbSolver.flip = true;
                // flipStatus = -1;
            }
            else
            {
                parentObject.rotation = Quaternion.Euler(parentObject.rotation.x, 0f, parentObject.rotation.z);
                IKLimbSolver.flip = false;
                //flipStatus = 1;
            }

            //Vector2 heading = ikParent.position - handBone.position;

            //var distance = heading.magnitude;
            //Vector2 direction = (heading / distance); // This is now the normalized direction.


            //rotation = Quaternion.LookRotation(Vector3.forward, shootDir * -1f);
            //rotation = handBone.rotation;
            //handBone.transform.rotation = rotation;

            //handBone.transform.LookAt(IKPos.transform.parent, handBone.up);

            //handBone.rotation = Quaternion.Euler(direction*-1f);

            IKTarget.transform.localPosition = shootDir;

            handBone.right = new Vector2(IKTarget.transform.localPosition.x, IKTarget.transform.localPosition.y *-1) - new Vector2(shootDir.x* -1, shootDir.y);

        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        // DrawHelperAtCenter(shootDir, Color.red, 1f);
        if (IKTarget != null)
            Gizmos.DrawLine(transform.position, IKTarget.transform.position);

    }

    public void OnReload()
    {
        if (basePlayer.player.GetButtonDown("Reload"))
        {
            // if (!isReloading && currentAmmo < currentWeapon.clipSize)
            //reloadCoroutine = StartCoroutine(Reload());
        }
    }

    void ShootController()
    {
        if (basePlayer.player.GetAxis("Shoot") > 0.5f)
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
    public void EquipGun(GunSO weaponToEquip, GameObject gunObj)
    {
        //set weapon and bullet stats for new gun
        currentWeapon = weaponToEquip;
        totalBulletsGunCanLoad = weaponToEquip.numBullets;
        currentAmmo = weaponToEquip.clipSize;

        isReloading = false;

        //find the new bulelt spawn location (bleh)
        bulletSpawn = gunObj.transform.Find("BulletSpawner");
        //update UI
        SendGunText();
    }

    public void OnShoot()
    {
        if (basePlayer.isDead)
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
