using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public List<GameObject> players;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float xAvg = 0;
        float yAvg = 0;

        float largestX =0;
        float largestY =0;

        foreach (var player in players)
        {
            xAvg += player.transform.position.x;
            yAvg += player.transform.position.y;

            //find largest x and y dist to use for determining camera z dist
            //if(player.transform.position.x > largestX)
            //{
            //    largestX = player.transform.position.x;
            //}
            //if (player.transform.position.y > largestY)
            //{
            //    largestY = player.transform.position.y;
            //}

        }

        xAvg = xAvg / players.Count;
        yAvg = yAvg / players.Count;

        //float zDist = (largestX - largestY)*2;

        transform.position = new Vector3(xAvg, yAvg, gameObject.transform.position.z);
    }
}
