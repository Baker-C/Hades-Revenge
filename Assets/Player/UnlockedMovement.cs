using UnityEngine;

public class UnlockedMovement : MonoBehaviour
{

    [Header("References")]
    public Transform orientation;
    public Rigidbody rb;
    public Transform camera;
    public float sprintSpeed;
    public float walkSpeed;
    public float rotationSpeed;
    public float acceleration;
    public float deceleration;
    private float movementX = 0.0f;
    private float movementZ = 0.0f;
    private bool lockedOn = false;
    private PlayerControl pc;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pc = GetComponent<PlayerControl>();
    }

    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        bool verticalPressed = Input.GetKey("w") || Input.GetKey("s");
        bool horizontalPressed = Input.GetKey("a") || Input.GetKey("d");
        bool sprintPressed = Input.GetKey("left shift");

        // Move Player
        Vector3 movement = CalculateVelocity(horizontal, vertical, sprintPressed);
        transform.position += movement * Time.deltaTime;

        pc.AnimateMovement(0f, movement.magnitude * 2 / sprintSpeed);

        // move player relative to camera orientation
        Vector3 inputDirection = camera.forward * (verticalPressed ? vertical : 0) + camera.right * (horizontalPressed ? horizontal : 0);
        inputDirection.y = 0f;

        // Animate
        // Get dot product between forward vectors
        float dotProduct = Vector3.Dot(transform.forward, inputDirection.normalized);
        // Convert to angle (in radians)
        float angleInRadians = Mathf.Acos(dotProduct);
        Debug.Log("Angle in radians: " + angleInRadians);
        // Convert to degrees if needed
        float angleInDegrees = angleInRadians * Mathf.Rad2Deg;
        Debug.Log("Angle in degrees: " + angleInDegrees);
        float velocityX = transform.forward.magnitude * Mathf.Asin(angleInDegrees);
        float velocityZ = transform.forward.magnitude * Mathf.Acos(angleInDegrees);

        if (angleInDegrees < 10f)
            velocityZ = 2;

        pc.AnimateMovement(velocityX, velocityZ);


        Vector3 playerOrigin = transform.position;

        // Draw the forward direction of the player (Red)
        Debug.DrawRay(playerOrigin, transform.forward * 5f, Color.red);

        // Draw the movement direction (Blue)
        Debug.DrawRay(playerOrigin, inputDirection * 5f, Color.blue);


        if (inputDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(inputDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }

    Vector3 CalculateVelocity(float horizontal, float vertical, bool sprintPressed)
    {
        bool forwardPressed = vertical > 0;
        bool leftPressed = horizontal < 0;
        bool rightPressed = horizontal > 0;
        bool backPressed = vertical < 0;

        float maxSpeed = sprintPressed ? sprintSpeed : walkSpeed;

        // Acceleration
        if (forwardPressed && movementZ < maxSpeed)
            movementZ += Time.deltaTime * acceleration;
        if (backPressed && movementZ > -maxSpeed)
            movementZ -= Time.deltaTime * acceleration;
        if (leftPressed && movementX > -maxSpeed)
            movementX -= Time.deltaTime * acceleration;
        if (rightPressed && movementX < maxSpeed)
            movementX += Time.deltaTime * acceleration;
        
        // Deceleration
        if (!forwardPressed && movementZ > 0.0f)
            movementZ -= Time.deltaTime * deceleration;
        if (!backPressed && movementZ < 0.0f)
            movementZ += Time.deltaTime * deceleration;
        if (!leftPressed && movementX < 0.0f)
            movementX += Time.deltaTime * deceleration;
        if (!rightPressed && movementX > 0.0f)
            movementX -= Time.deltaTime * deceleration;

        // Deceleration to walking
        if (movementZ > maxSpeed && movementZ > 0.0f)
            movementZ -= Time.deltaTime * deceleration;
        if (movementZ < -maxSpeed && movementZ < 0.0f)
            movementZ += Time.deltaTime * deceleration;
        if (movementX < -maxSpeed && movementX < 0.0f)
            movementX += Time.deltaTime * deceleration;
        if (movementX > maxSpeed && movementX > 0.0f)
            movementX -= Time.deltaTime * deceleration;

        // Clamp the velocity values to zero if they are close to zero
        if (Mathf.Abs(movementX) < 0.03f)
            movementX = 0.0f;
        if (Mathf.Abs(movementZ) < 0.03f)
            movementZ = 0.0f;

        // Create local movement vector
        Vector3 localVelocity = new Vector3(movementX, 0.0f, movementZ);

        // Create camera-relative movement vector
        Vector3 cameraForward = camera.forward;
        Vector3 cameraRight = camera.right;
        
        // Zero out Y components to keep movement horizontal
        cameraForward.y = 0f;
        cameraRight.y = 0f;
        
        // Normalize to maintain consistent speed
        cameraForward.Normalize();
        cameraRight.Normalize();
        
        // Calculate final movement vector
        Vector3 movement = cameraForward * localVelocity.z + cameraRight * localVelocity.x;
        return movement;
    }


}
