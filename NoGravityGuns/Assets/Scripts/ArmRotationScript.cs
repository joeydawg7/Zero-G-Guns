//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class ArmRotationScript : MonoBehaviour
//{

//    public PlayerScript basePlayer;


//    Vector2 shootDir;
//    Quaternion facing;
//    Quaternion rotation;

//    public string horizontalAxis;
//    public string verticalAxis;
//    public string triggerAXis;


//    public bool isReloading;
//    public bool canShoot;
//    public GunSO currentWeapon;

//    // Start is called before the first frame update
//    void Start()
//    {
        
//    }

//    // Update is called once per frame
//    void FixedUpdate()
//    {
//        if (GameManager.Instance.isGameStarted)
//            ShootController();
//    }


//    void ShootController()
//    {
//        if (!basePlayer.GetComponent<PlayerScript>().isDead)
//        {

//            shootDir = Vector2.right * Input.GetAxis(horizontalAxis) + Vector2.up * Input.GetAxis(verticalAxis);
//            // else
//            // shootDir = startingEulers;

//            shootDir = shootDir.normalized;

//            rotation = Quaternion.LookRotation(Vector3.forward, -shootDir);
//            rotation *= facing;
//            transform.rotation = rotation;

//            if (Input.GetAxisRaw(triggerAXis) > 0 && !isReloading)
//            {


//                if (shootDir.sqrMagnitude >= 0.1f)
//                {
//                    if (timeSinceLastShot >= currentWeapon.recoilDelay)
//                    {
//                        if (canShoot)
//                        {
//                            switch (currentWeapon.fireType)
//                            {
//                                case GunSO.FireType.semiAuto:
//                                    ShootyGunTemp();
//                                    break;
//                                case GunSO.FireType.buckshot:
//                                    BuckShot();
//                                    break;
//                                case GunSO.FireType.fullAuto:
//                                    ShootyGunTemp();
//                                    break;
//                                case GunSO.FireType.Burst:
//                                    StartCoroutine(FireInBurst());
//                                    break;
//                                default:
//                                    ShootyGunTemp();
//                                    break;
//                            }

//                            //add force to player in opposite direction of shot
//                            KnockBack(shootDir);
//                        }
//                    }
//                }

//            }
//        }
//    }
//}
