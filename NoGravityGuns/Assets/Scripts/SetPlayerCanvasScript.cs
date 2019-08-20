using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//quick script to keep the worldspace canvas attached to player without inheriting rotation like it would if it were a child
public class SetPlayerCanvasScript : MonoBehaviour
{
    GameObject parent;

    public PlayerScript playerscript;

    private void Awake()
    {
        parent = transform.parent.gameObject;
        playerscript = parent.GetComponent<PlayerScript>();
        transform.parent = null;
    }
    void Update()
    {
        if(parent!=null)
            transform.position = parent.transform.position;
    }
}
