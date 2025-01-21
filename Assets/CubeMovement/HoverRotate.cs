using UnityEngine;

public class Rotate : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 5.0f;
    [SerializeField] private float hoverSpeed = 0.5f;
    [SerializeField] private float hoverAmplitude = 0.5f;

    private Transform cachedTransform;
    private Vector3 upVector;
    private Vector3 hoverVector;
    private float initialY;

    void Start()
    {
        cachedTransform = transform;
        upVector = Vector3.up;
        initialY = cachedTransform.position.y;
    }

    void Update()
    {
        cachedTransform.Rotate(upVector, rotationSpeed * Time.deltaTime);
        
        float newY = initialY + (Mathf.Sin(Time.time * hoverSpeed) * hoverAmplitude);
        cachedTransform.position = new Vector3(
            cachedTransform.position.x,
            newY,
            cachedTransform.position.z
        );
    }
}
