using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SyncSceneObjectIfNetGame : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if(!PhotonNetwork.IsConnected)
        {
            if (this.gameObject.GetComponent<PhotonTransformView>())
            {
                this.gameObject.GetComponent<PhotonTransformView>().enabled = false;
            }
            if(this.gameObject.GetComponent<PhotonView>())
            {
                this.gameObject.GetComponent<PhotonView>().enabled = false;
            }           
        }        
    }   
}
