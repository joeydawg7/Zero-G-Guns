﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem.Users;
using UnityEngine.InputSystem.PlayerInput;
using Rewired;
using UnityEngine.SceneManagement;
using XInputDotNetPure;

public struct PlayerControllerData
{
    public readonly int ID;
    public readonly Controller controller;

    public PlayerControllerData(int ID, Controller controller)
    {
        this.ID = ID;
        this.controller = controller;
    }
}

public class JoiningPlayerScript : MonoBehaviour
{
    //public Color32 p1Color;
    //public Color32 p2Color;
    //public Color32 p3Color;
    //public Color32 p4Color;


    public Color32 emptySlotColor;

    public TextMeshProUGUI tipToStart;

    public JoinPanel[] joinPanels;
    List<int> assignedControls;

    public int maxPlayers = 4;
    private int rewiredPlayerIdCounter = 0;

    public AudioClip joinClick;
    public AudioClip unjoinClick;

    PlayerSpawnPoint[] playerSpawnPoints;

    Dictionary<int, PlayerControllerData> playerControllerDataDictionary;

    public GameObject playerCanvas;
    private float holdTimer;
    public Image backIndecator;

    private void Awake()
    {
        tipToStart.alpha = 0;
        assignedControls = new List<int>();
        playerSpawnPoints = FindObjectsOfType<PlayerSpawnPoint>();
        //joinPanels = FindObjectsOfType<JoinPanel>();

        // Subscribe to controller connected events
        ReInput.ControllerConnectedEvent += OnControllerConnected;
        ReInput.ControllerPreDisconnectEvent += OnControllerDisConnected;

        playerControllerDataDictionary = new Dictionary<int, PlayerControllerData>();
        holdTimer = 0;
        try
        {
            for (int i = 0; i < ReInput.players.AllPlayers.Count; i++)
            {
                foreach (Joystick joystick in ReInput.players.AllPlayers[i].controllers.Joysticks)
                {
                    //ReInput.players.AllPlayers[i].controllers.maps.SetAllMapsEnabled(true);
                    ReInput.players.AllPlayers[i].controllers.maps.SetMapsEnabled(false, "UI");
                    ReInput.players.AllPlayers[i].controllers.maps.SetMapsEnabled(true, "Gameplay");
                    ReInput.players.AllPlayers[i].controllers.maps.SetMapsEnabled(true, "Default");
                    // Debug.Log(ReInput.players.AllPlayers[i].controllers.maps.ContainsMapInCategory("Gameplay") + " " + ReInput.players.AllPlayers[i].name);
                }
            }
        }
        catch
        {
            SceneManager.LoadSceneAsync("Arena_PersistentScene", LoadSceneMode.Single);
            //LoadingBar.Instance.StartLoadingBar();
        }


    }

    void Start()
    {
        ControllerLayoutManager.SwapToGameplayMaps();

        PauseMenu.Instance.MenuOff();

        try
        {

            if (!RoundManager.Instance.finishedControllerSetup)
            {
                gameObject.SetActive(true);
                AssignAllJoysticksToSystemPlayer(true);
            }
            else
            {
                gameObject.SetActive(false);
                //GameManager.Instance.StartGame();
                if (RoundManager.Instance.currentRound == 0)
                    RoundManager.Instance.NewRound(false);
            }

        }
        catch
        {
            SceneManager.LoadSceneAsync("Arena_PersistentScene", LoadSceneMode.Single);
            LoadingBar.Instance.StartLoadingBar();
        }

    }
    //TODO: move this to something that is on every scene no matter what
    void OnControllerDisConnected(ControllerStatusChangedEventArgs args)
    {
        if (args.controllerType != ControllerType.Joystick) return;
        if (assignedControls.Contains(args.controllerId)) RemoveJoystickFromPlayer(ReInput.controllers.GetJoystick(args.controllerId));

    }
    //TODO: move this to something that is on every scene no matter what
    void OnControllerConnected(ControllerStatusChangedEventArgs args)
    {
        if (args.controllerType != ControllerType.Joystick) return;

        // Check if this Joystick has already been assigned. If so, just let Auto-Assign do its job.
        if (assignedControls.Contains(args.controllerId)) return;

        // Joystick hasn't ever been assigned before. Make sure it's assigned to the System Player until it's been explicitly assigned
        ReInput.players.GetSystemPlayer().controllers.AddController(
            args.controllerType,
            args.controllerId,
            true // remove any auto-assignments that might have happened
        );
    }


    void AssignAllJoysticksToSystemPlayer(bool removeFromOtherPlayers)
    {
        Debug.Log("assigning all to systemplayer!");
        foreach (var j in ReInput.controllers.Joysticks)
        {
            ReInput.players.GetSystemPlayer().controllers.AddController(j, removeFromOtherPlayers);
        }
    }

    private int GetNextGamePlayerId()
    {
        return rewiredPlayerIdCounter++;
    }

