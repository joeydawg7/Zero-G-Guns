using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public class LevelSelectData : MonoBehaviour
{
    public BTT_RoomSO room;

    //public TextMeshProUGUI roomName;
    //public TextMeshProUGUI time;

    /// <summary>
    /// sets initial room data on the UI object when spawned
    /// </summary>
    /// <param name="room"></param>
    public void SetRoomData(BTT_RoomSO room, Button but)
    {
        this.room = room;
        string bestTimeText = string.Empty;
        float bestTime = PlayerPrefs.GetFloat(room.roomName);

        //show best time if exists
        if (bestTime > 0)
        {
            bestTimeText = "Best Time: " + Extensions.FloatToTime(bestTime, "#0:00.000");
        }           
        else
        {
            bestTimeText = "No Best Time!";
        }


        but.GetComponentInChildren<TextMeshProUGUI>().text = room.roomName + Environment.NewLine + bestTimeText;
        //roomName.text = room.roomName;
        //gameObject.name = room.roomName;        

        //set room preview if it exists
        //if(room.roomPreviewImage)
        //{
        //    gameObject.GetComponent<Image>().sprite = room.roomPreviewImage.sprite;
        //}
    }

    /// <summary>
    /// onclick load the BTT persistent scene after setting a bool on the chosen room SO so we know which level to load for real
    /// </summary>
    public void OnClick()
    {
        room.playOnLoad = true;

        ControllerLayoutManager.SwapToGameplayMaps();

        if (RoundManager.Instance == null)
        {
            Debug.Log("Click, Round Manger Null");
            SceneManager.LoadSceneAsync("BTT_PersistentScene", LoadSceneMode.Single);
            LoadingBar.Instance.StartLoadingBar();

        }
        else
        {
            PersistenceGiverScript.Instance.PersistenceTaker();
            SceneManager.LoadSceneAsync("BTT_PersistentScene", LoadSceneMode.Single);
            LoadingBar.Instance.StartLoadingBar();

        }
    }

}
