using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Rewired;
using XInputDotNetPure;
using UnityEngine.SceneManagement;
using Photon.Pun;
using System.IO;
using UnityEngine.Events;

public class NetworkJoiningPlayerScript : MonoBehaviourPunCallbacks/*,  IPunObservable*/
{

    //#region IPunObservable implementation

    //public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    //{
    //    if (stream.IsWriting)
    //    {
    //        // We own this player: send the others our data
    //        stream.SendNext(IsFiring);
    //    }
    //    else
    //    {
    //        // Network player, receive data
    //        this.IsFiring = (bool)stream.ReceiveNext();
    //    }
    //    if (stream.IsWriting)
    //    {
    //        // We own this player: send the others our data
    //        stream.SendNext(IsFiring);
    //        stream.SendNext(Health);
    //    }
    //    else
    //    {
    //        // Network player, receive data
    //        this.IsFiring = (bool)stream.ReceiveNext();
    //        this.Health = (float)stream.ReceiveNext();
    //    }
    //}

    //#endregion

    public static NetworkJoiningPlayerScript Instance;

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

    //[Header("Player Data")]
    //public GlobalPlayerSettingsSO globalPlayerSettings;
    //public GameObject playerDataPrefab;
    //public GameObject playerCanvasPrefab;

    public GameObject NetworkUserDataPrefab;

    //public List<KeyValuePair<string, PlayerControllerData>> playerControllerDataDictionary;
    private float holdTimer;

    //consts
    const int MAX_PLAYERS = 4;

    private void Awake()
    {
        PhotonNetwork.Instantiate(Path.Combine("Network Prefabs", NetworkUserDataPrefab.name), transform.position, Quaternion.identity);
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            if (Instance != this)
            {
                Destroy(Instance);
                Instance = this;
            }
        }

        //playerControllerDataDictionary = new List<KeyValuePair<string, PlayerControllerData>>();
        audioSource = GetComponent<AudioSource>();
        assignedControls = new List<int>();

        // Subscribe to controller connected events
        ReInput.ControllerConnectedEvent += OnControllerConnected;
        ReInput.ControllerPreDisconnectEvent += OnControllerDisConnected;

        ControllerLayoutManager.SwapToUIMaps(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        if (LoadingBar.Instance)
            LoadingBar.StopLoading();

        //if we return to this screen from pause menu we do not want it showing here!
        if (PauseMenu.Instance)
            PauseMenu.Instance.MenuOff();

        holdTimer = 0;

        //AssignAllJoysticksToSystemPlayer(true);
    }

    public override void OnDisable()
    {
        EventManager.OnPressedStart -= StartButtonPressed;
    }

