using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class AIMovingObject : MonoBehaviour
{
    [SerializeField] 
    private float speed = 4;
    [SerializeField] 
    private float rotationSpeed = 120;
    [SerializeField]
    private float fovRotationSpeed = 20;
    [SerializeField]
    private AIMovementStrategy patrolMovementStrategy;

    private NavMeshAgent navMeshAgent;
    public AIMovementStrategy PatrolMovementStrategy => patrolMovementStrategy;

    public float Speed => speed;
    public float RotationSpeed => rotationSpeed;
    public float FovRotationSpeed => fovRotationSpeed;
    public Vector3 Position => transform.position;

    public void Move(Vector3 point)
    {
        navMeshAgent.destination = point;
    }
    public void Rotate(float rotation)
    {
        navMeshAgent.transform.Rotate(new Vector3(0, rotation, 0));
    }
    public bool IsDestinationReached()
    {
        if (!navMeshAgent.pathPending)
        {
            if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
            {
                if (!navMeshAgent.hasPath || navMeshAgent.velocity.sqrMagnitude == 0f)
                {
                    return true;
                }
            }
        }

        return false;
    }
    public float GetRemainingDistance()
    {
        return navMeshAgent.remainingDistance;
    }

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.speed = speed;
        navMeshAgent.angularSpeed = rotationSpeed;

        if (patrolMovementStrategy != null)
        {
            patrolMovementStrategy.Initialize(this);
        }
    }

    private void Start()
    {
        if (patrolMovementStrategy != null)
        {
            patrolMovementStrategy.StartMoving();
        }        
    }

}
