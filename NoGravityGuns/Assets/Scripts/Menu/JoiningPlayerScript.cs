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

    List<int> assignedControllers;

    private void Awake()
    {
        tipToStart.alpha = 0;
        assignedControllers = new List<int>();
    }

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < joinPanels.Length; i++)
        {
            joinPanels[i].color = emptySlotColor;
        }
    }

    // Update is called once per frame
    void Update()
    {

   

        if (!GameManager.Instance.isGameStarted)
        {
            for (int i = 1; i <= joinPanels.Length; i++)
            {
                if (assignedControllers.Contains(i))
                    continue;

                if (Input.GetButtonDown("J" + i + "A"))
                {
                    //Debug.Log(i);
                    AddPlayerController(i);
                    break;
                }
            }

            if (assignedControllers.Count > 0)
                tipToStart.alpha = 1;
            else
                tipToStart.alpha = 0;

            if (Input.GetButton("Submit") && assignedControllers.Count > 0)
            {
                GameManager.Instance.StartGame();
                tipToStart.alpha = 0;
            }
        }

    }


    public PlayerScript AddPlayerController(int controller)
    {
        assignedControllers.Add(controller);

        for (int i = 0; i < joinPanels.Length; i++)
        {
            if (joinPanels[i].GetComponent<JoinPanel>().hasAssignedController == false)
            {
                return joinPanels[i].GetComponent<JoinPanel>().AssignController(controller);
            }
        }

        return null;
    }

    public void OnGameStart()
    {
        Destroy(gameObject);
    }

}
