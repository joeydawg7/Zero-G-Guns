using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WackyGun : Guns
{
    public string name;
    public float knockBackModifier;
    //public int minDamageRange;
    //public int maxDamageRange;
    //public float bulletSpeed;

    public float delayBeforeShot;
    public List<Transform> bulletSpawns;
    private List<int> spawnIds = new List<int> { 0, 1, 2 };

    private void Start()
    {
       
    }

    public override void Fire(PlayerScript player)
    {
        if (CheckIfAbleToiFire(this))
        {            
            //player.armsScript.audioSource.PlayOneShot(GetRandomGunshotSFX);
            player.StartCoroutine(ShootThisGun(player));
            ReduceBullets(player);
        }
        else
        {
            CheckForAmmo(player);
        }       
    }

    public IEnumerator ShootThisGun(PlayerScript player)
    {
        var id = Random.Range(0, spawnIds.Count-1);
        player.armsScript.bulletSpawn = bulletSpawns[spawnIds[id]];
        spawnIds.Remove(spawnIds[id]);
        player.StartCoroutine(DelayShotCoroutine(player, delayBeforeShot, bulletSpeed, minDamageRange, maxDamageRange));
        ReduceBullets(player);
        yield return new WaitForSeconds(delayBeforeShot);
        id = Random.Range(0, spawnIds.Count - 1);
        player.armsScript.bulletSpawn = bulletSpawns[spawnIds[id]];
        spawnIds.Remove(spawnIds[id]);        
        player.StartCoroutine(DelayShotCoroutine(player, delayBeforeShot, bulletSpeed, minDamageRange, maxDamageRange));
        ReduceBullets(player);
        yield return new WaitForSeconds(delayBeforeShot);
        id = Random.Range(0, spawnIds.Count - 1);
        player.armsScript.bulletSpawn = bulletSpawns[spawnIds[id]];        
        player.StartCoroutine(DelayShotCoroutine(player, delayBeforeShot, bulletSpeed, minDamageRange, maxDamageRange));
        ReduceBullets(player);
        spawnIds.Clear();
        spawnIds.AddRange(new List<int> { 0, 1, 2 });
    }
}
