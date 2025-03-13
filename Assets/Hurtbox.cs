using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Hurtbox : MonoBehaviour
{
    public CharacterHealth cHealth;
    public CharacterControl cc;


    void Start()
    {
        cHealth = transform.GetComponentInParent<CharacterHealth>();

        cc = transform.GetComponentInParent<PlayerControl>();
        if (cc == null)
            cc = transform.GetComponentInParent<CharacterControl>();
    }

    public void RegisterHit(float dmg, float knockback, Vector3 direction)
    {
        cc.ApplyKnockback(knockback, direction);
        cHealth.Decrement(dmg);
    }

    public void EnableHurtbox()
    {
        this.enabled = true;
    }

    public void DisableHurtbox()
    {
        this.enabled = false;
    }
}
