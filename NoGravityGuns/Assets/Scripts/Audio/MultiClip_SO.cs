using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "MultiClip", menuName = "ScriptableObjects/Utility/MultiClip", order = 1)]

public class MultiClip_SO : ScriptableObject
{
    public AudioClip[] clips;

    public AudioClip GetRandomClip()
    {
        return clips[Random.Range(0, clips.Length)];
    }
}
