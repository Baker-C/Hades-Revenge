using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class PlayerControl : CharacterControl
{
    [Header("References")]
    [SerializeField] LayerMask collisionMask;
    [SerializeField] Animator stateCamAnimator;

    public bool locked = false;
    public bool dashCD = false;
    
    int VelocityXHash;
    int VelocityZHash;
    int LockedOnHash;


    UnlockedMovement unlockedMovement;
    LockedMovement lockedMovement;
    
    
    override protected void Start()
    {
        base.Start(); 

        VelocityXHash = Animator.StringToHash("Velocity X");  
        VelocityZHash = Animator.StringToHash("Velocity Z");
        LockedOnHash = Animator.StringToHash("LockedOn");

        lockedMovement = GetComponent<LockedMovement>();
        unlockedMovement = GetComponent<UnlockedMovement>();
    }

    void Update()
    {
        if (PlayerState.IsBusy())
        {
            AnimateMovement(0,0);
            return;
        }
        bool lockTogglePressed = Input.GetKeyDown("q");
        if (lockTogglePressed)
            ToggleLockedOn();
    }

    public void DashCooldown(float time)
    {
        StartCoroutine(DashCooldownRoutine(time));
    }

    IEnumerator DashCooldownRoutine(float time)
    {
        dashCD = true;
        yield return new WaitForSeconds(time);
        dashCD = false;
    }

    public void ToggleLockedOn()
    {
        locked = !locked;
        if (lockedMovement)
            lockedMovement.enabled = locked;
        if (unlockedMovement)
            unlockedMovement.enabled = !locked;
        stateCamAnimator.SetBool(LockedOnHash, locked);
        if (locked)
            PlayerState.LockMovement();
        else
            PlayerState.UnlockMovement();
    }

    public void AnimateMovement(float x, float z)
    {
        animator.SetFloat(VelocityXHash, x);
        animator.SetFloat(VelocityZHash, z);
    }

    override public void ApplyKnockback(float amount, Vector3 direction)
    {
        base.ApplyKnockback(amount, direction);
        PlayerState.MakeBusyForTime(GetAnimationLength("Knockback"));
    }

    override public void Die()
    {
        PlayerState.MakeBusy();
        animator.SetBool("Dead", true);
        animator.Play("Death");
        StartCoroutine(Respawn());
    }

    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(3.0f);
        transform.position = PlayerState.GetSpawnPosition();
        cHealth.ResetHealth();
        PlayerState.FreeActionState();
        animator.SetBool("Dead", false);
    }



    
}
