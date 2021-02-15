using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Camera mainCamera;
    public Camera secondCamera;

    void Start()
    {
        mainCamera.enabled = true;
        secondCamera.enabled = false;
    }

    void Update()
    {
        if (Input.GetKeyDown("o"))
        {
            mainCamera.enabled = true;
            secondCamera.enabled = false;
        }

        if (Input.GetKeyDown("p"))
        {
            mainCamera.enabled = false;
            secondCamera.enabled = true;
        }
    }
}
