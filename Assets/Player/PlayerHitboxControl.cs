using UnityEngine;

public class PlayerHitboxControl : MonoBehaviour
{

    [SerializeField] Hitbox leftHandHitbox;
    [SerializeField] Hitbox rightFootHitbox;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EnableLeftHandHitbox()
    {
        leftHandHitbox.EnableHitbox();
    }

    public void DisableLeftHandHitbox()
    {
        leftHandHitbox.DisableHitbox();
    }

    public void EnableRightFootHitbox()
    {
        rightFootHitbox.EnableHitbox();
    }

    public void DisableRightFootHitbox()
    {
        rightFootHitbox.DisableHitbox();
    }

}
