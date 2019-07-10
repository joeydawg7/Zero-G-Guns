using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    private void Awake()
    {
        tipToStart.alpha = 0;
        assignedControllers = new List<string>();
        //for (int i = 0; i < joinPanels.Length; i++)
        //{
        //    joinPanels[i].color = emptySlotColor;
        //}
    }


    // Update is called once per frame
    void Update()
    {

        if (!GameManager.Instance.isGameStarted)
        {
            string cont;

            for (int i = 1; i <= joinPanels.Length; i++)
            {
                cont = "cont" + i;

               // if (assignedControllers.Contains(cont))
               //     continue;

                if (Input.GetButtonDown("J" + i + "A"))
                {
                    AddPlayerController(i, cont);
                    break;
                }
            }

            for (int i = 1; i <= joinPanels.Length; i++)
            {
                cont = "cont" + i;

                if (Input.GetButtonDown("J" + i + "B"))
                {
                    RemovePlayerController(i, cont);
                    break;
                }
            }

            if (assignedControllers.Count >= 1)
                tipToStart.alpha = 1;
            else
                tipToStart.alpha = 0;

            if (Input.GetButton("Submit") && assignedControllers.Count >= 1)
            {
                GameManager.Instance.StartGame();
                tipToStart.alpha = 0;
            }
        }

    }


    public PlayerScript AddPlayerController(int controller, string contString)
    {


        for (int i = 0; i < joinPanels.Length; i++)
        {
            if (joinPanels[i].GetComponent<JoinPanel>().hasAssignedController == false)
            {
                assignedControllers.Add(contString);
                return joinPanels[i].GetComponent<JoinPanel>().AssignController(controller);
            }
        }

        return null;
    }

    PlayerScript RemovePlayerController(int controller, string contString)
    {
        Debug.Log(contString);
        assignedControllers.Remove(contString);
        Debug.Log(assignedControllers.Count);
        for (int i = joinPanels.Length - 1; i >= 0; i--)
        {
            if (joinPanels[i].GetComponent<JoinPanel>().hasAssignedController == true)
            {
                return joinPanels[i].GetComponent<JoinPanel>().UnAssignController(controller);
            }
        }

        return null;
    }

    public void OnGameStart()
    {
        gameObject.SetActive(false);
    }

}
