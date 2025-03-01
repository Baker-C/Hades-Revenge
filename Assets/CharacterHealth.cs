using UnityEngine;

public class CharacterHealth : MonoBehaviour
{
    protected float health = 100.0f;
    float maxHealth = 100.0f;

    protected CharacterControl cc;

    protected virtual void Start()
    {
        cc = GetComponent<CharacterControl>();
    }
    
    public virtual void Decrement(float dmg)
    {
        health -= dmg;
        Debug.Log("Health: " + health);
        if (health <= 0)
        {
            cc.Die();
        }
    }

    public virtual void Increment(float heals)
    {
        health += heals;
        Debug.Log("Health: " + health);
    }

    public virtual void ResetHealth()
    {
        health = maxHealth;
    }
}
