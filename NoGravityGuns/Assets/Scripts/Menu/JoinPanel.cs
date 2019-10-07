﻿using System.Collections;
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

    public Color selectedColour;
    //public Sprite readySprite;
    //public Sprite notReadySprite;

    private void Awake()
    {
        image = GetComponent<Image>();
        hasAssignedController = false;
        MainText.text = "Press A";

    }

    public PlayerScript AssignController(int i, Controller controller)
    {
        MainText.text = "Ready";
        hasAssignedController = true;
        image.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        image.color = selectedColour;
        //player.SetController(i, controller);
        //playerSpawnPoint.SetCharacter(i, controller);

        return null;
    }

    public PlayerScript UnAssignController()
    {
        //image.sprite = notReadySprite;
        MainText.text = "Press A";
        hasAssignedController = false;
        image.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
        //player.UnsetController();

        return null;
    }
}
