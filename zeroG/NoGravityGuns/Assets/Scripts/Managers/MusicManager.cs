using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private static MusicManager _instance;

    public static MusicManager Instance { get { return _instance; } }

    AudioSource audioSource;

    bool Looping;
    public bool looping
    {
        get
        {
            return Looping;
        }
        set
        {
            Looping = value;
            audioSource.loop = Looping;
        }

    }


    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();


        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }


    }


    public static void PlaySong(AudioClip clip)
    {
        //if it's the same song just let it keep playing, else stop and play the new one
        if (clip != Instance.audioSource.clip)
            Instance.audioSource.Stop();
        else
            return;


        Instance.audioSource.PlayOneShot(clip);
        Instance.audioSource.clip = clip;
    }

    public static void PlaySong(AudioClip clip, bool isLooping)
    {
        //if it's the same song just let it keep playing, else stop and play the new one
        if (clip != Instance.audioSource.clip)
            Instance.audioSource.Stop();
        else
            return;

        Instance.audioSource.PlayOneShot(clip);
        Instance.looping = isLooping;
        Instance.audioSource.clip = clip;
    }

    public static bool IsPlaying()
    {
        return Instance.audioSource.isPlaying;
    }

}
