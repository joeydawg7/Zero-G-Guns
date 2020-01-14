using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameModeFlag : MonoBehaviour
{
    #region singleton stuff
    private static GameModeFlag _instance;
    public static GameModeFlag Instance { get { return _instance; } }
    #endregion

    private AudioSource music;
    private bool multiPlayer;
    public bool MultiPlayer { get; set; }

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
        music = this.gameObject.GetComponent<AudioSource>();
    }

    public void PlayMusic()
    {
        if(music)
        {
            if(music.clip)
            {
                music.Stop();
                music.Play();
            }
        }
    }

    public void SetMusicClip(AudioClip song)
    {
        music.Stop();
        music.clip = song;
    }
}
