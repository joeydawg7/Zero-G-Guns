using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void KillMe()
    {
        Destroy(gameObject);
    }

    public void TurnMeOff(string animType)
    {
        gameObject.SetActive(false);
        GetComponent<Animator>().SetTrigger(animType);

    }
}
