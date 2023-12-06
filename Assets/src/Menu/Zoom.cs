using UnityEngine;

public class Zoom : MonoBehaviour
{
    public Camera mainCam;

    public void OnClick()
    {
        var p = mainCam.transform.position;
        var t = p.y < 13 ? 4 : -1;
        mainCam.transform.position = new Vector3(p.x, p.y + t, p.z);
        p = transform.parent.position;
        transform.parent.position = new Vector3(p.x, p.y + t, p.z);
    }
}
