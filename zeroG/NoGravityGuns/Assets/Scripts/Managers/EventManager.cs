using UnityEngine;
using System.Collections;
using Rewired;

enum Spawners
{
    Player1,
    Player2,
    Player3,
    Player4
}


public class EventManager : MonoBehaviour
{

    #region Init Custom Events

    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(Instance.gameObject);
            Instance = this;
        }

        DontDestroyOnLoad(this.gameObject);
    }

    public static EventManager Instance;

    #endregion

    #region JoinAction

    public delegate void JoinAction();
    public static event JoinAction OnJoin;

    public void PlayerJoined()
    {
        if (OnJoin != null)
        {
            OnJoin();
        }
    }

    #endregion

    #region Pressed Start Action

    public delegate void PressedStartAction();
    public static event PressedStartAction OnPressedStart;

   

    public void PressedStart()
    {
        if (OnPressedStart != null)
        {
            OnPressedStart();
        }
    }

    #endregion

}