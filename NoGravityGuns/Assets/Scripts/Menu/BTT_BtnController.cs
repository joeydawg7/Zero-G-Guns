using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTT_BtnController : MonoBehaviour
{
    public int levelID;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClick()
    {
        BTT_Manager.Instance.NewBTT_Level(levelID);
    }
}
