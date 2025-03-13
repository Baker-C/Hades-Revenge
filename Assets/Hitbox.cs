using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Hitbox : MonoBehaviour
{

    public float dmg = 20.0f;
    public float knockbackAmount = 5.0f;
    public Vector3 knockbackDirection = new Vector3(1,0,0);
    Collider hitboxCollider;

    void OnEnable()
    {
        if (!hitboxCollider)
            hitboxCollider = GetComponent<Collider>();
        if (hitboxCollider)
        {
            hitboxCollider.enabled = true;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Hurtbox hurtBox = other.gameObject.GetComponent<Hurtbox>();
        if (hurtBox == null)
        {
            Debug.LogWarning("No Hurtbox component found on " + other.name);
            return;
        }
        if (!hurtBox.gameObject.activeInHierarchy)
        {
            Debug.LogWarning("Hurtbox component is not active in hierarchy on " + other.name);
            return;
        }
        if (!hurtBox.enabled)
        {
            Debug.LogWarning("Hurtbox component is not enabled on " + other.name);
            return;
        }
        if (!other.enabled)
        {
            Debug.LogWarning("Collider is not enabled on " + other.name);
            return;
        }
        if (other.gameObject != hurtBox.gameObject)
        {
            Debug.LogWarning("Collider does not belong to the same GameObject as the Hurtbox on " + other.name);
            return;
        }

        Debug.Log("Hit " + other.name);

        hurtBox.RegisterHit(dmg, knockbackAmount, knockbackDirection);
        DisableHitbox();
    }

    public void DisableHitbox()
    {
        if (hitboxCollider)
            hitboxCollider.enabled = false;
        this.enabled = false;
    }
    
}
