using UnityEngine;

public class OrientationFollow : MonoBehaviour
{

    public Transform follow;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = follow.position;
    }
}
