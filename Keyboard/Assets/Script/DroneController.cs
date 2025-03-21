using UnityEngine;

public class DroneController : MonoBehaviour
{
    public float speed = 10f;           // Movement speed
    public float rotationSpeed = 100f;  // Rotation speed
    public float verticalSpeed = 5f;    // Vertical movement speed
    public float mouseSensitivity = 1f; // Sensitivity for mouse rotation

    public Transform cameraTransform;   // Reference to the camera transform
    public float cameraHeight = 10f;    // Height of the camera relative to the drone
    public float cameraSmoothSpeed = 0.125f; // Camera follow speed

    private float pitch = 0f;   // To limit up/down rotation (vertical)
    private float yaw = 0f;     // To track horizontal mouse movement

    void Update()
    {
        // Get input for forward, backward, and sideways movement
        float moveForward = Input.GetAxis("Vertical") * speed * Time.deltaTime;
        float moveSide = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        float moveVertical = 0f;

        // Use the Jump button (usually Space) for vertical movement (up/down)
        if (Input.GetButton("Jump"))
        {
            moveVertical = verticalSpeed * Time.deltaTime; // Move up
        }
        else if (Input.GetButton("Fire3")) // Typically Left Ctrl or custom for moving down
        {
            moveVertical = -verticalSpeed * Time.deltaTime; // Move down
        }

        // Move the drone in the 3D space (left-right, forward-backward, up-down)
        transform.Translate(moveSide, moveVertical, moveForward);

        // Mouse look: Get the mouse input for rotating the drone
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        yaw += mouseX * rotationSpeed * Time.deltaTime;
        pitch -= mouseY * rotationSpeed * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, -60f, 60f); // Limit pitch (vertical rotation) to prevent flipping

        // Rotate the drone based on mouse movement
        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);

        // Camera follow logic
        MoveCamera();
    }

    // Method to move the camera based on drone's position and user input
    void MoveCamera()
    {
        // Calculate desired camera position (camera follows drone's position)
        Vector3 desiredPosition = new Vector3(transform.position.x, transform.position.y + cameraHeight, transform.position.z);

        // Smoothly move the camera towards the desired position
        cameraTransform.position = Vector3.Lerp(cameraTransform.position, desiredPosition, cameraSmoothSpeed);
    }
}