    void AssignNextPlayer()
    {
        if (assignedControls.Count >= 4)
        {
            Debug.Log("Max player limit already reached!");
            return;
        }

        // Get the next Rewired Player Id
        int rewiredPlayerId = GetNextGamePlayerId();

        // Get the Rewired Player
        Player rewiredPlayer = ReInput.players.GetPlayer(rewiredPlayerId);

        // Determine which Controller was used to generate the JoinGame Action
        Player systemPlayer = ReInput.players.GetSystemPlayer();
        var inputSources = systemPlayer.GetCurrentInputSources("Join");

        foreach (var source in inputSources)
        {
            if (source.controllerType == ControllerType.Joystick)
            { // assigning a joystick

                // Assign the joystick to the Player. This will also un-assign it from System Player
                AssignJoystickToPlayer(rewiredPlayer, source.controller as Joystick);
                break;

            }
            else
            { // Custom Controller
                throw new System.NotImplementedException();
            }
        }
    }

    private void AssignJoystickToPlayer(Player player, Joystick joystick)
    {
        // Mark this joystick as assigned so we don't give it to the System Player again
        if (!assignedControls.Contains(joystick.id))
        {
            assignedControls.Add(joystick.id);
            //buzz joining controller so its easier to tell who just joined
            Vibrate(0.5f, 0.25f, joystick.id);
            //only play the sound if not contained, so we can tell if someone is joining when they are already in
            GameManager.Instance.audioSource.PlayOneShot(joinClick);

            JoinPanel jp = joinPanels[joystick.id].GetComponent<JoinPanel>();

            if (jp.hasAssignedController == false)
            {
                PlayerControllerData playerControllerData = new PlayerControllerData(joystick.id, joystick);

                playerControllerDataDictionary.Add(playerControllerData.ID, playerControllerData);

                jp.AssignController(playerControllerData.ID, playerControllerData.controller);

                //Debug.Log("Assigned " + joystick.name + " " + joystick.id + " to Player " + player.descriptiveName);
            }


        }
    }

    void RemoveNextPlayer()
    {
        //Debug.Log(rewiredPlayerIdCounter);
        if (assignedControls.Count <= 0 || rewiredPlayerIdCounter < 0)
        {
            //Debug.Log("Min player limit already reached!");
            return;
        }

        rewiredPlayerIdCounter--;
        // Get the next Rewired Player Id
        //int rewiredPlayerId = GetNextGamePlayerId();

        //// Get the Rewired Player
        //Player rewiredPlayer = ReInput.players.GetPlayer(rewiredPlayerId);

        // Determine which Controller was used to generate the Drop Action
        Player systemPlayer = ReInput.players.GetSystemPlayer();
        var inputSources = systemPlayer.GetCurrentInputSources("Drop");

        foreach (var source in inputSources)
        {
            //TODO: mouse and keyboard support?
            if (source.controllerType == ControllerType.Keyboard || source.controllerType == ControllerType.Mouse)
            { // Assigning keyboard/mouse

                // Assign KB/Mouse to the Player
                //AssignKeyboardAndMouseToPlayer(rewiredPlayer);

                //// Disable KB/Mouse Assignment category in System Player so it doesn't assign through the keyboard/mouse anymore
                //ReInput.players.GetSystemPlayer().controllers.maps.SetMapsEnabled(false, ControllerType.Keyboard, "Assignment");
                //ReInput.players.GetSystemPlayer().controllers.maps.SetMapsEnabled(false, ControllerType.Mouse, "Assignment");
                //break;

            }
            else if (source.controllerType == ControllerType.Joystick)
            { // assigning a joystick

                // Assign the joystick to the Player. This will also un-assign it from System Player
                RemoveJoystickFromPlayer(source.controller as Joystick);
                break;

            }
            else
            { // Custom Controller
                throw new System.NotImplementedException();
            }
        }
    }


    private void RemoveJoystickFromPlayer(Joystick joystick)
    {
        // Mark this joystick as assigned so we don't give it to the System Player again
        if (assignedControls.Contains(joystick.id))
        {
            assignedControls.Remove(joystick.id);
            //only play the sound if not contained, so we can tell if someone is joining when they are already in
            GameManager.Instance.audioSource.PlayOneShot(unjoinClick);
        }

        RemovePlayerControllerSetup(joystick.id, joystick);

        playerControllerDataDictionary.Remove(joystick.id);

        Debug.Log("Removed " + joystick.name + " from Player " + joystick.id);

        if (assignedControls.Count < 1)
            tipToStart.alpha = 0;
    }

    void RemovePlayerControllerSetup(int i, Controller controller)
    {
        JoinPanel jp = joinPanels[i].GetComponent<JoinPanel>();
        if (jp.hasAssignedController == true)
        {
            jp.UnAssignController();
            return;
        }
    }


