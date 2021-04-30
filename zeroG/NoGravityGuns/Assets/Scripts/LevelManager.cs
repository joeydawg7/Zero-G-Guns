using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;


public static class LevelManager
{

    public static void LoadLevel(MonoBehaviour mono, string levelName)
    {
        //mono.StartCoroutine(LoadLevelCoroutine(mono, levelName));

        PlayerPrefs.SetString("LastScene", SceneManager.GetActiveScene().name);

        SceneManager.LoadScene(levelName, LoadSceneMode.Single);
    }

    public static void LoadLevel(MonoBehaviour mono, RoomSO level)
    {
        //mono.StartCoroutine(LoadLevelCoroutine(mono, levelName));

        PlayerPrefs.SetString("LastScene", SceneManager.GetActiveScene().name);

        SceneManager.LoadScene(level.sceneName, LoadSceneMode.Single);
    }

    public static void LoadLastLevel(MonoBehaviour mono)
    {
        string lvl = PlayerPrefs.GetString("LastScene", SceneManager.GetActiveScene().name);

        SceneManager.LoadScene(lvl, LoadSceneMode.Single);
    }

    public static void ResetLastLevel()
    {
        PlayerPrefs.SetString("LastScene", SceneManager.GetActiveScene().name);
    }

    static IEnumerator LoadLevelCoroutine(MonoBehaviour mono, string level)
    {
        AsyncOperation asyncLoadLevel;
        asyncLoadLevel = SceneManager.LoadSceneAsync(level, LoadSceneMode.Single);

        Debug.Log("Last scene set to " + SceneManager.GetActiveScene().name);
        PlayerPrefs.SetString("LastScene", SceneManager.GetActiveScene().name);

        Debug.Log("start loading scene");

        while (!asyncLoadLevel.isDone)
        {
            yield return null;
        }
    }

    public static string GetLastLevelName()
    {
        return PlayerPrefs.GetString("LastScene");
    }

    public static RoomSO[] GetRooms()
    {
        RoomSO[] rooms = Resources.LoadAll<RoomSO>("ScriptableObjects/Rooms/PlayableRooms");

        return rooms;
    }

    public static RoomSO[] GetPlayableRooms()
    {
        //only get rooms where playable bool is flagged true and return them
        List<RoomSO> rooms = new List<RoomSO>();
        rooms.AddRange(Resources.LoadAll<RoomSO>("ScriptableObjects/Rooms/PlayableRooms"));
        RoomSO[] roomsArray = rooms.FindAll(room => room.isPlayable).ToArray();

        return roomsArray;
    }

    public static RoomSO GetRandomArenaRoom()
    {
        RoomSO[] rooms = GetPlayableRooms();

        return rooms[Random.Range(0, rooms.Length)];
    }

    public static void LoadArenaPersistentScene()
    {
        PlayerPrefs.SetString("LastScene", SceneManager.GetActiveScene().name);

        SceneManager.LoadScene("Arena_PersistentScene", LoadSceneMode.Single);
    }

   


}
