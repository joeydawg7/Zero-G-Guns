﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillScript : MonoBehaviour
{


    public void Update()
    {

    }

    public void TurnMeOff(string animType)
    {
        Debug.Log(animType);
        PlayerScript playerScript = transform.root.GetComponent<PlayerCanvasScript>().playerScript;
        if (playerScript != null)
        {
            Debug.Log("ending anim");
            //null this crap out... very important
            playerScript.floatingDamage = new PlayerScript.FloatingDamageStuff();
            GetComponent<Animator>().SetTrigger(animType);
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
            gameObject.SetActive(false);
            
        }
        
    }
}
