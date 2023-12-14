using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public GameObject cover;
    public Difficulty difficulty;
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

    public List<List<Vector3>> RandomizedFieldXZ(Vector3 start, Vector3 end)
    {
        var numX = 3 + (2 * difficulty.Selected);
        var numZ = 5 + (3 * difficulty.Selected);
        var v = 0.2f; v += v * 2 * difficulty.Selected;
        var rf = new List<List<Vector3>>();
        var d = (end.x - start.x) / numX;
        end = new Vector3(start.x, 0, end.z);
        for (int i = 0; i < numX + 1; i++)
        { rf.Add(RandomizedLine(start, end, numZ, v)); start.x += d; end.x += d; }
        return rf;
    }

    private List<Vector3> RandomizedLine(Vector3 start, Vector3 end, int num, float variance)
    {
        var l = new List<Vector3>() { start };
        var d = (end - start) / num;
        for (int i = 1; i < num; i++)
            l.Add(start + (i * d) - ((Random.value - 0.5f) * variance * i * d));
        l.Add(end);
        return l;
    }

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
        var x = System.Math.Max(-5, System.Math.Min(pbp.x + diff.x, 5));
        var z = System.Math.Max(0, System.Math.Min(pbp.z + diff.y, 15));
        previewBall.transform.position = new Vector3(x, 0, z);
        var end = previewBall.transform.position;
        end.y = hoverY;
        previewLine.SetPosition(1, end);
        mp = Input.mousePosition;
    }

    void ThrowBall()
    {
        var dir = previewBall.transform.position - waitingBall.transform.position;
        var f = 69 * (float)System.Math.Log(dir.magnitude, 1000);
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
