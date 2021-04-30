using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkCanvasSpawner : MonoBehaviourPunCallbacks
{

    public GameObject joinCanvas;
    
    // Start is called before the first frame update

    void Start()
    {
        var newjoinCanvas = PhotonNetwork.Instantiate(joinCanvas.name, new Vector3(0f, 5f, 0f), Quaternion.identity, 0);
        
    }

}
