using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class PlayerSpawnPoint : MonoBehaviour
{
    public Vector2 cubeSize;

    public enum FacingDirection { right, left };
    public FacingDirection facingDirection;

    [Header("0 will spawn nothing, 1-4 spawn respective ID")]
    [Range(0, 4)]
    public int IDToSpawn = 0;

    public GameObject[] characterToSpawn;

    Controller controller;

    bool hasAssignedCharacter = false;


    //gets set to false when has a corresponding player
    [HideInInspector]
    public bool destroyOnRoundStart = true;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;

        Gizmos.DrawCube(transform.position, cubeSize);
    }

    public void SetCharacter(int IDToSpawn, Controller controller)
    {
        //dont need to do this if a char is already assigned
        if (hasAssignedCharacter)
            return;

        //TODO: change this from random to a choice  in GUI :D
        GameObject character = characterToSpawn[Random.Range(0, characterToSpawn.Length)];

        GameObject go = GameObject.Instantiate(character, transform.position, Quaternion.identity);

        switch (facingDirection)
        {
            case FacingDirection.right:
                go.transform.localScale *= 1;
                break;
            case FacingDirection.left:
                go.transform.localScale *= -1;
                break;
            default:
                go.transform.localScale *= 1;
                break;
        }

        this.controller = controller;

        destroyOnRoundStart = false;
       

    }


    public void SpawnCharacter()
    {
        //TODO: change this from random to a choice  in GUI :D
        GameObject character = characterToSpawn[Random.Range(0, characterToSpawn.Length)];

        GameObject go = GameObject.Instantiate(character, transform.position, Quaternion.identity);

        switch (facingDirection)
        {
            case FacingDirection.right:
                go.transform.localScale *= 1;
                break;
            case FacingDirection.left:
                go.transform.localScale = new Vector2(go.transform.localScale.x*-1, go.transform.localScale.y);
                break;
            default:
                go.transform.localScale *= 1;
                break;
        }

        PlayerScript playerScript = go.GetComponentInChildren<PlayerScript>();

        playerScript.SetController(IDToSpawn, controller);

        Debug.Log(playerScript.playerID + " " + controller.id);

    }

}
