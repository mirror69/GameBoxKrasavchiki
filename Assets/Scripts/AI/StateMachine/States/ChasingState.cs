using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Состояние погони за игроком
/// </summary>
[RequireComponent(typeof(AIChasingStrategy))]
public class ChasingState : FsmState
{
    private AIMovingObject movingObject;
    private AIChasingStrategy chasingStrategy;
    private FieldOfView fov;

    public override void OnStateEnter()
    {
        Debug.Log($"{movingObject.name} starting chasing");
        fov.SetAlertDetectionDelay();

        chasingStrategy.Initialize(movingObject, GameManager.Instance.Player.transform);
        chasingStrategy.StartMoving();
    }

    public override void OnStateLeave()
    {
        chasingStrategy.StopMoving();
    }

    private void Awake()
    {
        movingObject = GetComponentInParent<AIMovingObject>();
        fov = GetComponentInParent<FieldOfView>();

        chasingStrategy = GetComponent<AIChasingStrategy>();
    }
}
