using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    PlayerState ps;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ps = FindFirstObjectByType<PlayerState>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Decrement(float dmg)
    {
        PlayerState.DecrementHealth(dmg);
        Debug.Log("Health: " + PlayerState.GetCurrentHealth());
    }

    public void Increment(float heals)
    {
        PlayerState.IncrementHealth(heals);
        Debug.Log("Health: " + PlayerState.GetCurrentHealth());
    }
}
