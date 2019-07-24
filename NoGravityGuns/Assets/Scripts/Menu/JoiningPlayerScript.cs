using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.InputSystem.PlayerInput;

public class JoiningPlayerScript : MonoBehaviour
{
    public Color32 p1Color;
    public Color32 p2Color;
    public Color32 p3Color;
    public Color32 p4Color;

    public Color32 emptySlotColor;

    public TextMeshProUGUI tipToStart;

    public Image[] joinPanels;

    List<string> assignedControllers;

    public List<InputActionMap> assignedControls;

    //public PlayerControls globalControls;

    PlayerControls globalControls;

    private void Awake()
    {
        tipToStart.alpha = 0;
        assignedControllers = new List<string>();
        assignedControls = new List<InputActionMap>();


        globalControls = new PlayerControls();

        globalControls.Enable();

        //foreach (var item in globalControls.devices)
        //{
        //    Debug.Log(item.device.displayName);
        //}
        //for (int i = 0; i < joinPanels.Length; i++)
        //{
        //    joinPanels[i].color = emptySlotColor;
        //}
        //globalControls.Enable();
        //Debug.Log(;
    }

    private void Start()
    {
        globalControls.Gameplay.Join.performed += JoinButtonPressed;
        globalControls.Gameplay.Drop.performed += DropButtonPressed;
        globalControls.Gameplay.Start.performed += StartButtonPressed;
    }

    void JoinButtonPressed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (!GameManager.Instance.isGameStarted && assignedControls.Count <= 4)
        {

            AddPlayerControllerSetup(globalControls.Gameplay.Get().Clone());
        }

    }

    void DropButtonPressed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {

        for (int i = joinPanels.Length - 1; i >= 0; i--)
        {
            if (joinPanels[i].GetComponent<JoinPanel>().hasAssignedController == true)
            {
                assignedControls.RemoveAt(i);
                joinPanels[i].GetComponent<JoinPanel>().UnAssignController();
                return;
            }
        }

    }

    void StartButtonPressed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (assignedControls.Count >= 1)
        {
            Debug.Log("start");
            GameManager.Instance.StartGame();
            tipToStart.alpha = 0;
        }
    }


    void AddPlayerControllerSetup(InputActionMap playerControls)
    {
        for (int i = 0; i < joinPanels.Length; i++)
        {
            if (joinPanels[i].GetComponent<JoinPanel>().hasAssignedController == false)
            {
                //assignedControllers.Add("1");
                joinPanels[i].GetComponent<JoinPanel>().AssignController(playerControls, (i + 1));
                assignedControls.Add(playerControls);
                return;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //var gamePad = Gamepad.all;

        //foreach (var pad in gamePad)
        //{
        //    if (pad.aButton.wasPressedThisFrame)
        //    {
        //        Debug.Log(pad.id);
        //        //pad.allControls = globalControls.;

        //        //globalControls.devices. = gamePad.ToArray();

        //        //foreach (var controller in globalControls.devices)
        //        //{
        //        //    Debug.Log(controller.id);
        //        //    controller.
        //        //}

        //    }

        //}




        //if (!GameManager.Instance.isGameStarted)
        //{
        //    string cont;

        //    for (int i = 1; i <= joinPanels.Length; i++)
        //    {
        //        cont = "cont" + i;

        //       if (assignedControllers.Contains(cont))
        //           continue;

        //        if (Input.GetButtonDown("J" + i + "A"))
        //        {
        //            AddPlayerController(i, cont);
        //            break;
        //        }
        //    }

        //    for (int i = 1; i <= joinPanels.Length; i++)
        //    {
        //        cont = "cont" + i;

        //        if (Input.GetButtonDown("J" + i + "B"))
        //        {
        //            RemovePlayerController(i, cont);
        //            break;
        //        }
        //    }

        //    if (assignedControllers.Count >= 1)
        //        tipToStart.alpha = 1;
        //    else
        //        tipToStart.alpha = 0;

        //    if (Input.GetButton("Submit") && assignedControllers.Count >= 1)
        //    {
        //        GameManager.Instance.StartGame();
        //        tipToStart.alpha = 0;
        //    }
        //}

    }


    //public PlayerScript AddPlayerController(int controller, string contString)
    //{


    //    for (int i = 0; i < joinPanels.Length; i++)
    //    {
    //        if (joinPanels[i].GetComponent<JoinPanel>().hasAssignedController == false)
    //        {
    //            assignedControllers.Add(contString);
    //            return joinPanels[i].GetComponent<JoinPanel>().AssignController(controller, i);
    //        }
    //    }

    //    return null;
    //}

    //PlayerScript RemovePlayerController(int controller, string contString)
    //{
    //    Debug.Log(contString);
    //    assignedControllers.Remove(contString);
    //    Debug.Log(assignedControllers.Count);
    //    for (int i = joinPanels.Length - 1; i >= 0; i--)
    //    {
    //        if (joinPanels[i].GetComponent<JoinPanel>().hasAssignedController == true)
    //        {
    //            return joinPanels[i].GetComponent<JoinPanel>().UnAssignController(controller);
    //        }
    //    }

    //    return null;
    //}

    public void OnGameStart()
    {
        //globalControls.Gameplay.Join.performed -=JoinButtonPressed;
        //globalControls.Gameplay.Drop.performed -=  DropButtonPressed;
        //globalControls.Gameplay.Start.performed -=  StartButtonPressed;
        //globalControls.Gameplay.Disable();
        //Debug.Log("global controls enabled: " + globalControls.Gameplay.enabled);
        gameObject.SetActive(false);
    }

}
