using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsDead : FsmCondition
{
    private IDamageable healthObject;

    public override bool IsSatisfied(FsmState curr, FsmState next)
    {
        return !healthObject.IsAlive();
    }
    private void Awake()
    {
        healthObject = GetComponentInParent<IDamageable>();
    }
}
