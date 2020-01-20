using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Room", menuName = "ScriptableObjects/BTT_Room", order = 3)]
public class BTT_RoomSO : RoomSO
{
    public float bestTime = 9999999999f;
    public bool playOnLoad;
}
