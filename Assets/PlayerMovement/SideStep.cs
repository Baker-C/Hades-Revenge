using UnityEngine;
using System.Collections;

public class SideStep : MonoBehaviour
{
    [Header("Sidestep Settings")]
    [SerializeField] private float sideStepSpeed = 20.0f; // Dash speed
    [SerializeField] private float sideStepDuration = 1.0f; // Dash cooldown
    [SerializeField] private float velocityFallOffRate = 0.1f; // Time step for movement
    [SerializeField] private float slowMotionFactor = 0.5f; // Fraction of time speed

    [SerializeField] private AnimationCurve slowMotionCurve;

    private bool isStepping = false;
    private Rigidbody rb;
    private Camera mainCamera;
    private Vector3 movement;
    private Vector3 cameraForward;
    private Vector3 cameraRight;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        mainCamera = Camera.main;

    }

    void Update()
    {
        if (isStepping)
            return;

        // Input for movement
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        // Calculate movement vector relative to the camera
        movement = CalculateCameraRelativeMovement(moveHorizontal, moveVertical);

        if (Input.GetKeyDown(KeyCode.Space) && movement.magnitude > 0.0f)
        {
            StartCoroutine(SideStepRoutine(movement));
        }
    }

    private Vector3 CalculateCameraRelativeMovement(float horizontal, float vertical)
    {
        cameraForward = mainCamera.transform.forward;
        cameraRight = mainCamera.transform.right;

        // Flatten vectors to ignore vertical tilt of the camera
        cameraForward.y = 0f;
        cameraRight.y = 0f;

        // Normalize for consistent movement speed
        cameraForward.Normalize();
        cameraRight.Normalize();

        return (cameraForward * vertical + cameraRight * horizontal).normalized;
    }

    private IEnumerator SideStepRoutine(Vector3 movement)
    {
        // Slows down time for the duration of the dash
        SlowMotionTrigger.TriggerSlowMotion(slowMotionFactor, sideStepDuration, slowMotionCurve);

        // For keeping track of time
        float t = 0.0f;

        isStepping = true;
        while (t <= 1.0f)
        {
            t += velocityFallOffRate;
            // Apply sidestep force
            rb.linearVelocity = Vector3.Lerp(movement * sideStepSpeed, movement, t);

            // Wait for step duration
            yield return new WaitForSeconds(sideStepDuration * velocityFallOffRate);

        }
        rb.linearVelocity = Vector3.zero;
        isStepping = false;
    }
}
