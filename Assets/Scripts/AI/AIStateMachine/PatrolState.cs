using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : FsmState
{
    private AIMovingObject movingObject;
    private FieldOfView fov;

    public override void OnStateEnter()
    {
        Debug.Log($"{name} starting patrol");
        fov.SetIdleDetectionDelay();
        movingObject.StartPatrol();
    }

    public override void OnStateLeave()
    {
    }

    private void Awake()
    {
        movingObject = GetComponentInParent<AIMovingObject>();
        fov = GetComponentInParent<FieldOfView>();
    }
}
