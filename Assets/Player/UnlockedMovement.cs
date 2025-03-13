using UnityEngine;

public class UnlockedMovement : MonoBehaviour
{

    [Header("References")]
    public Transform orientation;
    public Rigidbody rb;
    public Transform cam;
    
    [Header("Movement")]
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

    
    PlayerControl pc;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pc = GetComponent<PlayerControl>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {            
        if (PlayerState.IsBusy() && !PlayerState.IsJumping())
            return;

        forwardPressed = Input.GetKey("w");
        backPressed = Input.GetKey("s");
        leftPressed = Input.GetKey("a");
        rightPressed = Input.GetKey("d");
        sprintPressed = Input.GetKey("left shift");

        // Move Player
        CalculateDirection();
        CalculateVelocity();
        if (PlayerState.IsJumping())
            transform.position += movement / 3 * Time.fixedDeltaTime;
        else
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

        // Move player relative to cam orientation

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

        Vector2 inputDir = new Vector2(
            (rightPressed ? 1 : 0) - (leftPressed ? 1 : 0),
            (forwardPressed ? 1 : 0) - (backPressed ? 1 : 0)
        ).normalized;

        if (inputDir.x != 0)
            movementX = Mathf.MoveTowards(movementX, inputDir.x * maxSpeed, Time.fixedDeltaTime * acceleration);
        if (inputDir.y != 0)
            movementZ = Mathf.MoveTowards(movementZ, inputDir.y * maxSpeed, Time.fixedDeltaTime * acceleration);

        if (inputDir.x == 0)
            movementX = Mathf.MoveTowards(movementX, 0, Time.fixedDeltaTime * deceleration);
        if (inputDir.y == 0)
            movementZ = Mathf.MoveTowards(movementZ, 0, Time.fixedDeltaTime * deceleration);

        // Convert to cam relative movement
        Vector3 camForward = cam.forward;
        Vector3 camRight = cam.right;
        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        movement = (camForward * movementZ + camRight * movementX);
    }


    void CalculateDirection()
    {
        inputDirection = cam.forward * (forwardPressed ? 1 : 0) + cam.right * (rightPressed ? 1 : 0) + cam.forward * (backPressed ? -1 : 0) + cam.right * (leftPressed ? -1 : 0);

        if (inputDirection.magnitude == 0)
            return;

        inputDirection.y = 0f;
        inputDirection.Normalize();
        targetDirection = inputDirection;
    }

}
