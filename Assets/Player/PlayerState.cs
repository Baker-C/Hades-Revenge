using UnityEngine;
using System.Collections;

public class PlayerState : MonoBehaviour
{
    [SerializeField] float fHealth = 100.0f;
    
    float health; 
    bool busy;

    static PlayerState _instance;

    public static PlayerState Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<PlayerState>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("PlayerState");
                    _instance = go.AddComponent<PlayerState>();
                }
            }
            return _instance;
        }
    }  

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }

        health = fHealth;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static bool IsBusy()
    {
        return _instance.busy;
    }

    public static void MakeBusy(float time)
    {
        _instance.StartCoroutine(_instance.BusyRoutine(time));
    }

    private IEnumerator BusyRoutine(float time)
    {
        busy = true;
        yield return new WaitForSeconds(time);
        busy = false;
    }

    public static float GetFullHealth()
    {
        return _instance.fHealth;
    }

    public static float GetCurrentHealth()
    {
        return _instance.health;
    }

    public static void IncrementHealth(float heals)
    {
        _instance.health += heals;
    }

    public static void DecrementHealth(float dmg)
    {
        _instance.health -= dmg;
    }
}

