using UnityEngine;
using Unity.Cinemachine;

public class LockedMovement : MonoBehaviour
{

    [Header("References")]
    [SerializeField] Transform orientation;
    [SerializeField] Rigidbody rb;
    [SerializeField] Transform camera;
    [SerializeField] CinemachineCamera lockOnCam;
    [SerializeField] Transform target;
    Vector3 targetDirection;


    [Header("Movement")]
    [SerializeField] float sprintSpeed;
    [SerializeField] float walkSpeed;
    [SerializeField] float rotationSpeed;
    [SerializeField] float acceleration;
    [SerializeField] float deceleration;
    [SerializeField] float leftOffset;
    [SerializeField] float rightOffset;

    [Header("LockOn")]
    [Tooltip("Angle_Degree")] [SerializeField] float maxNoticeAngle = 60;
    [SerializeField] float noticeZone = 10;
    [SerializeField] LayerMask targetLayers;
    [SerializeField] Transform enemyTarget_Locator;
    [SerializeField] float crossHair_Scale = 5f;

    bool lockedOn;
    float movementX = 0.0f;
    float movementZ = 0.0f;
    Vector3 movement;

    bool forwardPressed;
    bool backPressed;
    bool leftPressed;
    bool rightPressed;
    bool sprintPressed;

    PlayerControl pc;

    [Header("Settings")]
    [SerializeField] bool zeroVert_Look;
    float currentYOffset;

    void Awake()
    {
        pc = GetComponent<PlayerControl>();
        CalculateTarget();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (PlayerState.IsBusy())
            return;
            
        if (target == null)
        {
            pc.ToggleLockedOn();
            return;
        }

        // Cam lock on to target
        lockOnCam.LookAt = target;

        forwardPressed = Input.GetKey("w");
        backPressed = Input.GetKey("s");
        leftPressed = Input.GetKey("a");
        rightPressed = Input.GetKey("d");
        sprintPressed = Input.GetKey("left shift");

        // Move Player
        CalculateVelocity();
        transform.position += movement * Time.fixedDeltaTime;        

        CalculateRotation();

        pc.AnimateMovement(0f, Mathf.Abs(movement.magnitude) * 2 / sprintSpeed);
    }

    void CalculateRotation()
    {
        targetDirection = target.position - transform.position;

        // Base rotation towards target
        Quaternion baseRotation = Quaternion.LookRotation(targetDirection);
        Quaternion targetRotation = baseRotation;
        
        // Apply offset based on movement
        if (leftPressed && !rightPressed)
            targetRotation = baseRotation * Quaternion.Euler(0, leftOffset, 0);
        else if (rightPressed && !leftPressed)
            targetRotation = baseRotation * Quaternion.Euler(0, rightOffset, 0);

        float angleDiff = Quaternion.Angle(transform.rotation, targetRotation);
        if (angleDiff < 10f)
            transform.rotation = targetRotation;
        else
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * rotationSpeed);
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(target.position, 1);
    }

    void CalculateVelocity()
    {
        float maxSpeed = sprintPressed ? sprintSpeed : walkSpeed;

        Vector2 inputDir = new Vector2(
            (rightPressed ? 1 : 0) - (leftPressed ? 1 : 0),
            (forwardPressed ? 1 : 0) - (backPressed ? 1 : 0)
        ).normalized;

        if (inputDir.x != 0)
            movementX = Mathf.MoveTowards(movementX, inputDir.x * maxSpeed, Time.fixedDeltaTime * acceleration);
        if (inputDir.y != 0)
            movementZ = Mathf.MoveTowards(movementZ, inputDir.y * maxSpeed, Time.fixedDeltaTime * acceleration);

        if (inputDir.x == 0)
            movementX = Mathf.MoveTowards(movementX, 0, Time.fixedDeltaTime * deceleration);
        if (inputDir.y == 0)
            movementZ = Mathf.MoveTowards(movementZ, 0, Time.fixedDeltaTime * deceleration);

        // Convert to camera relative movement
        Vector3 camForward = camera.forward;
        Vector3 camRight = camera.right;
        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        movement = (camForward * movementZ + camRight * movementX);
    }

    void CalculateTarget()
    {
        Collider[] nearbyTargets = Physics.OverlapSphere(transform.position, noticeZone, targetLayers);
        float closestAngle = maxNoticeAngle;
        target = null;
        if (nearbyTargets.Length <= 0)
        {
            pc.ToggleLockedOn();
            return;
        }

        for (int i = 0; i < nearbyTargets.Length; i++)
        {
            Debug.Log("NearbyTarget: " + nearbyTargets[i].name);
            Vector3 dir = nearbyTargets[i].transform.position - camera.position;
            dir.y = 0;
            float _angle = Vector3.Angle(camera.forward, dir);
            
            if (_angle < closestAngle)
            {
                target = nearbyTargets[i].transform;
                closestAngle = _angle;      
            }
        }

        if (!target ) 
        {
            pc.ToggleLockedOn();
            return;
        }

        // Calculate lockon height
        float h1 = target.GetComponent<CapsuleCollider>().height;
        float h2 = target.localScale.y;
        float h = h1 * h2;
        float half_h = (h / 2) / 2;
        currentYOffset = h - half_h;
        if(zeroVert_Look && currentYOffset > 1.6f && currentYOffset < 1.6f * 3) currentYOffset = 1.6f;
        
        Vector3 tarPos = target.position + new Vector3(0, currentYOffset, 0);
        if(Blocked(tarPos))
        {
            pc.ToggleLockedOn();
            return;
        }
        tarPos.y = h + 0.2f;

        enemyTarget_Locator.position = tarPos;
        lockOnCam.LookAt = enemyTarget_Locator;

    }


    bool Blocked(Vector3 t){
        RaycastHit hit;
        if(Physics.Linecast(transform.position + Vector3.up * 0.5f, t, out hit)){
            if(!hit.transform.CompareTag("Enemy")) return true;
        }
        return false;
    }

}
