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

    float movementX = 0.0f;
    float movementZ = 0.0f;
    Vector3 movement;
    Vector3 inputDirection;
    Vector3 targetDirection;

    bool forwardPressed;
    bool backPressed;
    bool leftPressed;
    bool rightPressed;
    bool sprintPressed;


    
    private PlayerControl pc;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pc = GetComponent<PlayerControl>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (PlayerState.IsBusy())
            return;

        forwardPressed = Input.GetKey("w");
        backPressed = Input.GetKey("s");
        leftPressed = Input.GetKey("a");
        rightPressed = Input.GetKey("d");
        sprintPressed = Input.GetKey("left shift");

        // Move Player
        CalculateVelocity();
        transform.position += movement * Time.fixedDeltaTime;

        // // Animate - MAY USE LATER, MAY NOT
        // // Get dot product between forward vectors
        // float dotProduct = Vector3.Dot(transform.forward, inputDirection.normalized);
        // // Convert to angle (in radians)
        // float angle = Mathf.Acos(dotProduct);
        // Debug.Log("Angle in radians: " + angle);

        // float velocityX = transform.forward.magnitude * Mathf.Asin(angle);
        // float velocityZ = transform.forward.magnitude * Mathf.Acos(angle);

        // if (angle < 0.1f)
        //     velocityZ = 2;

        // Move player relative to camera orientation
        CalculateDirection();

        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

        float angleDiff = Quaternion.Angle(transform.rotation, targetRotation);
        if (angleDiff < 10f)
            transform.rotation = targetRotation;
        else
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * rotationSpeed);

        // Animate player
        pc.AnimateMovement(0f, Mathf.Abs(movement.magnitude) * 2 / sprintSpeed);
    }


    void CalculateVelocity()
    {
        float maxSpeed = sprintPressed ? sprintSpeed : walkSpeed;

        // Acceleration
        if (forwardPressed && movementZ < maxSpeed)
            movementZ += Time.fixedDeltaTime * acceleration;
        if (backPressed && movementZ > -maxSpeed)
            movementZ -= Time.fixedDeltaTime * acceleration;
        if (leftPressed && movementX > -maxSpeed)
            movementX -= Time.fixedDeltaTime * acceleration;
        if (rightPressed && movementX < maxSpeed)
            movementX += Time.fixedDeltaTime * acceleration;
        
        // Deceleration
        if (!forwardPressed && movementZ > 0.0f)
            movementZ -= Time.fixedDeltaTime * deceleration;
        if (!backPressed && movementZ < 0.0f)
            movementZ += Time.fixedDeltaTime * deceleration;
        if (!leftPressed && movementX < 0.0f)
            movementX += Time.fixedDeltaTime * deceleration;
        if (!rightPressed && movementX > 0.0f)
            movementX -= Time.fixedDeltaTime * deceleration;

        // Deceleration to walking
        if (movementZ > maxSpeed && movementZ > 0.0f)
            movementZ -= Time.fixedDeltaTime * deceleration;
        if (movementZ < -maxSpeed && movementZ < 0.0f)
            movementZ += Time.fixedDeltaTime * deceleration;
        if (movementX < -maxSpeed && movementX < 0.0f)
            movementX += Time.fixedDeltaTime * deceleration;
        if (movementX > maxSpeed && movementX > 0.0f)
            movementX -= Time.fixedDeltaTime * deceleration;

        // Clamp the velocity values to zero if they are close to zero
        if (Mathf.Abs(movementX) < 0.1f)
            movementX = 0.0f;
            rb.linearVelocity = new Vector3(0,0,0);
        if (Mathf.Abs(movementZ) < 0.1f)
            movementZ = 0.0f;
            rb.linearVelocity = new Vector3(0,0,0);
        
        Vector3 camForward = camera.forward;
        Vector3 camRight = camera.right;
        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        // Calculate final movement vector
        movement = camForward * movementZ + camRight * movementX;
        movement.y = 0f;
    }


    void CalculateDirection()
    {
        inputDirection = camera.forward * (forwardPressed ? 1 : 0) + camera.right * (rightPressed ? 1 : 0) + camera.forward * (backPressed ? -1 : 0) + camera.right * (leftPressed ? -1 : 0);

        if (inputDirection.magnitude == 0)
            return;

        inputDirection.y = 0f;
        inputDirection.Normalize();
        targetDirection = inputDirection;
    }

}
