using UnityEngine;

public class DirectionalMovement : MonoBehaviour
{
    public float speed = 5.0f; // Movement speed
    private Rigidbody rb; // Rigidbody reference
    private Vector3 movement; // Movement vector
    private Camera mainCamera; // Miain camera reference

    void Start()
    {
        // Initialize Rigidbody
        rb = GetComponent<Rigidbody>();
        mainCamera = Camera.main;
    }

    void Update()
    {
        // Read input and calculate movement
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        // Get camera forward and right vectors
        Vector3 cameraForward = mainCamera.transform.forward;
        Vector3 cameraRight = mainCamera.transform.right;

        // Project vectors onto xz plane
        cameraForward.y = 0;
        cameraRight.y = 0;
        cameraForward = cameraForward.normalized;
        cameraRight = cameraRight.normalized;

        movement = cameraForward * moveVertical + cameraRight * moveHorizontal;

        // Normalize movement for diagonal speed consistency
        movement = movement.normalized;
    }

    void FixedUpdate()
    {
        // Apply movement in FixedUpdate for physics
        rb.MovePosition(transform.position + movement * speed * Time.fixedDeltaTime);
    }
}
