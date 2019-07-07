using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class JoinPanel : MonoBehaviour
{

    public bool hasAssignedController;

    public PlayerScript player;
    public TextMeshProUGUI MainText;
    public Color32 panelSelectedColor;

    private void Awake()
    {
        hasAssignedController = false;
        MainText.text = "Press A to Join";
    }

    public PlayerScript AssignController(int controller)
    {
        GetComponent<Image>().color = panelSelectedColor;
        MainText.text = "Player " + controller + " joined!";
        hasAssignedController = true;

        player.SetControllerNumber(controller);

        return player;
    }
}
