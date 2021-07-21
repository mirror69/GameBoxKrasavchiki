using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsTargetNotFound : FsmCondition
{
    private AIMovingObject movingObject;

    public override bool IsSatisfied(FsmState curr, FsmState next)
    {
        return movingObject.IsStopped();
    }

    private void Awake()
    {
        movingObject = GetComponentInParent<AIMovingObject>();
    }
}
