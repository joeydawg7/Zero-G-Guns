using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Rewired;

public class BTT_BtnController : MonoBehaviour
{
    public int levelID;


    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClick()
    {
       
        BTT_Manager.Instance.NewBTT_Level(levelID);
    }
}
