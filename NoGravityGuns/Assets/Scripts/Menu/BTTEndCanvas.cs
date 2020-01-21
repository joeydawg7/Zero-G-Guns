using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Rewired;
using UnityEngine.SceneManagement;

public class BTTEndCanvas : MonoBehaviour
{
    public TextMeshProUGUI time;
    public TextMeshProUGUI newRecord;

    bool gameIsDone;

    // Start is called before the first frame update
    void Start()
    {
        time.gameObject.SetActive(false);
        newRecord.gameObject.SetActive(false);

        SetAllTextAlphas(0);

        gameIsDone = false;
    }

    private void SetAllTextAlphas(float alpha)
    {
        foreach (Transform child in transform)
        {
            TextMeshProUGUI t = child.GetComponent<TextMeshProUGUI>();

            if (t)
            {
                t.alpha = alpha;
            }
        }
    }

    public void ShowEndScreen(float t)
    {
        SetAllTextAlphas(1);
        string tString = Extensions.FloatToTime(t, "#0:00.000");

        float prevBestTime = PlayerPrefs.GetFloat(BTT_Manager.Instance.currentRoom.roomName);


        bool newRecord = false;

        //TODO: do new record stuff here!
        if (prevBestTime <= 0)
        {
            PlayerPrefs.SetFloat(BTT_Manager.Instance.currentRoom.roomName, t);
            newRecord = true;
        }
        else if (t < prevBestTime)
        {
            PlayerPrefs.SetFloat(BTT_Manager.Instance.currentRoom.roomName, t);
            newRecord = true;
        }



        StartCoroutine(AnimateEndScreen(tString, newRecord));
    }

    IEnumerator AnimateEndScreen(string t, bool gotNewRecord)
    {
        yield return new WaitForSeconds(0.25f);
        gameIsDone = true;

        time.gameObject.SetActive(true);
        time.text = "Time: " + t;

        yield return new WaitForSeconds(0.25f);

        if (gotNewRecord)
        {
            Cursor.visible = true;
            newRecord.gameObject.SetActive(true);
        }
    }

    private void Update()
    {
        if (gameIsDone)
        {
            foreach (var player in ReInput.players.AllPlayers)
            {
                //A retry
                if (player.GetButtonDown("Join"))
                {
                    //BTT_Manager.Instance.NewBTT_Level(0);
                    //BTT_Manager.inst.
                    BTT_Manager.Instance.BackToPersistentScene();
                    gameIsDone = false;
                    break;
                }

                //B back to menu
                if (player.GetButtonDown("Drop"))
                {
                    BTT_Manager.Instance.BackToMenu();
                    gameIsDone = false;
                    break;
                }
            }
        }
    }
}
