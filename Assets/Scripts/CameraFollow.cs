using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Vector2 clampInDegrees = new Vector2(360, 180);
    public Vector2 sensitivity = new Vector2(2, 2);
    public Vector2 smoothing = new Vector2(3, 3);
    public Vector2 targetDirection;
    public Vector2 targetCharacterDirection;

    public Vector3 mouseOrigin = new Vector3();
    private bool m_isRottating = true;
    public float speed = 6.0F;
    public float jumpSpeed = 8.0F;
    public float gravity = 20.0F;
    public float turnSpeed = 200.0F;
    private Vector3 m_moveDirection = Vector3.zero;
    private Vector2 m_mouseAbsolute;
    private Vector2 m_smoothMouse;

    // Assign this if there's a parent object controlling motion, such as a Character Controller.
    // Yaw rotation will affect this object instead of the camera if set.
    // Use this for initialization
    void Start()
    {
        mouseOrigin = Input.mousePosition;

        // Set target direction to the camera's initial orientation.
        targetDirection = transform.localRotation.eulerAngles;

        // Set target direction for the character body to its inital state.
        Vector3 vec = transform.localRotation.eulerAngles;
        targetCharacterDirection = vec;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
            m_isRottating = !m_isRottating;

        if (m_isRottating == false)
            return;

        CharacterController controller = GetComponent<CharacterController>();
        if (controller.isGrounded)
        {
            m_moveDirection = new Vector3(Input.GetAxis("Vertical"), 0, -Input.GetAxis("Horizontal"));
            m_moveDirection = transform.TransformDirection(m_moveDirection);
            m_moveDirection *= speed;
            if (Input.GetButton("Jump"))
                m_moveDirection.y = jumpSpeed;
        }

        m_moveDirection.y -= gravity * Time.deltaTime;
        controller.Move(m_moveDirection * Time.deltaTime);

        // Allow the script to clamp based on a desired target value.
        var targetOrientation = Quaternion.Euler(targetDirection);
        var targetCharacterOrientation = Quaternion.Euler(targetCharacterDirection);

        // Get raw mouse input for a cleaner reading on more sensitive mice.
        var mouseDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

        // Scale input against the sensitivity setting and multiply that against the smoothing value.
        mouseDelta = Vector2.Scale(mouseDelta, new Vector2(sensitivity.x * smoothing.x, sensitivity.y * smoothing.y));

        // Interpolate mouse movement over time to apply smoothing delta.
        m_smoothMouse.x = Mathf.Lerp(m_smoothMouse.x, mouseDelta.x, 1f / smoothing.x);
        m_smoothMouse.y = Mathf.Lerp(m_smoothMouse.y, mouseDelta.y, 1f / smoothing.y);

        // Find the absolute mouse movement value from point zero.
        m_mouseAbsolute += m_smoothMouse;

        // Clamp and apply the local x value first, so as not to be affected by world transforms.
        if (clampInDegrees.x < 360)
            m_mouseAbsolute.x = Mathf.Clamp(m_mouseAbsolute.x, -clampInDegrees.x * 0.5f, clampInDegrees.x * 0.5f);

        // Then clamp and apply the global y value.
        if (clampInDegrees.y < 360)
            m_mouseAbsolute.y = Mathf.Clamp(m_mouseAbsolute.y, -clampInDegrees.y * 0.5f, clampInDegrees.y * 0.5f);

        GameObject.Find("Head").transform.localRotation = Quaternion.AngleAxis(-m_mouseAbsolute.y, targetOrientation * Vector3.right) * targetOrientation;

        // If there's a character body that acts as a parent to the camera
        var yRotation = Quaternion.AngleAxis(m_mouseAbsolute.x, Vector3.up);
        transform.localRotation = yRotation * targetCharacterOrientation;
    }
}