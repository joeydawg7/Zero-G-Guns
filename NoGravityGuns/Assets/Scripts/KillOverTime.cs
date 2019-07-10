using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillOverTime : MonoBehaviour
{
    public float killTime;

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void StartKilling()
    {
        Destroy(gameObject, killTime);
    }
}
