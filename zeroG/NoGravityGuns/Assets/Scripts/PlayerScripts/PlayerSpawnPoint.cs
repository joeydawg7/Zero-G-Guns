using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
using Photon.Pun;
using System.IO;

public class PlayerSpawnPoint : MonoBehaviour
{
    public Vector2 cubeSize;
    public GameObject netSpawnPrefab;

    public enum FacingDirection { right, left };
    public FacingDirection facingDirection;

    [Range(0, 3)]
    public int IDToSpawn = 0;

    public GameObject[] characterToSpawn;

    //Controller controller;

    //gets set to false when has a corresponding player
    [HideInInspector]
    public bool destroyOnRoundStart = true;

   // public int spawnerID;

    //shows a fun box where our spawn point is so we can place it easier
    private void OnDrawGizmos()
    {

#if UNITY_EDITOR
        Gizmos.color = Color.white;

        Gizmos.DrawCube(transform.position, cubeSize);
#endif
    }

    void Start()
    {
        if (!PhotonNetwork.IsConnectedAndReady)
        {
            PlayerDataScript[] playerDatas = FindObjectsOfType<PlayerDataScript>();
            foreach(var player in playerDatas)
            {
                if(player.playerID == IDToSpawn)
                {
                    SpawningCharacter(player);
                }                
            }            
        }  
        else
        {
            Debug.Log("NEED NET SPAWNERS");
            var networkUserDatas = FindObjectsOfType<NetworkUserData>();
            Debug.Log(" networkUserDatas.Length = " + networkUserDatas.Length);
            
            foreach (var networkUserData in networkUserDatas)
            {

                Debug.Log("networkUserData.playerData.playerID = " + networkUserData.playerData.playerID);
                Debug.Log("spawnerID = " + IDToSpawn);
                if (networkUserData.playerData.playerID == IDToSpawn)
                {

                    Debug.Log("Match Found");
                    networkUserData.CreateNetSpawner(this);                    
                    break;
                }
                                            
            }           
            this.gameObject.SetActive(false);
           
        }     
    }   

    //void OnEnable()
    //{
    //    EventManager.OnNetSpawnAction += SpawnChacterNetPlay;
    //}

    //void OnDisable()
    //{
    //EventManager.OnNetSpawnAction -= SpawnChacterNetPlay;
    //}

    //public void SpawnChacterNetPlay(int playerID)
    //{
    //    if (spawnerID == playerID)
    //    {
    //        var netSpawn = PhotonNetwork.Instantiate(Path.Combine("Network Prefabs", netSpawnPrefab.name), this.gameObject.transform.position, Quaternion.identity);
    //        this.transform.SetParent(netSpawn.transform);
    //    }
    //}
  
    public void SpawningCharacter(PlayerDataScript playerData)
    {
        if(playerData != null && playerData.playerID == IDToSpawn)
        {
            //TODO: change this from random to a choice  in GUI :D
            //for now, grabs the first in an array to spawn as the player
            GameObject character = characterToSpawn[0];

            Debug.Log("spawning " + characterToSpawn[0].name);

            //spawn char
            GameObject go = null;
            if (PhotonNetwork.IsConnected)
            {
                go = PhotonNetwork.Instantiate(Path.Combine("Network Prefabs", characterToSpawn[0].name), transform.position, Quaternion.identity);
            }
            else
            {
                go = GameObject.Instantiate(characterToSpawn[0], transform.position, Quaternion.identity);
            }

            //grab the script for use later
            PlayerScript playerScript = go.GetComponentInChildren<PlayerScript>();

            //sets everything that we store in the global player settings
            playerScript.playerName = playerData.globalPlayerSettings.playerSettings[IDToSpawn].playerName;
            playerScript.hexColorCode = playerData.globalPlayerSettings.playerSettings[IDToSpawn].playerColorHexCode;
            playerScript.collisionLayer = playerData.globalPlayerSettings.playerSettings[IDToSpawn].CollisionLayer;
            playerScript.playerColor = playerData.globalPlayerSettings.playerSettings[IDToSpawn].Color;
            playerScript.deadColor = playerData.globalPlayerSettings.playerSettings[IDToSpawn].DeadColor;

            //give the player a controller
            Debug.Log("ID TO SPAWN!!! : " + IDToSpawn);

            playerScript.SetController(playerData.playerID, playerData.playerControllerData.controller);

            //changes the name of the player so its easier to read in heirarchy
            go.name = "Player" + IDToSpawn;

            //set stuff for player canvas
            PlayerCanvasScript playerCanvasScript = Instantiate(playerData.playerCanvas).GetComponent<PlayerCanvasScript>();
            playerCanvasScript.SetPlayerCanvas(playerData.globalPlayerSettings.playerSettings[IDToSpawn].PlayerCanvasSettings.hpFront,
                playerData.globalPlayerSettings.playerSettings[IDToSpawn].PlayerCanvasSettings.hpBack,
                playerData.globalPlayerSettings.playerSettings[IDToSpawn].PlayerCanvasSettings.hpCriticalFlash,
                playerScript);

            //move canvas away until we know where to put it
            playerCanvasScript.gameObject.transform.position = new Vector2(10000, 10000);
            //if you're a winner you get a crown
            playerCanvasScript.ShowCurrentWinnerCrown(playerData.isCurrentWinner);

            //TODO: this facing direction stuff doesn't actually work right now, it just breaks things, for safety only use RIGHT FACING
            //until it gets fixed
            switch (facingDirection)
            {
                case FacingDirection.right:
                    go.transform.localScale *= 1;
                    playerCanvasScript.transform.localScale *= 1;
                    break;
                case FacingDirection.left:
                    go.transform.localScale *= 1;
                    //go.transform.localScale = new Vector2(go.transform.localScale.x * -1, go.transform.localScale.y);
                    //playerCanvasScript.transform.localScale =  new Vector2(playerCanvasScript.transform.localScale.x * -1, playerCanvasScript.transform.localScale.y);
                    break;
                default:
                    go.transform.localScale *= 1;
                    playerCanvasScript.transform.localScale *= 1;
                    break;
            }
            //gives the canvas to the player
            playerScript.playerCanvasScript = playerCanvasScript;
            //size the canvas using magic
            playerCanvasScript.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 8);
            playerCanvasScript.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 21);

            //starting stuff for the player we've spawned
            playerScript.OnGameStart();

            //turns off the spawn point now that we are done with it
            gameObject.SetActive(false);

            if(!GameManager.Instance.playerLocations.Contains(go.transform))
            {
                GameManager.Instance.playerLocations.Add(playerScript.rb.transform);
            }            
        }
    }

}
