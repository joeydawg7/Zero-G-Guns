using Photon.Pun;
using Rewired;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class NetworkUserData : MonoBehaviour
{
    PhotonView PV;

    public PlayerDataScript playerData;
    public bool isMasterClient;
    public NetSpawner[] netSpawners;

    [Header("Player Data")]
    public GlobalPlayerSettingsSO globalPlayerSettings;
    public GameObject playerCanvasPrefab;    

    void Awake()
    {
        PV = this.gameObject.GetComponent<PhotonView>();
        DontDestroyOnLoad(this.gameObject);        
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Start filling player Data");
        playerData.playerID = PV.OwnerActorNr - 1;
        playerData.name = PV.Owner.NickName;
        playerData.playerName = PV.Owner.NickName;
        playerData.hexColorCode = globalPlayerSettings.playerSettings[playerData.playerID].playerColorHexCode;
        playerData.globalPlayerSettings = globalPlayerSettings;
        playerData.playerCanvas = playerCanvasPrefab;
        playerData.isCurrentWinner = false;
        Debug.Log("Done filling player Data");

        AssignNextPlayer();
    }

    private void Update()
    {
        if (!GameManager.Instance)
        {
            CheckForSubmit();           
        }
        else
        {
            if (!GameManager.Instance.isGameStarted)
            {                
                CheckForSubmit();              
            }
        }        
    }



    //assigns the next available controller to a player
    public void AssignNextPlayer()
    {
        if(PV.IsMine)
        {
            var player = ReInput.players.GetSystemPlayer();

            Joystick joyStick = ReInput.controllers.GetController(ControllerType.Joystick, 0) as Joystick;

            player.controllers.AddController(joyStick, false);
            PlayerControllerData playerControllerData;

            playerControllerData = new PlayerControllerData(playerData.playerID, joyStick);

            Debug.Log("Assigned " + joyStick.name + " " + joyStick.id + " to Player " + player.descriptiveName);

            playerData.playerControllerData = playerControllerData;
        }        
    }

    //void OnDisable()
    //{
    //    EventManager.OnJoin -= SubmitPlayer;
    //}

    //void OnEnable()
    //{
    //    EventManager.OnJoin += SubmitPlayer;
    //}

    private void SubmitPlayer()
    {
        if(PV.IsMine)
        {
            Debug.Log("PLAYER SUBMIT RAISED BY " + PV.OwnerActorNr);
            //NetworkJoiningPlayerScript.Instance.AssignNextPlayer(playerData);
            PV.RPC("RPC_AssignNextPlayer", RpcTarget.AllBuffered, playerData.playerID);            
            //buzz joining controller so its easier to tell who just joined
            NetworkJoiningPlayerScript.Instance.Vibrate(0.5f, 0.25f, 0);
           
            //buzz joining controller so its easier to tell who just joined
            NetworkJoiningPlayerScript.Instance.Vibrate(0.5f, 0.25f, 0);
            SortGlobalPlayerSettings();
        }
       
    }

    private void SortGlobalPlayerSettings()
    {
        Debug.Log("PLAYER Sort global player setting for  = " + PV.OwnerActorNr);
        globalPlayerSettings.SortPlayerSettings();
    }

    public void CreateNetSpawner(PlayerSpawnPoint spawner)
    {
        if (PV.IsMine)
        {
            var netSpawnerObj = PhotonNetwork.Instantiate(Path.Combine("Network Prefabs", netSpawners[spawner.IDToSpawn].name), spawner.transform.position, Quaternion.identity);
            var netSpawner = netSpawnerObj.GetComponent<NetSpawner>();
            netSpawner.PV = netSpawner.gameObject.GetComponent<PhotonView>();
            netSpawner.playerData = playerData;
            netSpawner.PV.RPC("RPC_SpawnPlayer", RpcTarget.AllBuffered);
        }
    }

    [PunRPC]
    void RPC_LoadLevelAcrossNetwork(string levelBeingLoaded)
    {
        if (PV.IsMine)
        {
            PhotonNetwork.LoadLevel(levelBeingLoaded);
        }
    }

    [PunRPC]
    //assigns the next available controller to a player
    public void RPC_AssignNextPlayer(int playerDataID)
    {        
        NetworkJoiningPlayerScript.Instance.ChangeColourOfIcon(playerDataID);          
    }

    public void CheckForSubmit()
    {       
            if (ReInput.players.GetSystemPlayer().GetButtonDown("UISubmit"))
            {
                if(PV.IsMine)
                {
                    SubmitPlayer();
                }
            }          
    }
}