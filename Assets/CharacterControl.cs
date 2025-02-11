using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class CharacterControl : MonoBehaviour
{
    protected Rigidbody rb;
    protected bool isKnockedBack = false;
    protected Animator animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        animator = GetComponent<Animator>(); 
        rb = GetComponent<Rigidbody>(); 
    }

    public virtual void ApplyKnockback(Vector3 direction)
    {
        animator.Play("Knockback");
    }

    protected float GetAnimationLength(string clipName)
    {
        AnimationClip clip = animator.runtimeAnimatorController.animationClips
            .FirstOrDefault(clip => clip.name == clipName);
            
        if (clip != null)
        {
            return clip.length;
        }
        
        Debug.LogWarning($"Animation clip {clipName} not found!");
        return 0f;
    }


}
