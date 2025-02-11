using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class PlayerControl : CharacterControl
{
    [SerializeField] private float acceleration = 3f;
    [SerializeField] private float deceleration = 3f;
    [SerializeField] private float sprintMultiplier = 3f;
    [SerializeField] private float maxWalkingSpeed = 1.0f;
    [SerializeField] private float maxRunningSpeed = 2.0f;

    PlayerState playerState;
    private int VelocityXHash;
    private int VelocityZHash;

    private float velocityX = 0.0f;
    private float velocityZ = 0.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    override protected void Start()
    {
        animator = GetComponent<Animator>(); 
        rb = GetComponent<Rigidbody>();
        playerState = FindFirstObjectByType<PlayerState>();

        VelocityXHash = Animator.StringToHash("Velocity X");  
        VelocityZHash = Animator.StringToHash("Velocity Z");
    }

    // Update is called once per frame
    void Update()
    {
        bool sprintPressed = Input.GetKey("left shift");
        bool forwardPressed = Input.GetKey("w");
        bool leftPressed = Input.GetKey("a");
        bool rightPressed = Input.GetKey("d");
        bool backPressed = Input.GetKey("s");

        bool dashPressed = Input.GetKeyDown(KeyCode.Space);

        HandleDash(leftPressed, rightPressed, dashPressed);

        float maxSpeed = sprintPressed ? maxRunningSpeed : maxWalkingSpeed;

        AccelerationMovement(forwardPressed, leftPressed, rightPressed, backPressed, sprintPressed, maxSpeed);
        DecelerationMovement(forwardPressed, leftPressed, rightPressed, backPressed, sprintPressed, maxSpeed);

        SetMovement();
    }

    void HandleDash(bool leftPressed, bool rightPressed, bool dashPressed)
    {   
        // Initial acceleration
        if (dashPressed && !PlayerState.IsBusy())
        {
            if (leftPressed)
            {        
                PlayerState.MakeBusy(GetAnimationLength("Dash Left"));
                animator.Play("Dash Left", 0, 0f);
            }
            else if (rightPressed)
            {
                PlayerState.MakeBusy(GetAnimationLength("Dash Right"));
        animator.Play("Dash Right", 0, 0f);;
            }
        }
    }

    void AccelerationMovement(bool forwardPressed, bool leftPressed, bool rightPressed, bool backPressed, bool sprintPressed, float currentMaxVelocity)
    {
        // Walk forward
        if (!sprintPressed && forwardPressed && velocityZ < currentMaxVelocity)
        {
            velocityZ += Time.deltaTime * acceleration;
        }
        // Walk left
        if (leftPressed && velocityX > -currentMaxVelocity)
        {
            velocityX -= Time.deltaTime * acceleration;
        }
        // Walk right
        if (rightPressed && velocityX < currentMaxVelocity)
        {
            velocityX += Time.deltaTime * acceleration;
        }
        // Walk back
        if (backPressed && velocityZ > -currentMaxVelocity)
        {
            velocityZ -= Time.deltaTime * acceleration;
        }

        // Sprint forward
        if (sprintPressed && forwardPressed && velocityZ < currentMaxVelocity)
        {
            velocityZ += Time.deltaTime * acceleration * sprintMultiplier;
        }
        // Sprint left
        if (sprintPressed && leftPressed && velocityX > -currentMaxVelocity)
        {
            velocityX -= Time.deltaTime * acceleration * sprintMultiplier;
        }
        // Sprint right
        if (sprintPressed && rightPressed && velocityX < currentMaxVelocity)
        {
            velocityX += Time.deltaTime * acceleration * sprintMultiplier;
        }
        // Sprint back
        if (sprintPressed && backPressed && velocityZ > -currentMaxVelocity)
        {
            velocityZ -= Time.deltaTime * acceleration * sprintMultiplier;
        }
    }

    void DecelerationMovement(bool forwardPressed, bool leftPressed, bool rightPressed, bool backPressed, bool sprintPressed, float currentMaxVelocity)
    {
        // Walk forward
        if (!sprintPressed && forwardPressed && velocityZ > currentMaxVelocity)
        {
            velocityZ -= Time.deltaTime * deceleration;
        }
        // Walk left
        if (!sprintPressed && leftPressed && velocityX < -currentMaxVelocity)
        {
            velocityX += Time.deltaTime * deceleration;
        }
        // Walk right
        if (!sprintPressed && rightPressed && velocityX > currentMaxVelocity)
        {
            velocityX -= Time.deltaTime * deceleration;
        }
        // Walk back
        if (!sprintPressed && backPressed && velocityZ < -currentMaxVelocity)
        {
            velocityZ += Time.deltaTime * deceleration;
        }

        // Stop forward
        if (!forwardPressed && velocityZ > 0.0f)
        {
            velocityZ -= Time.deltaTime * deceleration;
        }
        // Stop left
        if (!leftPressed && velocityX < 0.0f)
        {
            velocityX += Time.deltaTime * deceleration;
        }
        // Stop right
        if (!rightPressed && velocityX > 0.0f)
        {
            velocityX -= Time.deltaTime * deceleration;
        }
        // Stop back
        if (!backPressed && velocityZ < 0.0f)
        {
            velocityZ += Time.deltaTime * deceleration;
        }
    }

    void SetMovement()
    {
        // Clamp the velocity values to zero if they are close to zero
        if (Math.Abs(velocityX) < 0.03f)
        {
            velocityX = 0.0f;
        }
        if (Math.Abs(velocityZ) < 0.03f)
        {
            velocityZ = 0.0f;
        }

        animator.SetFloat(VelocityXHash, velocityX);
        animator.SetFloat(VelocityZHash, velocityZ);
    }

    override public void ApplyKnockback(Vector3 direction)
    {
        PlayerState.MakeBusy(GetAnimationLength("Knockback"));
        animator.Play("Knockback", 0, 0f);
    }
}