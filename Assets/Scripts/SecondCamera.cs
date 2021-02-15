using UnityEngine;

public class SecondCamera : MonoBehaviour
{
    private float speed = 10.0f;

    void Update()
    {
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y - 1, transform.position.z);
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
        }

        if (Input.GetMouseButton(0))
        {
            if (Input.GetAxis("Mouse X") > 0)
            {
                transform.position += new Vector3(Input.GetAxisRaw("Mouse X") * Time.deltaTime * speed,
                                           0.0f, Input.GetAxisRaw("Mouse Y") * Time.deltaTime * speed);
            }

            else if (Input.GetAxis("Mouse X") < 0)
            {
                transform.position += new Vector3(Input.GetAxisRaw("Mouse X") * Time.deltaTime * speed,
                                           0.0f, Input.GetAxisRaw("Mouse Y") * Time.deltaTime * speed);
            }
        }
    }
}
