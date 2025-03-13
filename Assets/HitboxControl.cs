using UnityEngine;

public class HitboxControl : MonoBehaviour
{
    [SerializeField] Hitbox fullBodyHitbox;
    [SerializeField] Hitbox leftHandHitbox;
    [SerializeField] Hitbox rightHandHitbox;
    [SerializeField] Hitbox leftFootHitbox;
    [SerializeField] Hitbox rightFootHitbox;

    public void EnableFullBodyHitbox()
    {
        fullBodyHitbox.enabled = true;
    }

    public void DisableFullBodyHitbox()
    {
        fullBodyHitbox.DisableHitbox();
    }

    public void EnableLeftHandHitbox()
    {
        leftHandHitbox.enabled = true;
    }

    public void DisableLeftHandHitbox()
    {
        leftHandHitbox.DisableHitbox();
    }

    public void EnableRightHandHitbox()
    {
        rightHandHitbox.enabled = true;
    }

    public void DisableRightHandHitbox()
    {
        rightHandHitbox.DisableHitbox();
    }

    public void EnableLeftFootHitbox()
    {
        leftFootHitbox.enabled = true;
    }

    public void DisableLeftFootHitbox()
    {
        leftFootHitbox.DisableHitbox();
    }

    public void EnableRightFootHitbox()
    {
        rightFootHitbox.enabled = true;
    }

    public void DisableRightFootHitbox()
    {
        rightFootHitbox.DisableHitbox();
    }

}
