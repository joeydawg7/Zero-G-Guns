using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetPlayerCanvasScript : MonoBehaviour
{
    GameObject parent;

    // Start is called before the first frame update
    void Start()
    {
        parent = transform.parent.gameObject;
        transform.parent = null;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = parent.transform.position;
    }
}
