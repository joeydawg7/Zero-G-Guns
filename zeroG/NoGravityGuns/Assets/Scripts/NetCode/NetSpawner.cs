using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetSpawner : MonoBehaviour
{

    public PhotonView PV;
    public PlayerSpawnPoint playerSpawn;
    public PlayerDataScript playerData;

    //public void CallerRPC_Spawn()
    //{       
    //    if (PV.IsMine)
    //    {
    //        PV.RPC("RPC_SpawnPlayer", RpcTarget.AllBuffered);
    //    }
    //}

    [PunRPC]
    void RPC_SpawnPlayer()
    {
        if(PV != null)
        {
            if (PV.IsMine)
            {
                playerSpawn.SpawningCharacter(playerData);
            }
        }        
    }
}
