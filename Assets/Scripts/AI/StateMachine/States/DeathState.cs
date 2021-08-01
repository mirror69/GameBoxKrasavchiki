using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathState : FsmState
{
    private AIMovingObject movingObject;
    private FieldOfView fov;
    public override void OnStateEnter()
    {
        Debug.Log($"{movingObject.name} killed");

        movingObject.SetEnabled(false);
        fov.SetEnabled(false);
    }

    private void Awake()
    {
        movingObject = GetComponentInParent<AIMovingObject>();
        fov = GetComponentInParent<FieldOfView>();
    }
}
