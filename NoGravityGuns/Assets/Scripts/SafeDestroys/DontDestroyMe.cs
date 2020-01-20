using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//attach to any gameobject that is easier to keep alive between rounds

public class DontDestroyMe : MonoBehaviour
{
    private void Start()
    {
        //if(RoundManager.Instance.currentRound==1)
        gameObject.DontDestroyOnLoad();
    }
}
