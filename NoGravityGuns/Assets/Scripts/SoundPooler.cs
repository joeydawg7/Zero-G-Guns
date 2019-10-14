using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPooler : MonoBehaviour
{
    private static SoundPooler _instance;
    public static SoundPooler Instance { get { return _instance; } }

    public int numSourcesToSpawn;
    public GameObject audioSourceObject;
    List<AudioSource> audioSources;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        audioSources = new List<AudioSource>();

        //spawn all our empty audioSources
        for (int i = 0; i < numSourcesToSpawn; i++)
        {
            audioSources.Add(Instantiate(audioSourceObject, transform).GetComponent<AudioSource>());
        }
    }

    public void PlaySoundEffect(AudioClip clipToPlay)
    {
        foreach (var source in audioSources)
        {
            //find a source that isnt playing
            if (!source.isPlaying)
            {
                source.PlayOneShot(clipToPlay);
                //only way out
                return;
            }
        }
    }

}
