using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Rewired;
using XInputDotNetPure;
using UnityEngine.SceneManagement;

//stores info about the players controller
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

public class LocalJoiningPlayersScript : MonoBehaviour
{

    [Header("UI Elements")]
    public TextMeshProUGUI tipToStart;
    public JoinPanel[] joinPanels;
    public Image backIndicator;

    List<int> assignedControls;
    private int rewiredPlayerIdCounter = 0;

    [Header("Audio")]
    public AudioClip joinClick;
    public AudioClip unjoinClick;
    AudioSource audioSource;

    [Header("Player Data")]
    public GlobalPlayerSettingsSO globalPlayerSettings;
    public GameObject playerDataPrefab;
    public GameObject playerCanvasPrefab;


    Dictionary<int, PlayerControllerData> playerControllerDataDictionary;
    private float holdTimer;

    //consts
    const int MAX_PLAYERS = 4;


    private void Awake()
    {
        playerControllerDataDictionary = new Dictionary<int, PlayerControllerData>();
        audioSource = GetComponent<AudioSource>();
        assignedControls = new List<int>();

        // Subscribe to controller connected events
        ReInput.ControllerConnectedEvent += OnControllerConnected;
        ReInput.ControllerPreDisconnectEvent += OnControllerDisConnected;

        rewiredPlayerIdCounter = 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        PlayerDataScript[] playerDataScripts =  FindObjectsOfType<PlayerDataScript>();

        //get rid of the old playerData objects. If we're on this screen, its because we want new ones
        foreach (var pds in playerDataScripts)
        {
            Destroy(pds);
        }


        if (LoadingBar.Instance)
            LoadingBar.StopLoading();

        ControllerLayoutManager.SwapToUIMaps(false);

        //if we return to this screen from pause menu we do not want it showing here!
        if (PauseMenu.Instance)
            PauseMenu.Instance.MenuOff();

        holdTimer = 0;

        AssignAllJoysticksToSystemPlayer(true);
    }

    #region controller connect / disconnect
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
    #endregion

    void AssignAllJoysticksToSystemPlayer(bool removeFromOtherPlayers)
    {
        Debug.Log("assigning all to systemplayer!");
        foreach (var j in ReInput.controllers.Joysticks)
        {
            ReInput.players.GetSystemPlayer().controllers.AddController(j, removeFromOtherPlayers);
        }

        ControllerLayoutManager.SwapToUIMaps(false);
    }

    //gets the next player's id
    private int GetNextGamePlayerId()
    {
        return rewiredPlayerIdCounter++;
    }

