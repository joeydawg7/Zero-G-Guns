using Rewired;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AddSinglePlayer : MonoBehaviour
{
    //public Color32 emptySlotColor;
    //public TextMeshProUGUI tipToStart;
   
    int assignedControl;

    //public int maxPlayers = 1;
    //private int rewiredPlayerIdCounter = 0;
    //public AudioClip joinClick;
    //public AudioClip unjoinClick;
    PlayerSpawnPoint[] playerSpawnPoints;

    Dictionary<int, PlayerControllerData> playerControllerDataDictionary;

    public GameObject playerCanvas;
    private float holdTimer;
    public Image backIndecator;

    private void Awake()
    {
        //tipToStart.alpha = 0;        
        playerSpawnPoints = FindObjectsOfType<PlayerSpawnPoint>();
        //joinPanels = FindObjectsOfType<JoinPanel>();

        // Subscribe to controller connected events
        ReInput.ControllerConnectedEvent += OnControllerConnected;
        //ReInput.ControllerPreDisconnectEvent += OnControllerDisConnected;

        playerControllerDataDictionary = new Dictionary<int, PlayerControllerData>();
        holdTimer = 0;

    }

    void Start()
    {
        try
        {

            if (!BTT_Manager.Instance.finishedControllerSetup)
            {          
                gameObject.SetActive(true);               
            }           
        }
        catch
        {
            SceneManager.LoadSceneAsync("PersistentScene", LoadSceneMode.Single);
            LoadingBar.Instance.StartLoadingBar();
        }

    }
    
    void OnControllerConnected(ControllerStatusChangedEventArgs args)
    {
        if (args.controllerType != ControllerType.Joystick)
            return;

        // Check if this Joystick has already been assigned. If so, just let Auto-Assign do its job.
        if (assignedControl ==  args.controllerId)
            return;

        // Joystick hasn't ever been assigned before. Make sure it's assigned to the System Player until it's been explicitly assigned
        ReInput.players.GetSystemPlayer().controllers.AddController(
            args.controllerType,
            args.controllerId,
            true // remove any auto-assignments that might have happened
        );
    }

    //void AssignAllJoysticksToSystemPlayer(bool removeFromOtherPlayers)
    //{
    //    foreach (var j in ReInput.controllers.Joysticks)
    //    {
    //        ReInput.players.GetSystemPlayer().controllers.AddController(j, removeFromOtherPlayers);
    //    }
    //}

    //private int GetNextGamePlayerId()
    //{
    //    return rewiredPlayerIdCounter++;
    //}

    //void AssignNextPlayer()
    //{
    //    //if (assignedControls.Count >= 4)
    //    //{
    //    //    Debug.Log("Max player limit already reached!");
    //    //    return;
    //    //}
    //    // Get the next Rewired Player Id
    //    int rewiredPlayerId = GetNextGamePlayerId();

    //    // Get the Rewired Player
    //    Player rewiredPlayer = ReInput.players.GetPlayer(rewiredPlayerId);

    //    // Determine which Controller was used to generate the JoinGame Action
    //    Player systemPlayer = ReInput.players.GetSystemPlayer();
    //    var inputSources = systemPlayer.GetCurrentInputSources("Join");

    //    foreach (var source in inputSources)
    //    {
    //        if (source.controllerType == ControllerType.Joystick)
    //        { // assigning a joystick

    //            // Assign the joystick to the Player. This will also un-assign it from System Player
    //            AssignJoystickToPlayer(rewiredPlayer, source.controller as Joystick);
    //            break;

    //        }
    //        else
    //        { // Custom Controller
    //            throw new System.NotImplementedException();
    //        }
    //    }
    //}

    //private void AssignJoystickToPlayer(Player player, Joystick joystick)
    //{
    //    // Mark this joystick as assigned so we don't give it to the System Player again
    //    if (!assignedControls.Contains(joystick.id))
    //    {
    //        assignedControls.Add(joystick.id);
    //        //only play the sound if not contained, so we can tell if someone is joining when they are already in
    //        GameManager.Instance.audioSource.PlayOneShot(joinClick);

    //        JoinPanel jp = joinPanels[joystick.id].GetComponent<JoinPanel>();

    //        if (jp.hasAssignedController == false)
    //        {
    //            PlayerControllerData playerControllerData = new PlayerControllerData(joystick.id, joystick);

    //            playerControllerDataDictionary.Add(playerControllerData.ID, playerControllerData);

    //            jp.AssignController(playerControllerData.ID, playerControllerData.controller);

    //            //Debug.Log("Assigned " + joystick.name + " " + joystick.id + " to Player " + player.descriptiveName);
    //        }

    //        if (assignedControls.Count > 1)
    //        {
    //            tipToStart.alpha = 1;
    //        }
    //    }
    //}

    //void RemoveNextPlayer()
    //{
    //    Debug.Log(rewiredPlayerIdCounter);
    //    if (assignedControls.Count <= 0 || rewiredPlayerIdCounter < 0)
    //    {
    //        Debug.Log("Min player limit already reached!");
    //        return;
    //    }

    //    rewiredPlayerIdCounter--;
    //    // Get the next Rewired Player Id
    //    //int rewiredPlayerId = GetNextGamePlayerId();

    //    //// Get the Rewired Player
    //    //Player rewiredPlayer = ReInput.players.GetPlayer(rewiredPlayerId);

    //    // Determine which Controller was used to generate the Drop Action
    //    Player systemPlayer = ReInput.players.GetSystemPlayer();
    //    var inputSources = systemPlayer.GetCurrentInputSources("Drop");

    //    foreach (var source in inputSources)
    //    {
    //        //TODO: mouse and keyboard support?
    //        if (source.controllerType == ControllerType.Keyboard || source.controllerType == ControllerType.Mouse)
    //        { // Assigning keyboard/mouse

    //            // Assign KB/Mouse to the Player
    //            //AssignKeyboardAndMouseToPlayer(rewiredPlayer);

    //            //// Disable KB/Mouse Assignment category in System Player so it doesn't assign through the keyboard/mouse anymore
    //            //ReInput.players.GetSystemPlayer().controllers.maps.SetMapsEnabled(false, ControllerType.Keyboard, "Assignment");
    //            //ReInput.players.GetSystemPlayer().controllers.maps.SetMapsEnabled(false, ControllerType.Mouse, "Assignment");
    //            //break;

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

    //private void RemoveJoystickFromPlayer(Joystick joystick)
    //{
    //    // Mark this joystick as assigned so we don't give it to the System Player again
    //    if (assignedControls.Contains(joystick.id))
    //    {
    //        assignedControls.Remove(joystick.id);
    //        //only play the sound if not contained, so we can tell if someone is joining when they are already in
    //        GameManager.Instance.audioSource.PlayOneShot(unjoinClick);
    //    }

    //    RemovePlayerControllerSetup(joystick.id, joystick);

    //    playerControllerDataDictionary.Remove(joystick.id);

    //    Debug.Log("Removed " + joystick.name + " from Player " + joystick.id);

    //    if (assignedControls.Count < 1)
    //        tipToStart.alpha = 0;
    //}

    //void RemovePlayerControllerSetup(int i, Controller controller)
    //{
    //    JoinPanel jp = joinPanels[i].GetComponent<JoinPanel>();
    //    if (jp.hasAssignedController == true)
    //    {
    //        jp.UnAssignController();
    //        return;
    //    }
    //}

    //void StartButtonPressed()
    //{
    //    //start game for real on a new round with start over = false
    //    //if(GameModeFlag.Instance)
    //    //{
    //    //    if (GameModeFlag.Instance.MultiPlayer)
    //    //    {
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
    //    //}
    //    //else
    //    //{
    //    //    if (assignedControls.Count >= 1)
    //    //    {
    //    //        ReInput.players.SystemPlayer.controllers.ClearAllControllers();
    //    //        GameManager.Instance.StartGame();
    //    //        RoundManager.Instance.finishedControllerSetup = true;
    //    //        RoundManager.Instance.NewRound(false);
    //    //    }
    //    //}
    //    //}
    //    //else
    //    //{
    //    //    if (assignedControls.Count >= 2)
    //    //    {
    //    //        ReInput.players.SystemPlayer.controllers.ClearAllControllers();
    //    //        GameManager.Instance.StartGame();
    //    //        RoundManager.Instance.finishedControllerSetup = true;
    //    //        RoundManager.Instance.NewRound(false);
    //    //    }
    //    //    else if (GameManager.Instance.debugManager.useDebugSettings)
    //    //    {
    //    //        //allow single player for testing
    //    //        if (assignedControls.Count >= 1)
    //    //        {
    //    //            ReInput.players.SystemPlayer.controllers.ClearAllControllers();
    //    //            GameManager.Instance.StartGame();
    //    //            RoundManager.Instance.finishedControllerSetup = true;
    //    //            RoundManager.Instance.NewRound(false);
    //    //        }
    //    //    }
    //    //}      

    //    tipToStart.alpha = 0;
    //}

    // Update is called once per frame
    void Update()
    {
        //if either of these things are null we can't proceed. probably broken from loading outside of persistent scene and will fix itself shortly :D
        if (!GameManager.Instance || !BTT_Manager.Instance)
            return;
       
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
            BTT_Manager.Instance.SpawnPlayerManager(pCdataDic.Value);
        }

        gameObject.SetActive(false);
    }



}
//// Start is called before the first frame update
//void Start()
//    {
        
//    }

//    // Update is called once per frame
//    void Update()
//    {
        
//    }
//}
