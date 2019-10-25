using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class PlayerSpawnPoint : MonoBehaviour
{
    public Vector2 cubeSize;

    public enum FacingDirection { right, left };
    public FacingDirection facingDirection;

    [Range(0, 3)]
    public int IDToSpawn = 0;

    public GameObject[] characterToSpawn;

    Controller controller;

    //gets set to false when has a corresponding player
    [HideInInspector]
    public bool destroyOnRoundStart = true;

    private void OnDrawGizmos()
    {

#if UNITY_EDITOR
        Gizmos.color = Color.white;

        Gizmos.DrawCube(transform.position, cubeSize);
#endif
    }

    //public void SetCharacter(int IDToSpawn, Controller controller)
    //{
    //    this.controller = controller;
    //    this.IDToSpawn = IDToSpawn;

    //    destroyOnRoundStart = false;


    //    SpawnCharacter();
    //}


    public void SpawnCharacter(int IDToSpawn, Controller controller, GlobalPlayerSettingsSO globalPlayerSettings, GameObject playerCanvas, bool isCurrentWinner)
    {

        //TODO: change this from random to a choice  in GUI :D
        GameObject character = characterToSpawn[0];

        Debug.Log("spawning " + characterToSpawn[0].name);

        GameObject go = GameObject.Instantiate(characterToSpawn[0], transform.position, Quaternion.identity);

        PlayerScript playerScript = go.GetComponentInChildren<PlayerScript>();

        playerScript.SetController(IDToSpawn, controller);

        go.name = "Player" + IDToSpawn;

        //sets everything from global player settings
        playerScript.playerName = globalPlayerSettings.playerSettings[IDToSpawn].playerName;
        playerScript.hexColorCode = globalPlayerSettings.playerSettings[IDToSpawn].playerColorHexCode;
        playerScript.collisionLayer = globalPlayerSettings.playerSettings[IDToSpawn].CollisionLayer;
        playerScript.playerColor = globalPlayerSettings.playerSettings[IDToSpawn].Color;
        playerScript.deadColor = globalPlayerSettings.playerSettings[IDToSpawn].DeadColor;

        //set stuff for player canvas
        PlayerCanvasScript playerCanvasScript = Instantiate(playerCanvas).GetComponent<PlayerCanvasScript>();
        playerCanvasScript.SetPlayerCanvas(globalPlayerSettings.playerSettings[IDToSpawn].PlayerCanvasSettings.hpFront,
            globalPlayerSettings.playerSettings[IDToSpawn].PlayerCanvasSettings.hpBack, globalPlayerSettings.playerSettings[IDToSpawn].PlayerCanvasSettings.hpCriticalFlash,
            playerScript);

        playerCanvasScript.gameObject.transform.position = new Vector2(10000, 10000);

        playerCanvasScript.ShowCurrentWinnerCrown(isCurrentWinner);

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
        playerCanvasScript.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 8);
        playerCanvasScript.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 21);

        playerScript.OnGameStart();

        gameObject.SetActive(false);
    }

}
