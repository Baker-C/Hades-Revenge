using UnityEngine;

public class NavigationScript : MonoBehaviour
{
    [SerializeField] Transform target;
    UnityEngine.AI.NavMeshAgent agent;
    void Start()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (target == null)
        {
            target = GameObject.FindWithTag("Player").transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        agent.destination = target.position;
    }
}
