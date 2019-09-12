using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillScript : MonoBehaviour
{

    public void TurnMeOff(string animType)
    {
        PlayerScript playerScript = transform.root.GetComponent<SetPlayerCanvasScript>().playerscript;
        //null this crap out... very important
        playerScript.floatingDamage = new PlayerScript.FloatingDamageStuff();
        transform.parent = null;
        DontDestroyOnLoad(gameObject);
        gameObject.SetActive(false);
        GetComponent<Animator>().SetTrigger(animType);
        
    }
}
