using UnityEngine;
using UnityEngine.AI;
using System.Collections;


public class GiantAttack : MonoBehaviour
{
    public Transform player;
    public float attackDistance = 10f;
    public float jumpDistanceMin = 5f;
    public float jumpDistanceMax = 8f;
    public float jumpTimeInAir = 1f;
    public float jumpCooldown = 2f;
    public float jumpHeight = 2f;
    public float timeBetweenJumps = 10f;
    public NavMeshAgent agent;
    public float cooldown = 2f;
    public Animator animator;
    public LayerMask lineOfSightMask;
    public float rotationSpeed = 5f;
    public CapsuleCollider bc;
    public Rigidbody rb;

    private float nextJumpTime = 0f;
    private bool isAttacking = false;
    private int attackingHash;
    private int stompHash;
    private int punchHash;
    private int jumpHash;
    private int speedHash;
    private int deadHash;

    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        bc = GetComponent<CapsuleCollider>();
        rb = GetComponent<Rigidbody>();

        attackingHash = Animator.StringToHash("Attacking");
        stompHash = Animator.StringToHash("Stomp");
        punchHash = Animator.StringToHash("Punch");
        jumpHash = Animator.StringToHash("Jump");
        speedHash = Animator.StringToHash("Speed");
        deadHash = Animator.StringToHash("Dead");
    }

    void Update()
    {
        if (animator.GetBool(deadHash))
        {
            rb.angularVelocity = Vector3.zero;
            return;
        }
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackDistance && !isAttacking)
        {
            Attack();
        }
        else if (distanceToPlayer >= jumpDistanceMin && distanceToPlayer <= jumpDistanceMax && !isAttacking && Time.time > nextJumpTime)
        {
            if (HasLineOfSight())
            {
                StartCoroutine(RotateTowardsPlayer(player.position));
                Jump();
            }
        }

        animator.SetFloat(speedHash, agent.velocity.magnitude / agent.speed);
    }

    void Attack()
    {
        isAttacking = true;
        agent.isStopped = true;

        Vector3 directionToPlayer = (player.position - transform.position);
        directionToPlayer.y = transform.position.y;
        directionToPlayer.Normalize();
        StartCoroutine(RotateTowardsPlayer(directionToPlayer));
        StartCoroutine(AttackRoutine());
    }

    IEnumerator AttackRoutine()
    {
        yield return new WaitForSeconds(0.5f);

        Vector3 relativePosition = transform.InverseTransformPoint(player.position);
        bool isPlayerOnLeft = relativePosition.x < 0;

        if (isPlayerOnLeft)
            animator.SetBool("Stomp", true);
        else
            animator.SetBool("Punch", true);

        StartCoroutine(Cooldown(isPlayerOnLeft));
    }

    void Jump()
    {
        Debug.Log("Jumping");
        isAttacking = true;
        agent.isStopped = true;

        animator.SetBool(jumpHash, true);
        StartCoroutine(JumpRoutine());
    }

    IEnumerator JumpRoutine()
    {
        Vector3 startPos = transform.position;
        Vector3 endPos = player.position;
        float elapsedTime = 0f;

        bc.height = bc.height / 2;

        while (elapsedTime < jumpTimeInAir)
        {
            float t = elapsedTime / jumpTimeInAir;
            transform.position = Vector3.Lerp(startPos, endPos, t) + Vector3.up * Mathf.Sin(t * Mathf.PI) * jumpHeight;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = endPos;
        animator.SetBool(jumpHash, false);
        StartCoroutine(JumpCooldown());
    }

    IEnumerator JumpCooldown()
    {
        rb.angularVelocity = Vector3.zero;
        yield return new WaitForSeconds(0.3f);
        bc.height = bc.height * 2;
        yield return new WaitForSeconds(jumpCooldown);
        isAttacking = false;
        agent.isStopped = false;
        yield return new WaitForSeconds(0.5f);
        nextJumpTime = Time.time + timeBetweenJumps;
    }

    IEnumerator RotateTowardsPlayer(Vector3 targetDirection)
    {
        while (Vector3.Angle(transform.forward, targetDirection) > 5f && !isAttacking)
        {
            Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, rotationSpeed * Time.deltaTime, 0.0f);
            transform.rotation = Quaternion.LookRotation(newDirection);
            yield return null;
        }
    }

    IEnumerator Cooldown(bool isPlayerOnLeft)
    {
        yield return new WaitForSeconds(cooldown);
        if (isPlayerOnLeft)
            animator.SetBool("Stomp", false);
        else
            animator.SetBool("Punch", false);
        agent.isStopped = false;
        yield return new WaitForSeconds(0.5f);
        isAttacking = false;
    }

    bool HasLineOfSight()
    {
        Vector3 currentPosition = transform.position;
        currentPosition.y += 0.5f;
        
        Vector3 directionToPlayer = (player.position - transform.position);
        directionToPlayer.y = 0;
        directionToPlayer.y += 0.5f;
        directionToPlayer.Normalize();
        
        Debug.DrawRay(currentPosition, directionToPlayer * jumpDistanceMax, Color.red, 1.0f);

        if (Physics.Raycast(transform.position, directionToPlayer, out RaycastHit hit, jumpDistanceMax, lineOfSightMask))
        {
            return hit.transform == player;
        }
        return false;
    }
}