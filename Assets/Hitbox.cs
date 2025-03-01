using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Hitbox : MonoBehaviour
{

    public float dmg = 20.0f;
    public Vector3 knockback = new Vector3(100,0,0);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnTriggerEnter(Collider other)
    {
        Hurtbox hurtBox = other.GetComponent<Hurtbox>();
        Debug.Log("Hitbox collided with " + hurtBox);
        
    }

    public void EnableHitbox()
    {
        Debug.Log("Enabling hitbox");
        gameObject.SetActive(true);
        gameObject.GetComponent<Collider>().enabled = true;
    }

    public void DisableHitbox()
    {
        Debug.Log("Disabling hitbox");
        gameObject.SetActive(false);
        gameObject.GetComponent<Collider>().enabled = false;
    }
    
}
