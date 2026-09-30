using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyLR : MonoBehaviour
{
    [SerializeField] private Transform leftPoint;
    [SerializeField] private Transform rightPoint;

    private NavMeshAgent agent;
    private Transform target;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        agent.updateRotation = false;
    }

    private void Start()
    {
        leftPoint.SetParent(null, true);
        rightPoint.SetParent(null, true);

        target = rightPoint;
        agent.SetDestination(target.position);
    }

    private void Update()
    {
        if (agent.pathPending) return;

        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            target = target == leftPoint ? rightPoint : leftPoint;
            agent.SetDestination(target.position);
        }
    }
}
