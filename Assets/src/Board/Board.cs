using System;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public GameObject cover;
    public GameObject ballSpawn;
    public GameObject throwPreview;
    public int numElementsX = 10;
    public int numElementsZ = 10;
    public bool GameRunning
    {
        get => gameRunning;
        set
        {
            gameRunning = value;
            Destroy(previewBall);
            Destroy(previewLine);
            Time.timeScale = value ? 1 : 0;
            cover.SetActive(!value);
        }
    }
    private bool gameRunning = false;
    private GameObject waitingBall = null;
    private GameObject previewBall = null;
    private LineRenderer previewLine = null;
    private readonly float hoverY = 0.033f;
    private Vector3 mp;
    private readonly List<GameObject> flyingBalls = new();
    private readonly List<Vector3> destinations = new();

    void Update()
    {
        UpdateFlyingBalls();
        if (!GameRunning)
            return;
        if (waitingBall == null)
            waitingBall = Instantiate(ballSpawn, transform, true);
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
        previewBall = Instantiate(throwPreview, transform, true);
        waitingBall.AddComponent<LineRenderer>();
        previewLine = waitingBall.GetComponent<LineRenderer>();
        previewLine.material.color = Color.cyan;
        previewLine.startWidth = 0.2f;
        previewLine.endWidth = 0.4f;
        var start = waitingBall.transform.position;
        start.y = hoverY;
        previewLine.SetPosition(0, start);
        var end = previewBall.transform.position;
        end.y = hoverY;
        previewLine.SetPosition(1, end);
        mp = Input.mousePosition;
    }

    void MoveThrowPreview()
    {
        var pbp = previewBall.transform.position;
        var diff = Input.mousePosition - mp;
        diff *= 0.069f;
        var x = Math.Max(-5, Math.Min(pbp.x + diff.x, 5));
        var z = Math.Max(0, Math.Min(pbp.z + diff.y, 15));
        previewBall.transform.position = new Vector3(x, 0, z);
        var end = previewBall.transform.position;
        end.y = hoverY;
        previewLine.SetPosition(1, end);
        mp = Input.mousePosition;
    }

    void ThrowBall()
    {
        var dir = previewBall.transform.position - waitingBall.transform.position;
        var f = 69 * (float)Math.Log(dir.magnitude, 1000);
        dir.y = 10;
        waitingBall.AddComponent<Rigidbody>().AddForce(f * dir);
        flyingBalls.Add(waitingBall);
        destinations.Add(previewBall.transform.position);
        waitingBall = Instantiate(ballSpawn, transform, true);
        Destroy(previewBall);
        Destroy(previewLine);
    }

    void UpdateFlyingBalls()
    {
        for (int i = 0; i < flyingBalls.Count; i++)
        {
            var f = flyingBalls[i];
            f.SetActive(GameRunning);
            var fp = f.transform.position;
            if (fp.y < 0) fp.y = 0;
            f.transform.position = fp;
            if (fp.z > destinations[i].z)
            {
                f.transform.position = destinations[i];
                Destroy(f.GetComponent<Rigidbody>());
                flyingBalls.RemoveAt(i);
                destinations.RemoveAt(i);
                return;
            }
        }
    }
}
