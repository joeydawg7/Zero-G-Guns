//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//[CreateAssetMenu(fileName = "Gun", menuName = "ScriptableObjects/Guns/Gun_buckShot", order = 1)]
//public class GunSO_buckShot : GunSO
//{

//    [Header("buckshot specific variables")]
//    public float spreadAngleLimit;


//    public override void Fire(PlayerScript player, Vector3 dir)
//    {
//        BuckShot(player, dir);
//    }

//    protected override void SpawnBullet(Vector3 dir, PlayerScript player)
//    {
//        base.SpawnBullet(dir, player);
//    }

//    void BuckShot(PlayerScript player, Vector3 dir)
//    {
//        ArmsScript arms = player.armsScript;

//        SpawnBuckShot(player);

//        if (arms.audioSource.isPlaying)
//            arms.audioSource.Stop();

//        arms.audioSource.PlayOneShot(GetRandomGunshotSFX);

//        player.StartCoroutine(DelayShotCoroutine(player, dir));

//        ReduceBullets(player);

//    }

//    void SpawnBuckShot(PlayerScript player)
//    {

//        Transform bulletSpawn = player.armsScript.bulletSpawn;

//        for (int i = 0; i < UnityEngine.Random.Range(5, 8); i++)
//        {
//            // randomize angle variation between bullets
//            float spreadAngle = UnityEngine.Random.Range(
//           -spreadAngleLimit,
//           spreadAngleLimit);

//            GameObject bulletGo = ObjectPooler.Instance.SpawnFromPool("BuckShot", bulletSpawn.transform.position, Quaternion.identity);
//            var dir = bulletSpawn.transform.right;

//            float rotateAngle = spreadAngle +
//           (Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);

//            Vector2 MovementDirection = new Vector2(
//          Mathf.Cos(rotateAngle * Mathf.Deg2Rad),
//          Mathf.Sin(rotateAngle * Mathf.Deg2Rad)).normalized;

//            MovementDirection *= bulletSpeed;

//            bulletGo.GetComponent<Bullet>().Construct(GunDamage, player, MovementDirection, player.playerColor);

//        }

//    }
//}
