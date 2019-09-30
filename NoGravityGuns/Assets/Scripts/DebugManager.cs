using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugManager : MonoBehaviour
{
    #region singleton stuff
    private static DebugManager _instance;
    public static DebugManager Instance { get { return _instance; } }
    #endregion

    [Header("Set to false before making builds to turn off all debug settings in one blow")]
    public bool useDebugSettings;

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
