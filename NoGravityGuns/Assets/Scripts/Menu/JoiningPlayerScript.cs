using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem.Users;
using UnityEngine.InputSystem.PlayerInput;
using Rewired;

public class JoiningPlayerScript : MonoBehaviour
{
    public Color32 p1Color;
    public Color32 p2Color;
    public Color32 p3Color;
    public Color32 p4Color;

    public Color32 emptySlotColor;

    public TextMeshProUGUI tipToStart;

    public Image[] joinPanels;
    public List<int> assignedControls;

    public int maxPlayers = 4;
    private int rewiredPlayerIdCounter = 0;

    private void Awake()
    {
        tipToStart.alpha = 0;
        assignedControls = new List<int>();

        // Subscribe to controller connected events
        ReInput.ControllerConnectedEvent += OnControllerConnected;

    }

    void Start()
    {
        AssignAllJoysticksToSystemPlayer(true);
    }

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
        foreach (var j in ReInput.controllers.Joysticks)
        {
            ReInput.players.GetSystemPlayer().controllers.AddController(j, removeFromOtherPlayers);
        }

        //ReInput.players.GetSystemPlayer().controllers.maps.set
        //// Enable UI map so Player can start controlling the UI
        //rewiredPlayer.controllers.maps.SetMapsEnabled(true, "UI");
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
                AssignJoystickToPlayer(rewiredPlayer, source.controller as Joystick);
                break;

            }
            else
            { // Custom Controller
                throw new System.NotImplementedException();
            }
        }

        // Enable UI map so Player can start controlling the UI
       // rewiredPlayer.controllers.maps.SetMapsEnabled(true, "UI");
    }

    private void AssignJoystickToPlayer(Player player, Joystick joystick)
    {
        // Assign the joystick to the Player, removing it from System Player
        //player.controllers.AddController(joystick, true);

        // Mark this joystick as assigned so we don't give it to the System Player again
        if(!assignedControls.Contains(joystick.id))
            assignedControls.Add(joystick.id);

        AddPlayerControllerSetup(joystick.id, joystick);

        Debug.Log("Assigned " + joystick.name + " to Player " + joystick.id);
    }


    void StartButtonPressed()
    {

        Debug.Log("start Pressed");
        if (assignedControls.Count >= 1)
        {
            //foreach (var player in ReInput.players.AllPlayers)
            //{
            //    //player.controllers.AddController()
            //}

            ReInput.players.SystemPlayer.controllers.ClearAllControllers();

            //ReInput.players.SystemPlayer.controllers.ClearAllControllers();

            //foreach(var bleh in ReInput.players.SystemPlayer.controllers.Joysticks)
            //{
            //    Debug.Log(bleh.name);
            //}
            

            Debug.Log("start");
            GameManager.Instance.StartGame();
            tipToStart.alpha = 0;
        }
    }


    void AddPlayerControllerSetup(int i, Controller controller)
    {

        if (joinPanels[i].GetComponent<JoinPanel>().hasAssignedController == false)
        {
            joinPanels[i].GetComponent<JoinPanel>().AssignController((i + 1), controller);
            return;
        }


    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.Instance.isGameStarted)
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

        }
    }

    public void OnGameStart()
    {
        gameObject.SetActive(false);
    }

}
