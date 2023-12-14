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
        var f = board.RandomizedFieldXZ(bounds.min, bounds.max);
        var uv = new Vector2[] { new(0, 0), new(0, 1), new(1, 1), new(1, 0) };
        var t = new int[] { 0, 1, 2, 0, 2, 3 };
        var r = new System.Random();
        for (int i = 0; i < f.Count - 1; i++)
            for (int j = 0; j < f[i].Count - 1; j++)
            {
                var v = new Vector3[] { f[i][j], f[i][j + 1], f[i + 1][j + 1], f[i + 1][j] };
                var m = new Mesh { vertices = v, uv = uv, triangles = t };
                m.RecalculateNormals();
                var o = new GameObject("element");
                o.transform.parent = gameObject.transform;
                o.AddComponent<MeshFilter>().mesh = m;
                o.AddComponent<MeshRenderer>().material.color = colors[r.Next(colors.Length)];
            }
    }
}