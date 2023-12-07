using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public Board board;
    private Bounds bounds;
    private readonly Color[] colors = { Color.yellow, Color.green, Color.blue, Color.magenta, Color.red };

    void Start()
    {
        bounds = GetComponent<Renderer>().bounds;
        Destroy(GetComponent<Renderer>());
        var uv = new Vector2[] { new(0, 0), new(0, 1), new(1, 1), new(1, 0) };
        var t = new int[] { 0, 1, 2, 0, 2, 3 };
        var r = new System.Random();
        var dx1 = Distribution(bounds.min.x, bounds.max.x, board.numElementsX);
        var dz = new List<List<float>>();
        for (int i = 0; i < board.numElementsX + 1; i++)
            dz.Add(Distribution(bounds.min.z, bounds.max.z, board.numElementsZ));
        for (int i = 0; i < board.numElementsZ; i++)
        {
            var dx2 = Distribution(bounds.min.x, bounds.max.x, board.numElementsX);
            for (int j = 0; j < board.numElementsX; j++)
            {
                var v1 = new Vector3(dx1[j], 0, dz[j][i]);
                var v2 = new Vector3(dx2[j], 0, dz[j][i + 1]);
                var v3 = new Vector3(dx2[j + 1], 0, dz[j + 1][i + 1]);
                var v4 = new Vector3(dx1[j + 1], 0, dz[j + 1][i]);
                var v = new Vector3[] { v1, v2, v3, v4 };
                var m = new Mesh { vertices = v, uv = uv, triangles = t };
                m.RecalculateNormals();
                var o = new GameObject("element");
                o.transform.parent = gameObject.transform;
                o.AddComponent<MeshFilter>().mesh = m;
                o.AddComponent<MeshRenderer>().material.color = colors[r.Next(colors.Length)];
            }
            dx1 = dx2;
        }
    }

    List<float> Distribution(float start, float end, int num)
    {
        var p = 0.33f;
        var v = (end - start) / num;
        var l = new List<float>() { start };
        for (int i = 1; i < num; i++)
            l.Add(start + (i * v) - (i * v * p * (Random.value - 0.5f)));
        l.Add(end);
        return l;
    }
}
