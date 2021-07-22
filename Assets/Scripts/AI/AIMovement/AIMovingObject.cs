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

    [SerializeField, HideInInspector]
    private int agentType = 0;
    [SerializeField, HideInInspector]
    private int agentTypeWalkingThroughWalls = 0;

    private NavMeshAgent navMeshAgent;
    private Coroutine movingCoroutine;

    public float Speed => speed;
    public float RotationSpeed => rotationSpeed;
    public float FovRotationSpeed => fovRotationSpeed;
    public Vector3 Position => transform.position;

    public void Move(Vector3 point)
    {
        this.StopAndNullCoroutine(ref movingCoroutine);
        movingCoroutine = StartCoroutine(PerformMovingToPoint(point));
    }

    public void Rotate(float rotation)
    {
        transform.Rotate(new Vector3(0, rotation, 0));
    }

    public void SetEnabledAutomaticRotation(bool enabled)
    {
        navMeshAgent.updateRotation = enabled;
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
    }

    private RaycastHit RaycastByShortestWay(Vector3 targetPoint)
    {
        Vector3 vectorToTarget = targetPoint - Position;

        Physics.Raycast(Position, vectorToTarget, out RaycastHit hitInfo,
            vectorToTarget.magnitude, GameManager.Instance.ObstacleLayers);

        return hitInfo;
    }

    private IEnumerator PerformMovingToPoint(Vector3 finishTargetPoint)
    {        
        bool isDestinationReached = false;
        while (!isDestinationReached)
        {
            RaycastHit hitInfo = RaycastByShortestWay(finishTargetPoint);
            bool tryMoveThoughWall = false;
            if (hitInfo.collider != null)
            {
                IObstacle obstacle = hitInfo.collider.GetComponentInParent<IObstacle>();
                tryMoveThoughWall = obstacle != null && obstacle.IsPassable();
            }

            if (!tryMoveThoughWall)
            {
                // Путь свободен или стена непрозрачная - идем как обычно
                navMeshAgent.destination = finishTargetPoint;
                yield return new WaitForSeconds(0.1f);

                isDestinationReached = IsDestinationReached();
                continue;
            }

            // Стена прозрачная - идём к ней
            Vector3 moveDirection = finishTargetPoint - Position;
            moveDirection.y = 0;
            moveDirection = moveDirection.normalized;

            navMeshAgent.destination = hitInfo.point - navMeshAgent.radius * moveDirection;

            if (!IsDestinationReached() && GetRemainingDistance() > 0.5f)
            {
                yield return new WaitForSeconds(0.1f);
                continue;
            }

            // Дошли до стены
            // Находим точку, где оканчивается стена

            bool isPointToJumpFound = false;
            Vector3 pointToJump = Position;
            while (!isPointToJumpFound)
            {
                pointToJump += moveDirection;
                Vector3 topPoint = pointToJump;
                topPoint.y = float.MaxValue;
                isPointToJumpFound = !Physics.Raycast(topPoint, Vector3.down, out hitInfo,
                    float.MaxValue, GameManager.Instance.ObstacleLayers);
            }

            // Прыгаем за стену
            navMeshAgent.Warp(pointToJump);
        }

    }

}
