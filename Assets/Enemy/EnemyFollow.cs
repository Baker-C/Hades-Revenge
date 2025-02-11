using UnityEngine;
using System.Collections;

public class EnemyFollow : CharacterControl
{
    [SerializeField] private float defaultSpeed = 3.0f; // Default speed of the enemy
    [SerializeField] private float stoppingDistance = 2.0f; // Distance at which the enemy stops following the player
    [SerializeField] private float rotationSpeed = 5f; // Speed at which the enemy rotates towards the player
    [SerializeField] private float attackDelay = 2.0f; // Delay between attacks
    private GameObject playerObject; // Reference to the player GameObject
    private Vector3 direction; // Direction from the enemy to the player
    private float distance; // Distance from the enemy to the player
    private bool isAttacking; // Flag to check if the enemy is following the player
    private int VelocityXHash; // Hash for the Velocity X parameter in the Animator
    private int VelocityZHash; // Hash for the Velocity Z parameter in the Animator

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    override protected void Start()
    {
        playerObject = GameObject.FindWithTag("Player"); // Find the player GameObject by tag
        animator = GetComponent<Animator>(); // Get the Animator component

        // Cache animation parameter hashes
        VelocityXHash = Animator.StringToHash("Velocity X");
        VelocityZHash = Animator.StringToHash("Velocity Z");
    }

    // Update is called once per frame
    void Update()
    {
        FollowPlayer();
    }

    void FollowPlayer()
    {
        direction = playerObject.transform.position - transform.position;

        // Rotate towards player
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        if (isAttacking == true)
        {
            return;
        }

        // Calculate the distance from the enemy to the player
        distance = Vector3.Magnitude(direction);

        // If the enemy is within the stopping distance, stop following the player
        if (distance <= stoppingDistance)
        {
            // Stop movement
            animator.SetFloat(VelocityXHash, 0);
            animator.SetFloat(VelocityZHash, 0);
            BeginAttacking();
            return;
        }

        direction.Normalize();

        // Set forward movement in animator
        animator.SetFloat(VelocityZHash, defaultSpeed);
        animator.SetFloat(VelocityXHash, 0);
    }

    void BeginAttacking()
    {
        animator.SetFloat(VelocityZHash, 0);
        animator.SetFloat(VelocityXHash, 0);

        StartCoroutine(Attack());
    }

    IEnumerator Attack()
    {
        isAttacking = true;
        animator.Play("Light Punch", 0, 0.0f);
        yield return new WaitForSeconds(GetAnimationLength("Light Punch") + attackDelay); // Wait for 2 seconds
        isAttacking = false;
    }
}
