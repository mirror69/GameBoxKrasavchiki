using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
