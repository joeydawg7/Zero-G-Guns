//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//[CreateAssetMenu(fileName = "Gun", menuName = "ScriptableObjects/Guns/Gun_burstShot", order = 1)]
//public class GunSO_burstShot : GunSO
//{
//    [Header("burst specific variables")]
//    public float timeBetweenBurstShots;
//    public int numBurstShots;
   

//    public override void Fire(PlayerScript player, Vector3 dir)
//    {
//        mono = player;

//        mono.StartCoroutine(FireInBurst(player, dir, numBurstShots));
//    }

//     protected override void SpawnBullet(Vector3 dir, PlayerScript player)
//     {
//         base.SpawnBullet(dir, player);
//     }
     
//    //shoots X number of bullet with Y time in between
//    IEnumerator FireInBurst(PlayerScript player, Vector3 dir, int bulletsToShoot)
//    {
//        ArmsScript arms = player.armsScript;

//        for (int i = 0; i < bulletsToShoot; i++)
//        {

//            SpawnBullet(dir, player);

//            ReduceBullets(player);

//            if (arms.audioSource.isPlaying)
//                arms.audioSource.Stop();

//            arms.audioSource.PlayOneShot(GetRandomGunshotSFX);

//            yield return new WaitForSeconds(timeBetweenBurstShots);
//        }
//    }
//}
