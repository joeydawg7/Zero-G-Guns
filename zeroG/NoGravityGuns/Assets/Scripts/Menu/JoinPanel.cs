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
    public TextMeshProUGUI MainText;
    Image image;
    public Color selectedColour;


    private void Awake()
    {
        image = GetComponent<Image>();
        hasAssignedController = false;
        MainText.text = "Press A";
    }

    public void AssignController()
    {
        MainText.text = "Ready";
        hasAssignedController = true;
        image.color = selectedColour;
    }

    public void UnAssignController()
    {
        MainText.text = "Press A";
        hasAssignedController = false;
        image.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
    }
}