    //assigns the next available controller to a player
    void AssignNextPlayer()
    {
        if (assignedControls.Count >= 4)
        {
            Debug.Log("Max player limit already reached!");
            return;
        }

        // Get the next Rewired Player Id
        int rewiredPlayerId = GetNextGamePlayerId();

        Debug.Log("rewird player id : " + rewiredPlayerId);

        // Get the Rewired Player
        Player rewiredPlayer = ReInput.players.GetPlayer(rewiredPlayerId);

        // Determine which Controller was used to generate the JoinGame Action
        Player systemPlayer = ReInput.players.GetSystemPlayer();
        var inputSources = systemPlayer.GetCurrentInputSources("UISubmit");

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

    //assigns a player to a joystick
    private void AssignJoystickToPlayer(Player player, Joystick joystick)
    {
        // Mark this joystick as assigned so we don't give it to the System Player again
        if (!assignedControls.Contains(joystick.id))
        {
            assignedControls.Add(joystick.id);
            //buzz joining controller so its easier to tell who just joined
            Vibrate(0.5f, 0.25f, joystick.id);

            //only play the sound if not contained, so we can tell if someone is joining when they are already in
            audioSource.PlayOneShot(joinClick);

            JoinPanel jp = joinPanels[joystick.id].GetComponent<JoinPanel>();

            if (jp.hasAssignedController == false)
            {
                PlayerControllerData playerControllerData = new PlayerControllerData(joystick.id, joystick);

                playerControllerDataDictionary.Add(playerControllerData.ID, playerControllerData);

                jp.AssignController();

                Debug.Log("Assigned " + joystick.name + " " + joystick.id + " to Player " + player.descriptiveName);
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
        var inputSources = systemPlayer.GetCurrentInputSources("UICancel");

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
            audioSource.PlayOneShot(unjoinClick);
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
        // 2 or more people allow game to start
        if (assignedControls.Count >= 2)
        {

            ReInput.players.SystemPlayer.controllers.ClearAllControllers();
            OnGameStart();
            SceneManager.LoadScene("Arena_PersistentScene");

        }


        tipToStart.alpha = 0;
    }

    void DebugInputs()
    {
        if (Input.GetKeyDown(KeyCode.F12))
        {
            //allow single player for testing
            if (assignedControls.Count >= 1)
            {
                ReInput.players.SystemPlayer.controllers.ClearAllControllers();
                OnGameStart();
                SceneManager.LoadScene("Arena_PersistentScene");
                
                //RoundManager.Instance.finishedControllerSetup = true;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        ShowOrHideTipToStart();

        //checks for actions from system players
        ManageControllerInputs();

        DebugInputs();

        if (this.gameObject.activeInHierarchy)
        {
            //Go back to main menu if B is held
            if (ReInput.players.GetSystemPlayer().GetButtonTimedPressDown("UICancel", 1.0f))
            {
                holdTimer = 0;
                SceneManager.LoadSceneAsync("Menu", LoadSceneMode.Single);
                LoadingBar.Instance.StartLoadingBar();
            }

            if (ReInput.players.GetSystemPlayer().GetButton("UICancel"))
            {
                holdTimer += Time.deltaTime;
            }

            if (ReInput.players.GetSystemPlayer().GetButtonUp("UICancel"))
            {
                holdTimer = 0;
            }

            backIndicator.fillAmount = holdTimer;
        }
    }

    private void ManageControllerInputs()
    {
        if (ReInput.players.GetSystemPlayer().GetButtonDown("UISubmit"))
        {
            AssignNextPlayer();
        }

        if (ReInput.players.GetSystemPlayer().GetButtonDown("UIStart"))
        {
            StartButtonPressed();
        }

        if (ReInput.players.GetSystemPlayer().GetButtonDown("UICancel"))
        {
            RemoveNextPlayer();
        }
    }

    private void ShowOrHideTipToStart()
    {
        //whether or not to show the "Press start" text
        if (assignedControls.Count > 1)
        {
            tipToStart.alpha = 1;
        }
        if (assignedControls.Count <= 1)
        {
            tipToStart.alpha = 0;
        }
    }

    public void OnGameStart()
    {
        //Debug.Log(playerControllerDataDictionary.Count);

        Debug.Log("onGameStart");

        globalPlayerSettings.SortPlayerSettings();


        foreach (var pCdataDic in playerControllerDataDictionary)
        {
            Debug.Log("controller: " + pCdataDic.Value.controller.name);

            SpawnPlayerManager(pCdataDic.Value);
        }

        for (int i = 0; i < assignedControls.Count; i++)
        {
            Vibrate(0.0f, 0.0f, i);
        }

        gameObject.SetActive(false);

        LoadingBar.StartLoading();
    }



    //call vibrateController coroutine
    public void Vibrate(float strength, float time, int controllerID)
    {
        if (vibrateController != null)
            StopCoroutine(vibrateController);
        vibrateController = StartCoroutine(VibrateController(strength, time, controllerID));
    }

    //buzz the controller
    Coroutine vibrateController;
    IEnumerator VibrateController(float strength, float time, int controllerID)
    {
        GamePad.SetVibration((PlayerIndex)controllerID, strength, strength);
        yield return new WaitForSecondsRealtime(time);
        GamePad.SetVibration((PlayerIndex)controllerID, 0, 0);
    }

    ////create a manager that stores all our player info we just got, save as don't destroy on load so we can use it to spawn players in
    ////the real round
    public void SpawnPlayerManager(PlayerControllerData playerControllerData)
    {
        PlayerDataScript PD = GameObject.Instantiate(playerDataPrefab).GetComponent<PlayerDataScript>();
        PD.gameObject.DontDestroyOnLoad();

        PD.SetPlayerInfoAfterRoundStart(playerControllerData, globalPlayerSettings, playerCanvasPrefab);

    }
}
