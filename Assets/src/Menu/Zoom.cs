using UnityEngine;

public class Zoom : MonoBehaviour
{
    public Camera mainCam;

    public void OnClick()
    { mainCam.fieldOfView += mainCam.fieldOfView < 50 ? 20 : -2; }
}
