using UnityEngine;

public class BasicEnemyAttack : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float attackRange = 2.0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (target == null)
            target = GameObject.FindWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(transform.position, target.position) < attackRange)
        {
            Debug.Log("Attack!");
        }
    }
}
