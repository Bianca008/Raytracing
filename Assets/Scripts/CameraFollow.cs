using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform Target;
    private float smoothSpeed = 0.00001f;
    public Vector3 Offset;

    private void Start()
    {
        SetInitialPosition();
    }

    private void SetInitialPosition()
    {
        Vector3 desiredPosition = Target.position + Offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        transform.LookAt(Target);
    }

    /*LateUpdate help us to update camera after source.*/
    /*Fixed update is used to update physics.*/
    private void Update()
    {
        MoveCamera();
    }

    private void MoveCamera()
    {
        float speed = 2;
        Vector3 pos = transform.position;

        if (Input.GetKey("w"))
            pos.z += speed * Time.deltaTime;
        if (Input.GetKey("s"))
            pos.z -= speed * Time.deltaTime;
        if (Input.GetKey("d"))
            pos.x += speed * Time.deltaTime;
        if (Input.GetKey("a"))
            pos.x -= speed * Time.deltaTime;
        if (Input.GetKey("r"))
            SetInitialPosition();

        transform.position = pos;
    }
}
