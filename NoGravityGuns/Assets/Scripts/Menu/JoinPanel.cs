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

    Image image;

    public Sprite readySprite;
    public Sprite notReadySprite;

    private void Awake()
    {
        image = GetComponent<Image>();
        hasAssignedController = false;
        MainText.text = "Press A to Join";
        
    }


    public PlayerScript AssignController(PlayerControls controller, int i)
    {
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
