using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Gun", menuName = "ScriptableObjects/Managers/DebugManager", order = 2)]

public class DebugManager : ScriptableObject
{
    [Header("Set to false before making builds to turn off all debug settings in one blow")]
    public bool useDebugSettings;

}
