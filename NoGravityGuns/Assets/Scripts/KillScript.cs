using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillScript : MonoBehaviour
{

    public bool floatAway = false;
    public bool crit = false;

    public void Update()
    {
        if(floatAway)
        {
            TurnMeOff("FloatAway");
            floatAway = false;
        }

        if (crit)
        {
            TurnMeOff("Crit");
            crit = false;
        }
    }

    public void TurnMeOff(string animType)
    {
        Debug.Log(animType);
        PlayerScript playerScript = transform.root.GetComponent<PlayerCanvasScript>().playerScript;
        if (playerScript != null)
        {
            //null this crap out... very important
            playerScript.floatingDamage = new PlayerScript.FloatingDamageStuff();
            transform.parent = null;
            DontDestroyOnLoad(gameObject);
            gameObject.SetActive(false);
            GetComponent<Animator>().SetTrigger(animType);
        }
        
    }
}
