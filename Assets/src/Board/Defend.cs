using UnityEngine;

public class Defend : MonoBehaviour
{
    public Board board;
    private Bounds bounds;

    void Start()
    {
        bounds = GetComponent<Renderer>().bounds;
        //Destroy(GetComponent<Renderer>());
    }
}
