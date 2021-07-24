using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Состояние патрулирования
/// </summary>
[RequireComponent(typeof(AIPatrolStrategy))]
public class PatrolState : FsmState
{
    private AIPatrolStrategy patrolStrategy;

    private AIMovingObject movingObject;
    private FieldOfView fov;

    public AIPatrolStrategy PatrolStrategy => patrolStrategy;

    public override void OnStateEnter()
    {
        Debug.Log($"{movingObject.name} starting patrol");
        fov.SetIdleDetectionDelay();
        patrolStrategy.StartMoving();
    }

    public override void OnStateLeave()
    {
        patrolStrategy.StopMoving();
    }

    private void Awake()
    {
        movingObject = GetComponentInParent<AIMovingObject>();
        fov = GetComponentInParent<FieldOfView>();

        patrolStrategy = GetComponent<AIPatrolStrategy>();
        patrolStrategy.Initialize(movingObject);
    }
}
