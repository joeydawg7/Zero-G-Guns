﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//Stores all required information about rooms, used to load seamlessly between them between rounds.

[CreateAssetMenu(fileName = "Room", menuName = "ScriptableObjects/Room", order = 2)]
public class RoomSO : ScriptableObject
{
    [Tooltip("The title that shows when the scene loads")]
    public string roomName;
    [TextArea]
    public string roomDescription;

    [Header("What actually calls the scene... CHECK SPELLING!")]
    public string sceneName;

    public Image roomPreviewImage;

    //checks off when player checks the map off in a list
    public bool isPlayable = true;

}
