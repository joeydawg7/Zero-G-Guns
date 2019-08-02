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
    // Update is called once per frame
    void Update()
    {
        if(parent!=null)
            transform.position = parent.transform.position;
    }
}
