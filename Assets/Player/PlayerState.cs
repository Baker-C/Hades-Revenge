using UnityEngine;
using System.Collections;

public class PlayerState : MonoBehaviour
{
    [SerializeField] float fHealth = 100.0f;
    [SerializeField] Vector3 spawnPosition = new Vector3(0, 0.1f, 0);
    
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
    }

    public static Vector3 GetSpawnPosition()
    {
        return _instance.spawnPosition;
    }

    public static void SetSpawnPosition(Vector3 position)
    {
        _instance.spawnPosition = position;
    }

    public static bool IsBusy()
    {
        return _instance.busy;
    }

    public static void MakeBusy()
    {
        _instance.busy = true;
    }

    public static void Free()
    {
        _instance.busy = false;
    }

    public static void MakeBusyForTime(float time)
    {
        _instance.StartCoroutine(_instance.BusyRoutine(time));
    }

    private IEnumerator BusyRoutine(float time)
    {
        busy = true;
        yield return new WaitForSeconds(time);
        busy = false;
    }

    public static void Respawn(GameObject go)
    {
        _instance.StartCoroutine(_instance.RespawnCoroutine(go));
    }

    private IEnumerator RespawnCoroutine(GameObject go)
    {
        yield return new WaitForSeconds(3.0f);
        Debug.Log("Character Spawned");
        go.SetActive(true);
        go.transform.position = spawnPosition;
        go.GetComponent<CharacterHealth>().ResetHealth();
    }
}

