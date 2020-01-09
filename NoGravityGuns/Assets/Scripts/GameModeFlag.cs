using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameModeFlag : MonoBehaviour
{
    #region singleton stuff
    private static GameModeFlag _instance;
    public static GameModeFlag Instance { get { return _instance; } }
    #endregion

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
    }
}
