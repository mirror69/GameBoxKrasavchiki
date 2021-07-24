using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Наведение на цель
/// </summary>
public class AIAimingStrategy : AIMovementStrategy
{
    private IDamageable target;

    public void Initialize(AIMovingObject movingObject, IDamageable target)
    {
        BaseInitialize(movingObject);
        SetTarget(target);
    }

    public void SetTarget(IDamageable target)
    {
        this.target = target;
    }

    protected override IEnumerator PerformMoving()
    {
        if (target == null)
        {
            movingCoroutine = null;
            yield break;
        }

        movingObject.SetEnabledAutomaticRotation(false);
        Coroutine rotatingCoroutine = StartCoroutine(PerformInfiniteLookAt(target.Transform, 
            movingObject.RotationSpeed));
        while (rotatingCoroutine != null && target.IsAlive())
        {
            yield return new WaitForSeconds(0.5f);
        }

        movingCoroutine = null;
    }
}
