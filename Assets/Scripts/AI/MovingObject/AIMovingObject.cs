using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class AIMovingObject : MovingObject
{
    [SerializeField]
    private float fovRotationSpeed = 20;

    private NavMeshAgent navMeshAgent;
    private Coroutine movingCoroutine;

    private Vector3 currentEndDestinationPoint;

    public float FovRotationSpeed => fovRotationSpeed;

    public override void MoveToPoint(Vector3 point)
    {
        Stop();
        navMeshAgent.destination = point;
        currentEndDestinationPoint = navMeshAgent.destination;
        movingCoroutine = StartCoroutine(PerformMovingToPoint(point));
    }
    public override void Rotate(float rotation)
    {
        transform.Rotate(new Vector3(0, rotation, 0));
    }

    public void Stop()
    {
        navMeshAgent.destination = Position;
        this.StopAndNullCoroutine(ref movingCoroutine);
    }

    public void SetEnabledAutomaticRotation(bool enabled)
    {
        navMeshAgent.updateRotation = enabled;
    }

    public bool IsDestinationReached()
    {
        return movingCoroutine == null;
    }

    public bool IsDestinationInStoppingDistance()
    {
        return navMeshAgent.destination == currentEndDestinationPoint &&
            navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance;
    }

    public override Vector3 GetCurrentVelocity()
    {
        return navMeshAgent.velocity;
    }

    public void SetEnabled(bool enabled)
    {
        navMeshAgent.enabled = enabled;
    }

    private bool IsNavMeshDestinationReached()
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
    public float GetNavMeshRemainingDistance()
    {
        return navMeshAgent.remainingDistance;
    }

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.speed = speed;
        navMeshAgent.angularSpeed = rotationSpeed;
        currentEndDestinationPoint = navMeshAgent.destination;
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
            IObstacle obstacle = null;
            if (hitInfo.collider != null)
            {
                obstacle = hitInfo.collider.GetComponentInParent<IObstacle>();
            }

            if (hitInfo.collider == null || obstacle == null)
            {
                // ???? ???????? ?? ???????????, ???? ??? ???????? ?? ?????
                navMeshAgent.destination = finishTargetPoint;
                while (!IsNavMeshDestinationReached())
                {
                    yield return new WaitForSeconds(0.1f);
                }
                break;
            }

            if (!obstacle.IsPassable())
            {
                // ????? ???????????? - ???? ??? ????????, ?? ?????? ????????? ???????,
                // ????? ?????????, ?? ????? ?? ??? ????????????
                navMeshAgent.destination = finishTargetPoint;
                yield return new WaitForSeconds(0.1f);

                isDestinationReached = IsNavMeshDestinationReached();
                continue;
            }

            // ????? ?????????? - ???? ? ???
            Vector3 moveDirection = finishTargetPoint - Position;
            moveDirection.y = 0;
            moveDirection = moveDirection.normalized;

            navMeshAgent.destination = hitInfo.point - navMeshAgent.radius * moveDirection;
            
            yield return new WaitForSeconds(0.1f);
            
            if (!IsNavMeshDestinationReached())
            {
                continue;
            }

            // ????? ?? ?????
            // ??????? ?????, ??? ???????????? ?????

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

            // ??????? ?? ?????
            navMeshAgent.Warp(pointToJump + moveDirection);
        }
        movingCoroutine = null;
    }

}
