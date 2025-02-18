using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class PlayerControl : CharacterControl
{
    [SerializeField] private float dashLength = 0.5f;
    [SerializeField] private float dashDistance = 5.0f;
    [SerializeField] private float dashCooldown = 1.0f;
    [SerializeField] private LayerMask collisionMask;

    PlayerState ps;
    private int VelocityXHash;
    private int VelocityZHash;

    private float velocityX = 0.0f;
    private float velocityZ = 0.0f;
    private bool isLocked = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    override protected void Start()
    {
        animator = GetComponent<Animator>(); 
        rb = GetComponent<Rigidbody>();
        ps = FindFirstObjectByType<PlayerState>();

        VelocityXHash = Animator.StringToHash("Velocity X");  
        VelocityZHash = Animator.StringToHash("Velocity Z");
    }

    // Update is called once per frame
    void Update()
    {
        if (isLocked)
        {
            return;
        }
        else
        {
            return;
        }
    }

    public void AnimateMovement(float x, float z)
    {
        animator.SetFloat(VelocityXHash, x);
        animator.SetFloat(VelocityZHash, z);
    }
    void HandleDash(bool dashPressed)
    {   
        // Initial acceleration
        if (dashPressed && !PlayerState.IsBusy())
        {
            StartCoroutine(Dash());
        }
    }

    override public void ApplyKnockback(Vector3 direction)
    {
        PlayerState.MakeBusyForTime(GetAnimationLength("Knockback"));
        animator.Play("Knockback", 0, 0f);
    }

    IEnumerator Dash()
    {
        PlayerState.MakeBusy();

        Vector3 direction = new Vector3(velocityX, 0.0f, velocityZ).normalized;
        float dashSpeed = dashDistance / dashLength;

        velocityX = dashSpeed * direction.x;
        velocityZ = dashSpeed * direction.z;

        Vector3 startPosition = transform.position;
        Vector3 targetPosition = transform.position + direction * dashDistance;

        Debug.DrawRay(transform.position, direction * dashDistance, Color.red, 2f);

        // Check if there's an obstacle in the way
        if (Physics.Raycast(transform.position, direction, out RaycastHit hit, dashDistance, collisionMask))
        {
            targetPosition = hit.point;
        }

        float timeElapsed = 0f;

        while (timeElapsed < dashLength)
        {
            rb.MovePosition(Vector3.Lerp(startPosition, targetPosition, timeElapsed / dashLength));
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        PlayerState.MakeBusyForTime(dashCooldown);

        velocityX = 0.0f;
        velocityZ = 0.0f;
        
        yield return null;
    }
}
