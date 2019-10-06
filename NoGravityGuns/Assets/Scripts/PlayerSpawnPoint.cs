﻿using System.Collections;
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


    public void SpawnCharacter(int IDToSpawn, Controller controller)
    {

        //TODO: change this from random to a choice  in GUI :D
        GameObject character = characterToSpawn[0];

        Debug.Log("spawning " + characterToSpawn[0].name);

        GameObject go = GameObject.Instantiate(characterToSpawn[0], transform.position, Quaternion.identity);

        PlayerScript playerScript = go.GetComponentInChildren<PlayerScript>();

        playerScript.SetController(IDToSpawn, controller);

        go.name = "Player" + IDToSpawn;
        //go.transform.parent = transform;


        switch (facingDirection)
        {
            case FacingDirection.right:
                go.transform.localScale *= 1;
                break;
            case FacingDirection.left:
                go.transform.localScale = new Vector2(go.transform.localScale.x * -1, go.transform.localScale.y);
                break;
            default:
                go.transform.localScale *= 1;
                break;
        }

        playerScript.playerName = "Red Player";
        playerScript.hexColorCode = "#666666";

    }

}