using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.PlayerInput;
using UnityEngine.InputSystem.Users;

public class JoinPanel : MonoBehaviour
{

    public bool hasAssignedController;

    public PlayerScript player;
    public TextMeshProUGUI MainText;

    Image image;

    public Sprite readySprite;
    public Sprite notReadySprite;

    private void Awake()
    {
        image = GetComponent<Image>();
        hasAssignedController = false;
        MainText.text = "Press A to Join";
        
    }


    public PlayerScript AssignController(InputActionMap controller, int i)
    {
        //controller =

        //foreach (var device in InputUser.GetUnpairedInputDevices())
        //{
        //    if (!device.name.Contains("Controller"))
        //        continue;
        //    else
        //        player2Input.ApplyBindingOverridesOnMatchingControls(device);
        //}

        image.sprite = readySprite;
        MainText.text = "Player " + controller + " joined!";
        hasAssignedController = true;

        player.SetController(controller, i);

        return player;
    }

    public PlayerScript UnAssignController()
    {
        image.sprite = notReadySprite;
        MainText.text = "Press A to join";
        hasAssignedController = false;

        player.UnsetController();

        return player;
    }
}
