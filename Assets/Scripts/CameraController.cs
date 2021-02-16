using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Camera mainCamera;
    public Camera secondCamera;

    private readonly Vector3 defaultPosMainCamera = new Vector3(1, 3.5f, -2);
    private readonly Vector3 defaultPosSecondCamera = new Vector3(0, 5, 0);

    void Start()
    {
        mainCamera.enabled = true;
        secondCamera.enabled = false;
    }

    void Update()
    {
        if (Input.GetKeyDown("o"))
        {
            mainCamera.transform.position = defaultPosMainCamera;
            mainCamera.enabled = true;
            secondCamera.enabled = false;
        }

        if (Input.GetKeyDown("p"))
        {
            secondCamera.transform.position = defaultPosSecondCamera;
            mainCamera.enabled = false;
            secondCamera.enabled = true;
        }

        if (Input.GetKeyDown("r"))
        {
            mainCamera.transform.position = defaultPosMainCamera;
            secondCamera.transform.position = defaultPosSecondCamera;
        }
    }
}
