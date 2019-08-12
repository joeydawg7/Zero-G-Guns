using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetPlayerCanvasScript : MonoBehaviour
{
    GameObject parent;

    private void Awake()
    {
        parent = transform.parent.gameObject;
        transform.parent = null;
    }
    void Update()
    {
        if(parent!=null)
            transform.position = parent.transform.position;
    }
}
