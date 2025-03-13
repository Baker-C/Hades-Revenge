using UnityEngine;
using Unity.Cinemachine;
using System.Collections;

public class LockedMovement : MonoBehaviour
{

    [Header("References")]
    [SerializeField] Transform orientation;
    [SerializeField] Rigidbody rb;
    [SerializeField] Transform cam;
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
    [SerializeField] Transform targetLockOnLocator;
    [Tooltip("Angle_Degree")] [SerializeField] float maxNoticeAngle = 60;
    [SerializeField] float noticeZone = 10;
    [SerializeField] LayerMask targetLayers;
    [SerializeField] float crossHair_Scale = 5f;


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


    void OnEnable()
    {
        if (!pc) pc = GetComponent<PlayerControl>();
        CalculateTarget();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        CalculateRotation();

        if (target == null || Vector3.Distance(transform.position, target.position) > noticeZone)
        {
            pc.ToggleLockedOn();
            return;
        }

        DrawLockOnIndicator();
        
        if (PlayerState.IsBusy() || pc.dashCD)
            return;

        forwardPressed = Input.GetKey("w");
        backPressed = Input.GetKey("s");
        leftPressed = Input.GetKey("a");
        rightPressed = Input.GetKey("d");
        sprintPressed = Input.GetKey("left shift");

        // Move Player
        CalculateVelocity();

        transform.position += movement * Time.fixedDeltaTime;        

        pc.AnimateMovement(movementX * 2 / sprintSpeed, movement.z * 2 / sprintSpeed);
    }

    void CalculateRotation()
    {
        if (target == null)
            return;

        targetDirection = target.position - transform.position;
        targetDirection.y = transform.position.y;

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
        if (PlayerState.IsBusy())
            Gizmos.DrawWireSphere(transform.position, 1);
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

        // Convert to cam relative movement
        Vector3 camForward = cam.forward;
        Vector3 camRight = cam.right;
        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        movement = (camForward * movementZ + camRight * movementX);
    }



    void CalculateTarget()
    {
        Collider[] nearbyTargets = Physics.OverlapSphere(transform.position, noticeZone, targetLayers);
        Debug.Log("Nearby Targets: " + nearbyTargets.Length);
        if (nearbyTargets.Length <= 0)
        {
            pc.ToggleLockedOn();
            return;
        }

        for (int i = 0; i < nearbyTargets.Length; i++)
        {
            Debug.Log($"Nearby target {i+1}: {nearbyTargets[i].name}");
        }

        target = null;
        float closestAngle = maxNoticeAngle;
        for (int i = 0; i < nearbyTargets.Length; i++)
        {
            Vector3 dir = nearbyTargets[i].transform.position - cam.position;
            dir.y = 0;
            float _angle = Vector3.Angle(cam.forward, dir);
            
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
    }

    void DrawLockOnIndicator()
    {
        // Calculate lockon height
        float h1 = target.GetComponent<CapsuleCollider>().height;
        float h2 = target.localScale.y;
        float h = h1 * h2;
        float half_h = (h / 2);
        currentYOffset = h - half_h;
        
        target.position += Vector3.up * currentYOffset;
        if(Blocked(target.position))
        {
            targetLockOnLocator.position = Vector3.up * 1000;
            pc.ToggleLockedOn();
            return;
        }
        lockOnCam.LookAt = target;

        targetLockOnLocator.position = target.position - Vector3.up * currentYOffset;

        float scale = crossHair_Scale * Mathf.Sqrt(Vector3.Distance(transform.position, target.position));
        targetLockOnLocator.localScale = new Vector3(scale, scale, scale);

    }


    bool Blocked(Vector3 t){
        RaycastHit hit;
        if(Physics.Linecast(transform.position + Vector3.up * 0.5f, t, out hit)){
            if(!hit.transform.CompareTag("Enemy")) return true;
        }
        return false;
    }
}