    public override void OnEnable()
    {
        EventManager.OnPressedStart += StartButtonPressed;
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

    //assigns the next available controller to a player
    public void ChangeColourOfIcon(int playerDataID)
    {
       

        //only play the sound if not contained, so we can tell if someone is joining when they are already in
        audioSource.PlayOneShot(joinClick);

        //JoinPanel jp = joinPanels[PV.Owner.ActorNumber - 1].GetComponent<JoinPanel>();
        JoinPanel jp = joinPanels[playerDataID].GetComponent<JoinPanel>();
        if (jp.hasAssignedController == false)
        {
            jp.AssignController();          
        }                   
    }

    //void RemoveNextPlayer()
    //{
    //    //Debug.Log(rewiredPlayerIdCounter);
    //    if (assignedControls.Count <= 0 || rewiredPlayerIdCounter < 0)
    //    {
    //        //Debug.Log("Min player limit already reached!");
    //        return;
    //    }

    //    rewiredPlayerIdCounter--;
    //    // Get the next Rewired Player Id
    //    //int rewiredPlayerId = GetNextGamePlayerId();

    //    //// Get the Rewired Player
    //    //Player rewiredPlayer = ReInput.players.GetPlayer(rewiredPlayerId);

    //    // Determine which Controller was used to generate the Drop Action
    //    Player systemPlayer = ReInput.players.GetSystemPlayer();
    //    var inputSources = systemPlayer.GetCurrentInputSources("UICancel");

    //    foreach (var source in inputSources)
    //    {
    //        //TODO: mouse and keyboard support?
    //        if (source.controllerType == ControllerType.Keyboard || source.controllerType == ControllerType.Mouse)
    //        { // Assigning keyboard/mouse

    //        }
    //        else if (source.controllerType == ControllerType.Joystick)
    //        { // assigning a joystick

    //            // Assign the joystick to the Player. This will also un-assign it from System Player
    //            RemoveJoystickFromPlayer(source.controller as Joystick);
    //            break;
    //        }
    //        else
    //        { // Custom Controller
    //            throw new System.NotImplementedException();
    //        }
    //    }
    //}

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

        //playerControllerDataDictionary.Remove(joystick.id);

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

    private void ManageControllerInputs()
    {

        //Debug.Log(ReInput.players.GetSystemPlayer().descriptiveName);        
        //if (ReInput.players.GetSystemPlayer().GetButtonDown("UISubmit"))
        //{
            
        //    Debug.Log("A Press Detected by Player Join Script");
        //    EventManager.Instance.PlayerJoined();
        //}

        //if (ReInput.players.GetSystemPlayer().GetButtonDown("UICancel"))
        //{           
        //   //RemoveNextPlayer();           
        //}

        if (ReInput.players.GetSystemPlayer().GetButtonDown("UIStart"))
        {
            if (PhotonNetwork.IsMasterClient)
            {
                EventManager.Instance.PressedStart();
            }
        }
    }

    public void StartButtonPressed()
    {         
        if (PhotonNetwork.IsMasterClient)
        {
            ReInput.players.SystemPlayer.controllers.ClearAllControllers();
            OnGameStart();
            PhotonNetwork.LoadLevel("Networking_PersistentScene");

        }
        else
        {
            Debug.Log("Can't Start into scene You are not Master Client");
        }

        tipToStart.alpha = 0;
    }

    void DebugInputs()
    {
        if (Input.GetKeyDown(KeyCode.F12))
        {
            //allow single player for testing
           if(PhotonNetwork.IsMasterClient)
           {
                ReInput.players.SystemPlayer.controllers.ClearAllControllers();
                OnGameStart();
                PhotonNetwork.LoadLevel("Networking_PersistentScene");
           }
           else
            {
                Debug.Log("Can't Start into scene You are not Master Client");
            }
              

           
        }
    }

    // Update is called once per frame
    void Update()
    {
        ShowOrHideTipToStart();
        ManageControllerInputs();
        DebugInputs();       
    }

    
    private void ShowOrHideTipToStart()
    {
        //whether or not to show the "Press start" text
        //if (playersInGame == playersInRoom)
        //{
            tipToStart.alpha = 1;
        //}
        //else
        //{
        //    tipToStart.alpha = 0;
        //}
    }

    public void OnGameStart()
    {
        //Debug.Log(playerControllerDataDictionary.Count);
       
       
        Debug.Log("onGameStart");        
       
        gameObject.SetActive(false);
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
        Debug.Log("vibrating for " + time + " seconds");
        GamePad.SetVibration((PlayerIndex)controllerID, strength, strength);
        yield return new WaitForSecondsRealtime(time);
        GamePad.SetVibration((PlayerIndex)controllerID, 0, 0);
    }

    //create a manager that stores all our player info we just got, save as don't destroy on load so we can use it to spawn players in
    //the real round
    //public void SpawnPlayerManager(PlayerControllerData playerControllerData)
    //{
    //    PlayerDataScript PD = GameObject.Instantiate(playerDataPrefab).GetComponent<PlayerDataScript>();
    //    PD.gameObject.DontDestroyOnLoad();
       
    //    PD.SetPlayerInfoAfterRoundStart(playerControllerData, globalPlayerSettings, playerCanvasPrefab);
    //}

    
}