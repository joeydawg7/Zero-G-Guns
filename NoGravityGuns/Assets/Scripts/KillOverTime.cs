using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillOverTime : MonoBehaviour
{
    public float killTime;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, killTime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
