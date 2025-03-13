using UnityEngine;
using System.Collections;
using System;
using Unity.Cinemachine;

public class DashJump : MonoBehaviour
{
    [Header("References")]
    [SerializeField] LayerMask collisionMask;
    [SerializeField] PlayerControl pc;
    [SerializeField] Transform cam;

    [Header("Dash Settings")]
    [SerializeField] float distance; // Dash speed
    [SerializeField] float duration = 1.0f; // Dash cooldown
    [SerializeField] float amountForward = 1.0f;
    [SerializeField] AnimationCurve animationCurve;
    [SerializeField] float dashCD;

    [Header("Jump Settings")]
    public float jumpDistance;
    public float jumpDuration;
    public float jumpAmountUp;

    Vector3 inputDirection;
    Rigidbody rb;
    Animator animator;
    Hurtbox hurtbox;

    bool forwardPressed;
    bool backPressed;
    bool leftPressed;
    bool rightPressed;
    bool shiftPressed;

    int dashHash;
    int dashTriggerHash;
    int dashXHash;
    int dashZHash;
    int jumpHash;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        pc = GetComponent<PlayerControl>();
        hurtbox = GetComponent<Hurtbox>();

        dashHash = Animator.StringToHash("Dash");
        dashTriggerHash = Animator.StringToHash("DashTrigger");
        dashXHash = Animator.StringToHash("Dash X");
        dashZHash = Animator.StringToHash("Dash Z");
        jumpHash = Animator.StringToHash("Jump");
    }

    void Update()
    {
        if (PlayerState.IsBusy() && !PlayerState.IsAttacking())
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            forwardPressed = Input.GetKey("w");
            backPressed = Input.GetKey("s");
            leftPressed = Input.GetKey("a");
            rightPressed = Input.GetKey("d");
            shiftPressed = Input.GetKey("left shift");

            CalculateCamRelativeInput();
            
            if (inputDirection.magnitude == 0 || (shiftPressed && !PlayerState.IsLockedOn()))
            {
                Jump();
            }
            else
            {
                Dash();
            }
        }
    }

    void CalculateCamRelativeInput()
    {
        inputDirection = cam.forward * (forwardPressed ? 1 : 0) + cam.right * (rightPressed ? 1 : 0) + cam.forward * (backPressed ? -1 : 0) + cam.right * (leftPressed ? -1 : 0);

        if (pc.locked && !forwardPressed && !backPressed)
        {
            inputDirection += cam.forward * (rightPressed ? amountForward : 0) + cam.forward * (leftPressed ? amountForward : 0);
        }

        inputDirection.y = 0f;
        inputDirection.Normalize();
    }

    void Dash()
    {
        StartCoroutine(DashRoutine());
        StartCoroutine(DashAnimationRoutine());
        StartCoroutine(DisableHurtboxDuringTime(0.1f * duration, 0.8f * duration));
    }

    void Jump()
    {
        StartCoroutine(JumpRoutine());
        StartCoroutine(DisableHurtboxDuringTime(0.1f * jumpDuration, 0.8f * jumpDuration));
    }

    IEnumerator DashRoutine()
    {
        PlayerState.MakeBusyForTime(duration, ActionState.Dashing);

        Vector3 startPos = transform.position + Vector3.up * 0.5f;
        Vector3 destination = transform.position + Vector3.up * 0.5f + inputDirection * distance;
        if (Physics.Raycast(transform.position, inputDirection, out RaycastHit hit, distance, collisionMask))
        {
            destination = hit.point;
        }

        Vector3 totalDisplacement = destination - startPos;

        rb.linearVelocity = Vector3.zero;
        float timeElapsed = 0f;
        while (timeElapsed < duration - 0.03)
        {
            float normalizedTime = timeElapsed / duration;            
            float currentCurveValue = animationCurve.Evaluate(normalizedTime);
            
            Vector3 instantaneousVelocity = totalDisplacement * (currentCurveValue / duration);
            
            rb.linearVelocity = instantaneousVelocity;
            rb.angularVelocity = Vector3.zero;
            
            timeElapsed += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        rb.linearVelocity = Vector3.zero;
        pc.DashCooldown(dashCD);
    }

    IEnumerator DashAnimationRoutine()
    {
        animator.SetFloat(dashXHash, inputDirection.x);
        animator.SetFloat(dashZHash, inputDirection.z);
        animator.SetBool(dashHash, true);
        animator.Play("Dashing");
        yield return new WaitForSeconds(duration);
        animator.SetFloat(dashXHash, 0);
        animator.SetFloat(dashZHash, 0);
        animator.SetBool(dashHash, false);
    }

    
    IEnumerator JumpRoutine()
    {
        PlayerState.MakeBusy(ActionState.Jumping);

        Vector3 startPos = transform.position;
        Vector3 destination = transform.position + inputDirection * jumpDistance;
        if (Physics.Raycast(transform.position + Vector3.up, inputDirection + Vector3.up, out RaycastHit hit, jumpDistance, collisionMask))
        {
            destination = hit.point - Vector3.up;
        }
        Vector3 totalDisplacement = destination - startPos;

        animator.SetBool(jumpHash, true);
        animator.Play("Jump");

        rb.linearVelocity = Vector3.zero;
        rb.AddForce(inputDirection * jumpDistance, ForceMode.Impulse);
        rb.AddForce(Vector3.up * jumpAmountUp, ForceMode.Impulse);

        while (rb.linearVelocity.y > -0.05f)
        {
            rb.angularVelocity = Vector3.zero;
            yield return new WaitForFixedUpdate();
        }

        // Wait for landing
        while (!IsGrounded())
        {
            rb.angularVelocity = Vector3.zero;
            yield return new WaitForFixedUpdate();
        }

        rb.linearVelocity = Vector3.zero;
        animator.SetBool(jumpHash, false);
        PlayerState.FreeActionState();
    }

    bool IsGrounded()
    {
        Vector3 start = transform.position + Vector3.up * 0.1f;
        float rayLength = 0.2f;
        bool grounded = Physics.Raycast(start, Vector3.down, out RaycastHit hit, rayLength, collisionMask);
        return grounded;
    }

    IEnumerator DisableHurtboxDuringTime(float startTime, float endTime)
    {
        yield return new WaitForSeconds(startTime);
        hurtbox.DisableHurtbox();
        yield return new WaitForSeconds(endTime - startTime);
        hurtbox.EnableHurtbox();
    }
}
