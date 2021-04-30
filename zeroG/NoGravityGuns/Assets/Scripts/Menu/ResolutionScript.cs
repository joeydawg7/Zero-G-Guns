using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using Rewired;
public class ResolutionScript : MonoBehaviour
{
    public TextMeshProUGUI width;
    public TextMeshProUGUI height;

    LinkedList<Resolution> resolutions;
    LinkedListNode<Resolution> currentResNode;

    private void Awake()
    {
        resolutions = new LinkedList<Resolution>();
        InitializeResolution();
    }

    //inital setup of resolution data, grabs from previous saved if one exists
    public void InitializeResolution()
    {
        //grab a list of resolutions based on all possible resolutions for the given monitor
        resolutions = new LinkedList<Resolution>(Screen.resolutions);
        currentResNode = resolutions.Last;

        //set our default res, either from prefs or setting a default if none found
       // currentResNode.Value = GetResFromPrefs();

        //Debug.Log("initial resolution set:");
        //Debug.Log(currentResNode.Value.width + " x " + currentResNode.Value.height + " @ " + currentResNode.Value.refreshRate);

        //saves the above data to prefs in case it wasnt already there
        //SaveResToPrefs(currentResNode);

        //actually set res 
        //Screen.SetResolution(currentResNode.Value.width, currentResNode.Value.height,
        //    Screen.fullScreen, currentResNode.Value.refreshRate);

        //change the text
        width.text = Screen.currentResolution.width.ToString();
        height.text = Screen.currentResolution.height.ToString();


    }

    private void SaveResToPrefs(LinkedListNode<Resolution> resNode)
    {
        PlayerPrefs.SetInt("ResWidth", resNode.Value.width);
        PlayerPrefs.SetInt("ResHeight", resNode.Value.height);
        PlayerPrefs.SetInt("RefreshRate", resNode.Value.refreshRate);

        Debug.Log("saved: " + resNode.Value.width + " x " + resNode.Value.height + " @ " + resNode.Value.refreshRate);
    }

    Resolution GetResFromPrefs()
    {
 
        //if we are missing any of these saved keys we have no preset res and should just set a default and get out of here
        if (!PlayerPrefs.HasKey("ResWidth")
            || !PlayerPrefs.HasKey("ResHeight")
            || !PlayerPrefs.HasKey("RefreshRate")
            )
        {
           return resolutions.Last.Value;
        }

        //otherwise return a resolution from preferences

        int width = 0;
        int height = 0;
        int refreshRate = 0;

        PlayerPrefs.GetInt("ResWidth", width);
        PlayerPrefs.GetInt("ResHeight", height);
        PlayerPrefs.GetInt("RefreshRate", refreshRate);

        Resolution res = new Resolution();
        res.width = width;
        res.height = height;
        res.refreshRate = refreshRate;

        return res;

    }

    private void Update()
    {
        //if we have the resolution object selected
        if (EventSystem.current.currentSelectedGameObject == gameObject)
        {
            //press A to finalize res
            if (ReInput.players.GetSystemPlayer().GetButtonDown("UISubmit"))
            {
                //actually set the res based on our current position in the linked list
                Screen.SetResolution(currentResNode.Value.width, currentResNode.Value.height,
               true, currentResNode.Value.refreshRate);

                //save it so its remembered next time we open
                SaveResToPrefs(currentResNode);
            }
        }

    }

    public void SetRes(int incrementListNum)
    {
        //move up in list of possible resolutions (higher res)
        if (incrementListNum == 1)
        {
            //reached top of list, loop around to bottom
            if (currentResNode.Next == null)
            {
                currentResNode = currentResNode.List.First;
            }
            else
                currentResNode = currentResNode.Next;
        }
        //move down in possible res (lower res)
        else
        {  //reached bottom of list, loop around to top
            if (currentResNode.Previous == null)
            {
                currentResNode = currentResNode.List.Last;
            }
            else
                currentResNode = currentResNode.Previous;
        }

        //change the display
        width.text = currentResNode.Value.width.ToString();
        height.text = currentResNode.Value.height.ToString();

    }


}
