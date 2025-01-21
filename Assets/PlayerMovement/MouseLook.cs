using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float mouseSensitivity = 100f; // Adjust the speed of camera rotation
    private Transform cameraTransform;    // Reference to the camera's transform
    private float xRotation = 0f;
    private float yRotation = 0f;

    void Start()
    {
        // Lock the cursor to the center of the screen
        Cursor.lockState = CursorLockMode.Locked;
        cameraTransform = transform;
    }

    void Update()
    {
        // Get mouse movement input
        xRotation += Input.GetAxisRaw("Mouse Y") * mouseSensitivity * Time.unscaledDeltaTime;
        yRotation += Input.GetAxisRaw("Mouse X") * mouseSensitivity * Time.unscaledDeltaTime;

        // Set vertical look rotation with limits
        xRotation = Mathf.Clamp(-xRotation, -90f, 90f);

        // Apply rotation
        transform.localRotation = Quaternion.Euler(0f, yRotation, 0f);
    }

    private void HandleMouseMovement()
    {

    }
}
