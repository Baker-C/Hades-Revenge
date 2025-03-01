using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class PlayerControl : CharacterControl
{
    [Header("References")]
    [SerializeField] private LayerMask collisionMask;
    [SerializeField] private Animator stateCamAnimator;




    PlayerState ps;
    int VelocityXHash;
    int VelocityZHash;
    int LockedOnHash;

    private float velocityX = 0.0f;
    private float velocityZ = 0.0f;
    [SerializeField] private bool locked = false;

    UnlockedMovement unlockedMovement;
    LockedMovement lockedMovement;
    
    
    override protected void Start()
    {
        base.Start(); 
        ps = FindFirstObjectByType<PlayerState>();

        VelocityXHash = Animator.StringToHash("Velocity X");  
        VelocityZHash = Animator.StringToHash("Velocity Z");
        LockedOnHash = Animator.StringToHash("LockedOn");

        lockedMovement = GetComponent<LockedMovement>();
        unlockedMovement = GetComponent<UnlockedMovement>();
    }

    void Update()
    {
        bool lockTogglePressed = Input.GetKeyDown("q");
        if (lockTogglePressed)
            ToggleLockedOn();
    }

    public void ToggleLockedOn()
    {
        locked = !locked;
        lockedMovement.enabled = locked;
        unlockedMovement.enabled = !locked;
        stateCamAnimator.SetBool(LockedOnHash, locked);
    }

    public bool LockedOn()
    {
        return locked;
    }

    public void AnimateMovement(float x, float z)
    {
        animator.SetFloat(VelocityXHash, x);
        animator.SetFloat(VelocityZHash, z);
    }

    override public void ApplyKnockback(Vector3 direction)
    {
        PlayerState.MakeBusyForTime(GetAnimationLength("Knockback"));
        animator.Play("Knockback", 0, 0f);
    }
}
