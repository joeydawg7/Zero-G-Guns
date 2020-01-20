using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class LevelSelectData : MonoBehaviour
{
    public BTT_RoomSO room;

    public TextMeshProUGUI roomName;
    public TextMeshProUGUI time;

    public void SetRoomData(BTT_RoomSO room)
    {
        this.room = room;
        roomName.text = room.roomName;
        gameObject.name = room.roomName;
        time.text = "Time: " + Extensions.FloatToTime(room.bestTime, "#0:00.000");
    }

    public void OnClick()
    {
        room.playOnLoad = true;

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
