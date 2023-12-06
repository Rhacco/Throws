using System;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public GameObject ballSpawn;
    public GameObject throwPreview;
    public bool GameRunning
    {
        get => gameRunning;
        set
        {
            gameRunning = value;
            Destroy(previewBall);
            Destroy(previewLine);
            Time.timeScale = value ? 1 : 0;
        }
    }
    private bool gameRunning = false;
    private GameObject waitingBall = null;
    private GameObject previewBall = null;
    private LineRenderer previewLine = null;
    private Vector3 mp;
    private readonly List<GameObject> flyingBalls = new();
    private readonly List<Vector3> destinations = new();

    void Update()
    {
        UpdateFlyingBalls();
        if (!GameRunning)
            return;
        if (waitingBall == null)
            waitingBall = Instantiate(ballSpawn);
        if (Input.GetMouseButton(0))
        {
            if (previewBall == null)
                SpawnThrowPreview();
            MoveThrowPreview();
        }
        else if (previewBall != null && Input.GetMouseButtonUp(0))
            ThrowBall();
    }

    void SpawnThrowPreview()
    {
        previewBall = Instantiate(throwPreview);
        var l = new GameObject();
        var start = waitingBall.transform.position;
        start.y = 0.01f;
        l.transform.position = start;
        l.AddComponent<LineRenderer>();
        previewLine = l.GetComponent<LineRenderer>();
        previewLine.startWidth = 0.2f;
        previewLine.endWidth = 0.4f;
        previewLine.SetPosition(0, start);
        var end = previewBall.transform.position;
        end.y = 0.01f;
        previewLine.SetPosition(1, end);
        mp = Input.mousePosition;
    }

    void MoveThrowPreview()
    {
        var pbp = previewBall.transform.position;
        var diff = Input.mousePosition - mp;
        diff *= 0.1f;
        var x = Math.Max(-5, Math.Min(pbp.x + diff.x, 5));
        var z = Math.Max(0, Math.Min(pbp.z + diff.y, 15));
        previewBall.transform.position = new Vector3(x, 0, z);
        var end = previewBall.transform.position;
        end.y = 0.01f;
        previewLine.SetPosition(1, end);
        mp = Input.mousePosition;
    }

    void ThrowBall()
    {
        var dir = previewBall.transform.position - waitingBall.transform.position;
        var f = 69 * (float)Math.Log(dir.magnitude, 1000);
        dir.y = 10;
        waitingBall.GetComponent<Rigidbody>().AddForce(f * dir);
        waitingBall.GetComponent<Rigidbody>().useGravity = true;
        flyingBalls.Add(waitingBall);
        destinations.Add(previewBall.transform.position);
        waitingBall = Instantiate(ballSpawn);
        Destroy(previewBall);
        Destroy(previewLine);
    }

    void UpdateFlyingBalls()
    {
        for (int i = 0; i < flyingBalls.Count; i++)
        {
            var flying = flyingBalls[i];
            var fp = flying.transform.position;
            if (flying.transform.position.y < 0)
                flying.transform.position = new Vector3(fp.x, 0, fp.z);
            if (fp.z > destinations[i].z)
            {
                flying.transform.position = destinations[i];
                Destroy(flying.GetComponent<Rigidbody>());
                flyingBalls.RemoveAt(i);
                destinations.RemoveAt(i);
                return;
            }
        }
    }
}
