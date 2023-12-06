using UnityEngine;

public class Board : MonoBehaviour
{
    public bool GameRunning { get; set; }
    public GameObject ballPrefab;

    public void OnClick()
    {
        if (!GameRunning)
            return;
        var newB = Instantiate(ballPrefab);
        newB.transform.Translate(0, 0.01f, 0);
        var forward = new Vector3(Random.value - 0.5f, 1, 2);
        newB.GetComponent<Rigidbody>().AddForce(forward * 300);
    }
}
