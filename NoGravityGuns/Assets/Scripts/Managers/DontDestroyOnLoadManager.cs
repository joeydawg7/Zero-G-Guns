using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DontDestroyOnLoadManager
{
    static List<GameObject> _ddolObjects = new List<GameObject>(100);

    public static void DontDestroyOnLoad(this GameObject go)
    {
        UnityEngine.Object.DontDestroyOnLoad(go);
        _ddolObjects.Add(go);
    }

    public static void DestroyAll()
    {
        foreach (var go in _ddolObjects)
            if (go != null)
            {
                UnityEngine.Object.Destroy(go);
            }

        _ddolObjects.Clear();
    }
}