using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsStateOver : FsmCondition
{
    public override bool IsSatisfied(FsmState curr, FsmState next)
    {
        return !curr.IsPerforming();
    }
}
