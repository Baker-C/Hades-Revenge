using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class CharacterControl : MonoBehaviour
{
    [SerializeField] protected LayerMask environmentMask;
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
        animator = GetComponent<Animator>(); 
        rb = GetComponent<Rigidbody>(); 
        rb.AddForce(direction * 10, ForceMode.Impulse);
        animator.Play("Knockback");
    }

    protected float GetAnimationLength(string clipName, float effectiveSpeed = 1f)
    {
        AnimationClip clip = animator.runtimeAnimatorController.animationClips
            .FirstOrDefault(clip => clip.name == clipName);
        
        if (clip != null)
        {
            return clip.length / effectiveSpeed;
        }
        
        Debug.LogWarning($"Animation clip {clipName} not found!");
        return 0f;
    }

    public virtual void AttackMove(float timeOfAttack, float distance)
    {
        StartCoroutine(AttackMoveCoroutine(timeOfAttack, distance));
    }

    protected virtual IEnumerator AttackMoveCoroutine(float duration, float distance)
    {
        Vector3 direction = transform.forward;
        direction.y = 0;
        direction.Normalize();

        Vector3 startPos = transform.position;
        Vector3 destination = transform.position + direction * distance;
        if (Physics.Raycast(startPos, direction, out RaycastHit hit, distance, environmentMask))
        {
            destination = hit.point;
        }

        yield return new WaitForSeconds(duration/4);

        float timeElapsed = 0.0f;
        while (timeElapsed <= duration/2)
        {
            transform.position = Vector3.Lerp(startPos, destination, timeElapsed);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(duration/4);
    }

    public virtual void Die()
    {
        Debug.Log("Character died");
        PlayerState.Respawn(gameObject);
        gameObject.SetActive(false);   
    }

}
