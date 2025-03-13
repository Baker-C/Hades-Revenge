using UnityEngine;

public class CharacterHealth : MonoBehaviour
{
    public Vector3 respawnPoint = new Vector3(0, 0.1f, 0);
    protected float health = 100.0f;
    public float maxHealth = 100.0f;

    protected CharacterControl cc;

    protected virtual void Start()
    {
        cc = GetComponent<PlayerControl>();
        if (cc == null)
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

    public virtual void SetRespawnPoint(Vector3 point)
    {
        respawnPoint = point;
    }

    public virtual void Respawn()
    {
        transform.position = respawnPoint;
        ResetHealth();
    }

    public virtual void SetHealth(float newHealth)
    {
        health = newHealth;
    }

    public virtual float GetHealth()
    {
        return health;
    }

    public virtual float GetMaxHealth()
    {
        return maxHealth;
    }

    public virtual void SetMaxHealth(float newMaxHealth)
    {
        maxHealth = newMaxHealth;
    }
}
