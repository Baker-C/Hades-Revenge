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


//////////////////
/// <summary>
/// 
/// </summary>


    [Header("Settings")]
    [SerializeField] bool zeroVert_Look;

    float currentYOffset;
    Vector3 pos;

    DefMovement defMovement;
/// <summary>
/// 
/// </summary>
//////////////////



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pc = GetComponent<PlayerControl>();
    }

    void Awake()
    {
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
        targetDirection = target.position - transform.position;

        pc.AnimateMovement(movementX, movementZ);

        // Rotate player to face the target direction
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * rotationSpeed);
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(target.position, 1);
    }



    void CalculateVelocity()
    {
        float maxSpeed = sprintPressed ? sprintSpeed : walkSpeed;

        // Acceleration
        if (forwardPressed && movementZ < maxSpeed)
            movementZ += Time.fixedDeltaTime * acceleration;
        if (backPressed && movementZ > -maxSpeed)
            movementZ -= Time.fixedDeltaTime * acceleration;
        if (leftPressed && movementX > -maxSpeed)
            movementX -= Time.fixedDeltaTime * acceleration;
        if (rightPressed && movementX < maxSpeed)
            movementX += Time.fixedDeltaTime * acceleration;
        
        // Deceleration
        if (!forwardPressed && movementZ > 0.0f)
            movementZ -= Time.fixedDeltaTime * deceleration;
        if (!backPressed && movementZ < 0.0f)
            movementZ += Time.fixedDeltaTime * deceleration;
        if (!leftPressed && movementX < 0.0f)
            movementX += Time.fixedDeltaTime * deceleration;
        if (!rightPressed && movementX > 0.0f)
            movementX -= Time.fixedDeltaTime * deceleration;

        // Deceleration to walking
        if (movementZ > maxSpeed && movementZ > 0.0f)
            movementZ -= Time.fixedDeltaTime * deceleration;
        if (movementZ < -maxSpeed && movementZ < 0.0f)
            movementZ += Time.fixedDeltaTime * deceleration;
        if (movementX < -maxSpeed && movementX < 0.0f)
            movementX += Time.fixedDeltaTime * deceleration;
        if (movementX > maxSpeed && movementX > 0.0f)
            movementX -= Time.fixedDeltaTime * deceleration;

        // Clamp the velocity values to zero if they are close to zero
        if (Mathf.Abs(movementX) < 0.2f)
            movementX = 0.0f;
        if (Mathf.Abs(movementZ) < 0.2f)
            movementZ = 0.0f;
        
        Vector3 camForward = camera.forward;
        Vector3 camRight = camera.right;
        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        // Calculate final movement vector
        movement = camForward * movementZ + camRight * movementX;
        movement.y = 0f;
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
