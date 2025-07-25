using UnityEngine;

public class OrientationFollow : MonoBehaviour
{
    public PlayerControl pc;
    public Transform follow;

    void Update()
    {
        transform.position = follow.position;
        if (pc != null && PlayerState.IsLockedOn())
            transform.rotation = follow.rotation;
    }
}
