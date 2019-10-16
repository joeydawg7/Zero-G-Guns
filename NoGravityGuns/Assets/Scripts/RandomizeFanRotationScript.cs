using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomizeFanRotationScript : MonoBehaviour
{
    WheelJoint2D fan;

    // Start is called before the first frame update
    void Awake()
    {
        fan = GetComponent<WheelJoint2D>();
        var motor = fan.motor;


        int rand = Random.Range(0,2);
        if (rand ==1 )
        {
            Debug.Log("FAN:1");
            motor.motorSpeed = 600;
        }
        else
        {
            Debug.Log("FAN:2");
            motor.motorSpeed = -600;
        }

        fan.motor = motor;

    }

}
