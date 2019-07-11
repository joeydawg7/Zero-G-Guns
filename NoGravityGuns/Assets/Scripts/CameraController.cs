using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public List<Transform> players;

    public Vector3 offset;
    public float smoothTime;

    public float minZoom;
    public float maxZoom;
    public float zoomLimit;
    public float zoomSpeed;

    Vector3 velocity;

    private void Awake()
    {
        players = new List<Transform>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (players.Count == 0)
            return;

        Move();
        Zoom();

    }

    void Move()
    {
        Vector3 centerPoint = GetCenterPoint();
        Vector3 newPosition = centerPoint + offset;
        transform.position = Vector3.SmoothDamp(transform.position, newPosition, ref velocity, smoothTime);
    }

    void Zoom()
    {
        float newZoom = Mathf.Lerp(maxZoom, minZoom, GetGreatestDistance() / zoomLimit);
        Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, newZoom, Time.deltaTime * zoomSpeed);
    }

    Vector3 GetCenterPoint()
    {
        if (players.Count == 1)
        {
            return players[0].position;
        }

        var bounds = new Bounds(players[0].position, Vector3.zero);

        for (int i = 0; i < players.Count; i++)
        {
            bounds.Encapsulate(players[i].position);
        }

        return bounds.center;
    }

    //returns between 0 and 1
    float GetGreatestDistance()
    {
        var bounds = new Bounds(players[0].position, Vector3.zero);
        for (int i = 0; i < players.Count; i++)
        {
            bounds.Encapsulate(players[i].position);
        }

        return Mathf.Max(bounds.size.x, bounds.size.y);
    }

    public void OnGameStart()
    {
        StartCoroutine(DelayOnGameStart());
    }


    IEnumerator DelayOnGameStart()
    {
        yield return new WaitForSeconds(0.25f);

        GameObject[] playersArray = GameObject.FindGameObjectsWithTag("Player");

        for (int i = 0; i < playersArray.Length; i++)
        {
            players.Add(playersArray[i].transform);
        }
    }

}
