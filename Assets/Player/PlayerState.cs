using UnityEngine;
using System.Collections;

public enum ActionState
{
    None,
    Busy,
    Attacking,
    Dashing,
    Jumping,
    Dead
}

public enum MovementState
{
    None,
    Locked,
    Unlocked
}

public class PlayerState : MonoBehaviour
{
    [SerializeField] float fHealth = 100.0f;
    [SerializeField] Vector3 spawnPosition = new Vector3(0, 0.1f, 0);


    ActionState currentActionState = ActionState.None;
    MovementState currentMovementState = MovementState.None;

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

    void Start()
    {
        Instance.currentMovementState = MovementState.Unlocked;        
    }

    public static Vector3 GetSpawnPosition()
    {
        return Instance.spawnPosition;
    }

    public static void SetSpawnPosition(Vector3 position)
    {
        Instance.spawnPosition = position;
    }

    public static bool IsBusy()
    {
        return Instance.currentActionState != ActionState.None;
    }

    public static void MakeBusy(ActionState actionState = ActionState.Busy)
    {
        Instance.currentActionState = actionState;
    }

    public static void FreeActionState()
    {
        if (Instance.currentActionState == ActionState.Dead)
            return;
        Instance.currentActionState = ActionState.None;
    }

    public static void MakeBusyForTime(float time, ActionState actionState = ActionState.Busy)
    {
        Instance.StartCoroutine(Instance.BusyRoutine(time, actionState));
    }

    private IEnumerator BusyRoutine(float time, ActionState actionState)
    {
        Instance.currentActionState = actionState;
        yield return new WaitForSeconds(time);
        FreeActionState();
        yield return null;
    }

    public static void UnlockMovement()
    {
        Instance.currentMovementState = MovementState.Unlocked;
    }

    public static void LockMovement()
    {
        Instance.currentMovementState = MovementState.Locked;
    }

    public static bool IsLockedOn()
    {
        return Instance.currentMovementState == MovementState.Locked;
    }

    public static bool IsDashing()
    {
        return Instance.currentActionState == ActionState.Dashing;
    }

    public static bool IsJumping()
    {
        return Instance.currentActionState == ActionState.Jumping;
    }

    public static bool IsAttacking()
    {
        return Instance.currentActionState == ActionState.Attacking;
    }

    public static void Respawn(GameObject go)
    {
        Instance.StartCoroutine(Instance.RespawnCoroutine(go));
    }

    private IEnumerator RespawnCoroutine(GameObject go)
    {
        yield return new WaitForSeconds(3.0f);
        Instance.currentActionState = ActionState.None;
        Debug.Log("Character Spawned");
        go.SetActive(true);
        go.transform.position = spawnPosition;
        go.GetComponent<CharacterHealth>().ResetHealth();
    }
}

