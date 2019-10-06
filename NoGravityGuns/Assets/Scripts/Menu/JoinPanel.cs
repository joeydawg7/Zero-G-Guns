using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.PlayerInput;
using UnityEngine.InputSystem.Users;
using Rewired;

public class JoinPanel : MonoBehaviour
{

    public bool hasAssignedController;

    //public PlayerScript player;
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

    public PlayerScript AssignController(int i, Controller controller)
    {
        MainText.text = "Player joined!";
        hasAssignedController = true;
        image.sprite = readySprite;
        //player.SetController(i, controller);
        //playerSpawnPoint.SetCharacter(i, controller);

        return null;
    }

    public PlayerScript UnAssignController()
    {
        image.sprite = notReadySprite;
        MainText.text = "Press A to join";
        hasAssignedController = false;
        image.sprite = notReadySprite;
        //player.UnsetController();

        return null;
    }
}
