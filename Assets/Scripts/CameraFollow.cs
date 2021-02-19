using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private float panSpeed = 10;
    private Vector3 panLimit = new Vector3(30, 15, 30);
    private float speed = 2f;

    void Start()
    {

    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            transform.Rotate(new Vector3(Input.GetAxis("Mouse Y") * speed, -Input.GetAxis("Mouse X") * speed, 0));
            float X = transform.rotation.eulerAngles.x;
            float Y = transform.rotation.eulerAngles.y;
            transform.rotation = Quaternion.Euler(X, Y, 0);
        }

        if (Input.GetKey("w"))
        {
            transform.Translate(transform.forward * panSpeed * Time.deltaTime, Space.World);
        }
        if (Input.GetKey("d"))
        {
            transform.Translate(transform.right * panSpeed * Time.deltaTime, Space.World);
        }
        if (Input.GetKey("q"))
        {
            transform.Translate(transform.up * panSpeed * Time.deltaTime, Space.World);
        }
        if (Input.GetKey("s"))
        {
            transform.Translate(-transform.forward * panSpeed * Time.deltaTime, Space.World);
        }
        if (Input.GetKey("a"))
        {
            transform.Translate(-transform.right * panSpeed * Time.deltaTime, Space.World);
        }
        if (Input.GetKey("e"))
        {
            transform.Translate(-transform.up * panSpeed * Time.deltaTime, Space.World);
        }
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, -panLimit.x, panLimit.x);
        pos.y = Mathf.Clamp(pos.y, 1, panLimit.y);
        pos.z = Mathf.Clamp(pos.z, -panLimit.z, panLimit.z);
        transform.position = pos;
    }

}