using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIChasingStrategy : AIMovementStrategy
{
    const float CheckPeriod = 0.1f;

    private Transform target;

    public void Initialize(AIMovingObject movingObject, Transform target)
    {
        BaseInitialize(movingObject);
        SetTarget(target);
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
    }

    protected override IEnumerator PerformMoving()
    {
        movingObject.SetEnabledAutomaticRotation(false);
        Coroutine rotatingCoroutine = StartCoroutine(PerformInfiniteLookAt(target, movingObject.RotationSpeed));
        while (target != null)
        {
            movingObject.Move(target.position);
            yield return new WaitForSeconds(CheckPeriod);
        }

        this.StopAndNullCoroutine(ref rotatingCoroutine);

        movingCoroutine = null;
    }
}
