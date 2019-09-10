using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugKeys : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F1))
        {
            PlayerScript ps = GameObject.Find("Player1").GetComponent<PlayerScript>();
            ps.transform.position = ps.spawnPoint;
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            PlayerScript ps = GameObject.Find("Player2").GetComponent<PlayerScript>();
            ps.transform.position = ps.spawnPoint;
        }
        if (Input.GetKeyDown(KeyCode.F3))
        {
            PlayerScript ps = GameObject.Find("Player3").GetComponent<PlayerScript>();
            ps.transform.position = ps.spawnPoint;
        }
        if (Input.GetKeyDown(KeyCode.F4))
        {
            PlayerScript ps = GameObject.Find("Player4").GetComponent<PlayerScript>();
            ps.transform.position = ps.spawnPoint;
        }

        //starts new round
        if (Input.GetKeyDown(KeyCode.F5))
        {
           RoundManager.Instance.NewRound(false);
        }
        //starts new game
        if (Input.GetKeyDown(KeyCode.F9))
        {
            RoundManager.Instance.NewRound(true);
        }

    }
}