    void StartButtonPressed()
    {
        //start game for real on a new round with start over = false
        //if(GameModeFlag.Instance)
        //{
        //    if (GameModeFlag.Instance.MultiPlayer)
        //    {
        if (assignedControls.Count >= 2)
        {
            ReInput.players.SystemPlayer.controllers.ClearAllControllers();

            RoundManager.Instance.NewRound(false);

            GameManager.Instance.StartGame();
            RoundManager.Instance.finishedControllerSetup = true;

        }
        else if (GameManager.Instance.debugManager.useDebugSettings)
        {
            //allow single player for testing
            if (assignedControls.Count >= 1)
            {
                ReInput.players.SystemPlayer.controllers.ClearAllControllers();

                RoundManager.Instance.NewRound(false);

                GameManager.Instance.StartGame();
                RoundManager.Instance.finishedControllerSetup = true;
            }
        }
        //}
        //else
        //{
        //    if (assignedControls.Count >= 1)
        //    {
        //        ReInput.players.SystemPlayer.controllers.ClearAllControllers();
        //        GameManager.Instance.StartGame();
        //        RoundManager.Instance.finishedControllerSetup = true;
        //        RoundManager.Instance.NewRound(false);
        //    }
        //}
        //}
        //else
        //{
        //    if (assignedControls.Count >= 2)
        //    {
        //        ReInput.players.SystemPlayer.controllers.ClearAllControllers();
        //        GameManager.Instance.StartGame();
        //        RoundManager.Instance.finishedControllerSetup = true;
        //        RoundManager.Instance.NewRound(false);
        //    }
        //    else if (GameManager.Instance.debugManager.useDebugSettings)
        //    {
        //        //allow single player for testing
        //        if (assignedControls.Count >= 1)
        //        {
        //            ReInput.players.SystemPlayer.controllers.ClearAllControllers();
        //            GameManager.Instance.StartGame();
        //            RoundManager.Instance.finishedControllerSetup = true;
        //            RoundManager.Instance.NewRound(false);
        //        }
        //    }
        //}      

        tipToStart.alpha = 0;
    }


    // Update is called once per frame
    void Update()
    {

        //if either of these things are null we can't proceed. probably broken from loading outside of persistent scene and will fix itself shortly :D
        if (!GameManager.Instance || !RoundManager.Instance)
            return;

        if (assignedControls.Count > 1)
        {
            tipToStart.alpha = 1;
        }
        if (assignedControls.Count <= 1)
        {
            tipToStart.alpha = 0;
        }

        if (!GameManager.Instance.isGameStarted && !RoundManager.Instance.finishedControllerSetup)
        {
            // Watch for JoinGame action in System Player
            if (ReInput.players.GetSystemPlayer().GetButtonDown("Join"))
            {
                AssignNextPlayer();
            }

            if (ReInput.players.GetSystemPlayer().GetButtonDown("Start"))
            {
                StartButtonPressed();
            }

            if (ReInput.players.GetSystemPlayer().GetButtonDown("Drop"))
            {
                RemoveNextPlayer();
            }
        }

        if (this.gameObject.activeInHierarchy)
        {
            if (ReInput.players.GetSystemPlayer().GetButtonTimedPressDown("Drop", 1.0f))
            {
                holdTimer = 0;
                SceneManager.LoadSceneAsync(0, LoadSceneMode.Single);
                LoadingBar.Instance.StartLoadingBar();
            }

            if (ReInput.players.GetSystemPlayer().GetButton("Drop"))
            {
                holdTimer += Time.deltaTime;
            }

            if (ReInput.players.GetSystemPlayer().GetButtonUp("Drop"))
            {
                holdTimer = 0;
            }

            backIndecator.fillAmount = holdTimer;
        }
    }


    public void OnGameStart()
    {
        //Debug.Log(playerControllerDataDictionary.Count);

        foreach (var pCdataDic in playerControllerDataDictionary)
        {

            Debug.Log("controller: " + pCdataDic.Value.controller.name);
            RoundManager.Instance.SpawnPlayerManager(pCdataDic.Value);
        }

        for (int i = 0; i < assignedControls.Count; i++)
        {
            Vibrate(0.0f, 0.0f, i);
        }

        gameObject.SetActive(false);
    }

    public void Vibrate(float strength, float time, int controllerID)
    {
        if (vibrateController != null)
            StopCoroutine(vibrateController);
        vibrateController = StartCoroutine(VibrateController(strength, time, controllerID));
    }

    Coroutine vibrateController;
    IEnumerator VibrateController(float strength, float time, int controllerID)
    {
        Debug.Log("vibrating for " + time + " seconds");
        GamePad.SetVibration((PlayerIndex)controllerID, strength, strength);
        yield return new WaitForSecondsRealtime(time);
        GamePad.SetVibration((PlayerIndex)controllerID, 0, 0);
    }

}
