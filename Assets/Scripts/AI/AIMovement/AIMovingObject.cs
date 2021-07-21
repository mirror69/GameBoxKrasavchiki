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
    private AIPatrolStrategy patrolMovementStrategy;
    [SerializeField]
    private AIChasingStrategy chasingStrategy;
    [SerializeField]
    private AISearchingStrategy searchingStrategy;

    [SerializeField, HideInInspector]
    private int agentType = 0;
    [SerializeField, HideInInspector]
    private int agentTypeWalkingThroughWalls = 0;

    private NavMeshAgent navMeshAgent;

    private AIMovementStrategy currentMovementStrategy = null;

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

    public void StartPatrol()
    {
        navMeshAgent.agentTypeID = agentType;
        ChangeMovementStrategy(patrolMovementStrategy);
    }

    public void StartChasing(Transform target)
    {
        //navMeshAgent.agentTypeID = agentTypeWalkingThroughWalls;
        chasingStrategy.SetTarget(target);
        ChangeMovementStrategy(chasingStrategy);
    }
    public bool IsStopped()
    {
        return currentMovementStrategy.IsStopped();
    }

    public void StartSearching(Transform target)
    {
        searchingStrategy.SetTarget(target);
        ChangeMovementStrategy(searchingStrategy);
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
        if (searchingStrategy != null)
        {
            searchingStrategy.Initialize(this);
        }
        if (chasingStrategy != null)
        {
            chasingStrategy.Initialize(this);
        }
    }

    private void Start()
    {
        //if (patrolMovementStrategy != null)
        //{
        //    patrolMovementStrategy.StartMoving();
        //}        
    }

    private void ChangeMovementStrategy(AIMovementStrategy strategy)
    {
        if (strategy != null && currentMovementStrategy != strategy)
        {
            if (currentMovementStrategy != null)
            {
                currentMovementStrategy.StopMoving();
            }            
            currentMovementStrategy = strategy;
            currentMovementStrategy.StartMoving();
        }
    }

}
