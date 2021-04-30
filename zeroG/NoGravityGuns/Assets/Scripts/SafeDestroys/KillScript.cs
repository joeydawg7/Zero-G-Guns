using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillScript : MonoBehaviour
{


    public void Update()
    {

    }

    public void TurnMeOff(string animType)
    {
        PlayerScript playerScript = transform.root.GetComponent<PlayerCanvasScript>().playerScript;
        if (playerScript != null)
        {
            //null this crap out... very important
            playerScript.floatingDamage = new PlayerScript.FloatingDamageStuff();
            GetComponent<Animator>().SetTrigger(animType);
            transform.SetParent(null);
            gameObject.DontDestroyOnLoad();
            gameObject.SetActive(false);
            
        }
        
    }
}
