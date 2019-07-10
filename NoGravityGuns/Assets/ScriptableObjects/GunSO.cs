using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Gun", menuName = "ScriptableObjects/Gun", order = 1)]
public class GunSO : ScriptableObject
{
    public string name;
    public int minDamageRange;
    public int maxDamageRange;
    public float knockback;
    public float bulletSpeed;
    public float recoilDelay;
    public float recoilPerShot;
    public float recoilMax;
    public int clipSize;
    public int clipNum;
    public int reloadTime;


    public Sprite weaponPad;

    public enum FireType { semiAuto, buckshot, fullAuto, Burst  };
    public FireType fireType;

    public PlayerScript.GunType GunType;

    public float cameraShakeDuration;

    public List<AudioClip> bulletSounds;
    public AudioClip reloadSound;

    public GameObject projectile;


    public AudioClip GetRandomGunshotSFX
    {
        get { return bulletSounds[Random.Range(0, bulletSounds.Count)];  }
    }

    public int GunDamage
    {
        get { return Random.Range(minDamageRange, maxDamageRange); }
    }

}
