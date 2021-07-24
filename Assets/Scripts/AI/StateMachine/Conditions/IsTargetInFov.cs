using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Условие попадания цели в зону видимости
/// </summary>
public class IsTargetInFov : FsmCondition
{    
   
    private FieldOfView fov;

    public override bool IsSatisfied(FsmState curr, FsmState next)
    {
        return fov.DetectingState == FieldOfView.TargetDetectingState.Detected;
    }

    private void Awake()
    {
        fov = GetComponentInParent<FieldOfView>();
    }

}
