using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Camera MainCamera;
    public Camera SecondCamera;

    private readonly Vector3 defaultPosMainCamera = new Vector3(1, 3.5f, -2);
    private readonly Vector3 defaultPosSecondCamera = new Vector3(0, 5, 0);

    void Start()
    {
        MainCamera.enabled = true;
        SecondCamera.enabled = false;
    }

    void Update()
    {
        if (Input.GetKeyDown("o"))
        {
            MainCamera.transform.position = defaultPosMainCamera;
            MainCamera.enabled = true;
            SecondCamera.enabled = false;
        }

        if (Input.GetKeyDown("p"))
        {
            SecondCamera.transform.position = defaultPosSecondCamera;
            MainCamera.enabled = false;
            SecondCamera.enabled = true;
        }

        if (Input.GetKeyDown("r"))
        {
            MainCamera.transform.position = defaultPosMainCamera;
            SecondCamera.transform.position = defaultPosSecondCamera;
        }
    }
}
