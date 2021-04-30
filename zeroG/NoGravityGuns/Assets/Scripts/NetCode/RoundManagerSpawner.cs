using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using System.IO;

public class RoundManagerSpawner : MonoBehaviour
{

    public GameObject managerToSpawn;

    // Start is called before the first frame update
    void Awake()
    {
        PhotonNetwork.Instantiate(Path.Combine("Network Prefabs", managerToSpawn.name), Vector3.zero, Quaternion.identity);
    }
}
