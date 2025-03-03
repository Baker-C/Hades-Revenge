using UnityEngine;
using System.Collections;

public class Dash : MonoBehaviour
{
    [SerializeField] private LayerMask collisionMask;
    [SerializeField] float distance; // Dash speed
    [SerializeField] float duration = 1.0f; // Dash cooldown
    [SerializeField] AnimationCurve animationCurve;

    PlayerState ps;
    Transform camera;
    Vector3 inputDirection;
    Rigidbody rb;
    Animator animator;

    bool forwardPressed;
    bool backPressed;
    bool leftPressed;
    bool rightPressed;

    int dashHash;
    int dashTriggerHash;

    void Start()
    {
        ps = GetComponent<PlayerState>();
        camera = Camera.main.transform;
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        dashHash = Animator.StringToHash("Dash");
        dashTriggerHash = Animator.StringToHash("DashTrigger");
    }

    void FixedUpdate()
    {
        if (PlayerState.IsBusy())
            return;

        forwardPressed = Input.GetKey("w");
        backPressed = Input.GetKey("s");
        leftPressed = Input.GetKey("a");
        rightPressed = Input.GetKey("d");

        // Calculate movement vector relative to the camera
        CalculateCamRelativeInput();
        if (Input.GetKeyDown(KeyCode.Space) && inputDirection.magnitude > 0f)
        {
            StartCoroutine(DashRoutine());
        }
    }

    void CalculateCamRelativeInput()
    {
        inputDirection = camera.forward * (forwardPressed ? 1 : 0) + camera.right * (rightPressed ? 1 : 0) + camera.forward * (backPressed ? -1 : 0) + camera.right * (leftPressed ? -1 : 0);

        inputDirection.y = 0f;
        inputDirection.Normalize();
    }

    IEnumerator DashRoutine()
    {
        PlayerState.MakeBusyForTime(duration);

        animator.SetTrigger(dashTriggerHash);
        animator.SetBool(dashHash, true);

        Vector3 startPos = transform.position;
        Vector3 destination = transform.position + inputDirection * distance;
        if (Physics.Raycast(transform.position, inputDirection, out RaycastHit hit, distance, collisionMask))
        {
            destination = hit.point;
        }

        Vector3 totalDisplacement = destination - startPos;

        rb.linearVelocity = Vector3.zero;
        float timeElapsed = 0f;
        while (timeElapsed < duration)
        {
            float normalizedTime = timeElapsed / duration;
            float nextNormalizedTime = (timeElapsed + Time.fixedDeltaTime) / duration;
            
            float currentCurveValue = animationCurve.Evaluate(normalizedTime);
            float nextCurveValue = animationCurve.Evaluate(nextNormalizedTime);
            
            float curveDerivative = (nextCurveValue - currentCurveValue) / Time.fixedDeltaTime;
            
            Vector3 instantaneousVelocity = totalDisplacement * (curveDerivative / duration);
            
            rb.linearVelocity = instantaneousVelocity;
            rb.angularVelocity = Vector3.zero;
            
            timeElapsed += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        
        rb.linearVelocity = Vector3.zero;
        animator.SetBool("Dash", false);
    }
}
