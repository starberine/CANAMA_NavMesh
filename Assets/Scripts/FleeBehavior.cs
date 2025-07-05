using UnityEngine;
using UnityEngine.AI;

public class FleeBehavior : MonoBehaviour
{
    private Transform target; // dynamically found player
    private NavMeshAgent agent;

    [Header("Flee Settings")]
    public float fleeDistance = 5f;
    public float fleeMultiplier = 10f;

    public static bool isFleeing;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // Try to find the player at runtime
        var player = FindObjectOfType<NavmeshPlayerController>();
        if (player != null)
        {
            target = player.transform;
        }
        else
        {
            Debug.LogWarning("FleeBehavior: No NavmeshPlayerController found in scene.");
        }
    }

    void Update()
    {
        if (target == null) return;

        if (IsTargetClose())
        {
            FleeFromTheTarget();
        }
    }

    private bool IsTargetClose()
    {
        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        isFleeing = distanceToTarget <= fleeDistance;
        return isFleeing;
    }

    private void FleeFromTheTarget()
    {
        Vector3 fleeDirection = transform.position - target.position;
        Vector3 fleePosition = transform.position + fleeDirection.normalized * fleeMultiplier;

        if (NavMesh.SamplePosition(fleePosition, out NavMeshHit navHit, 10f, NavMesh.AllAreas))
        {
            agent.SetDestination(navHit.position);
        }
    }
}
