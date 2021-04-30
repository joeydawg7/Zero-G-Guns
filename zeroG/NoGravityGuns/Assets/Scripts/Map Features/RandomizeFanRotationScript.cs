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
            motor.motorSpeed = 600;
        }
        else
        {
            motor.motorSpeed = -600;
        }

        fan.motor = motor;

    }

}
