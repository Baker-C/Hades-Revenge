using UnityEngine;

public class LockOn : MonoBehaviour
{
    [SerializeField] Transform target;

    void OnEnable()
    {
        if (target == null)
            target = null;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
