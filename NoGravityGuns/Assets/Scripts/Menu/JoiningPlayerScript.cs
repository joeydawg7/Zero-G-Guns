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

    JoinPanel[] joinPanels;
    List<int> assignedControls;

    public int maxPlayers = 4;
    private int rewiredPlayerIdCounter = 0;

    public AudioClip joinClick;

    PlayerSpawnPoint[] playerSpawnPoints;

    private void Awake()
    {
        tipToStart.alpha = 0;
        assignedControls = new List<int>();
        playerSpawnPoints = FindObjectsOfType<PlayerSpawnPoint>();
        joinPanels = FindObjectsOfType<JoinPanel>();

        // Subscribe to controller connected events
        ReInput.ControllerConnectedEvent += OnControllerConnected;

    }

    void Start()
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
    }

    private int GetNextGamePlayerId()
    {
        return rewiredPlayerIdCounter++;
    }

    void AssignNextPlayer()
    {
        if (assignedControls.Count >= 4 || rewiredPlayerIdCounter >= 4)
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
                AssignJoystickToPlayer(rewiredPlayer, source.controller as Joystick);
                break;

            }
            else
            { // Custom Controller
                throw new System.NotImplementedException();
            }
        }

    }

    void RemoveNextPlayer()
    {
        Debug.Log(rewiredPlayerIdCounter);
        if (assignedControls.Count < 0 || rewiredPlayerIdCounter < 0)
        {
            Debug.Log("Min player limit already reached!");
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

    private void AssignJoystickToPlayer(Player player, Joystick joystick)
    {
        // Mark this joystick as assigned so we don't give it to the System Player again
        if (!assignedControls.Contains(joystick.id))
        {
            assignedControls.Add(joystick.id);
            //only play the sound if not contained, so we can tell if someone is joining when they are already in
            GameManager.Instance.audioSource.PlayOneShot(joinClick);
        }

        

        foreach (var spawnPoint in playerSpawnPoints)
        {
            if((spawnPoint.IDToSpawn-1) == joystick.id)
            {
                AddPlayerControllerSetup(joystick.id, joystick, spawnPoint);
                break;
            }
        }

        Debug.Log("Assigned " + joystick.name + " " + joystick.id + " to Player " + player.id);

        tipToStart.alpha = 1;
    }

    private void RemoveJoystickFromPlayer(Joystick joystick)
    {
        // Mark this joystick as assigned so we don't give it to the System Player again
        if (assignedControls.Contains(joystick.id))
        {
            assignedControls.Remove(joystick.id);
            //only play the sound if not contained, so we can tell if someone is joining when they are already in
            GameManager.Instance.audioSource.PlayOneShot(joinClick);
        }

        RemovePlayerControllerSetup(joystick.id, joystick);

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

        if (assignedControls.Count >= 1)
        {
            ReInput.players.SystemPlayer.controllers.ClearAllControllers();
            GameManager.Instance.StartGame();
            RoundManager.Instance.finishedControllerSetup = true;
            RoundManager.Instance.NewRound(false);
            tipToStart.alpha = 0;
        }
    }


    void AddPlayerControllerSetup(int i, Controller controller, PlayerSpawnPoint playerSpawnPoint)
    {
        JoinPanel jp = joinPanels[i].GetComponent<JoinPanel>();
        if (jp.hasAssignedController == false)
        {
            jp.AssignController((i + 1), controller, playerSpawnPoint);
            return;
        }
    }

    // Update is called once per frame
    void Update()
    {
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
    }


    public void OnGameStart()
    {
        gameObject.SetActive(false);

        Debug.Log("player spawn points: " + playerSpawnPoints.Length);

        foreach (var playerSpawnPoint in playerSpawnPoints)
        {
            playerSpawnPoint.SpawnCharacter();
        }
    }

}
