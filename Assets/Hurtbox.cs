using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Hurtbox : MonoBehaviour
{
    public PlayerHealth pHealth;
    public CharacterControl cc;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pHealth = transform.GetComponentInParent<PlayerHealth>();
        cc = transform.GetComponentInParent<PlayerControl>();
        
        // If no derived class found, try to get base CharacterControl
        if (cc == null)
        {
            cc = transform.GetComponentInParent<CharacterControl>();
        }    
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        Hitbox hitBox = other.GetComponent<Hitbox>();
        if (hitBox == null)
        {
            Debug.Log($"Collided with non-hitbox object: {other.gameObject.name}");
            return;
        }

        if (hitBox.transform.parent == transform.parent)
        {
            Debug.Log($"Hitbox belongs to same parent, ignoring collision");
            return;
        }

        Debug.Log($"Valid hitbox collision with: {hitBox.gameObject.name}");
        Debug.Log("CharControl: "+cc);
        Vector3 direction = (transform.position - other.transform.position).normalized;
        cc.ApplyKnockback(direction);
        if (pHealth != null)
        {
            pHealth.Decrement(hitBox.dmg);
        }
    }

    public void EnableHurtbox()
    {
        Debug.Log("Enabling hurtbox");
        gameObject.SetActive(true);
    }

    public void DisableHurtbox()
    {
        Debug.Log("Disabling hurtbox");
        gameObject.SetActive(false);
    }
}
