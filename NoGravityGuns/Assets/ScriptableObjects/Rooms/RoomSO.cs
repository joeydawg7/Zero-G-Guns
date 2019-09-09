using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Room", menuName = "ScriptableObjects/Room", order = 2)]
public class RoomSO : ScriptableObject
{
    [Tooltip("The title that shows when the scene loads")]
    public string roomName;
    [TextArea]
    public string roomDescription;

    [Header("What actually calls the scene... CHECK SPELLING!")]
    public string sceneName;

    public bool isPlayable = true;

}
