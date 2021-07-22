using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AIChasingStrategy))]
public class ChasingState : FsmState
{
    private AIMovingObject movingObject;
    private AIChasingStrategy chasingStrategy;
    private Transform target;
    private FieldOfView fov;

    public override void OnStateEnter()
    {
        Debug.Log($"{name} starting chasing");
        fov.SetAlertDetectionDelay();
        chasingStrategy.SetTarget(target);
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
        chasingStrategy.Initialize(movingObject);
    }
    private void Start()
    {
        target = GameManager.Instance.Player.transform;
    }
}
