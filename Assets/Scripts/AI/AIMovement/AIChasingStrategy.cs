using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIChasingStrategy : AIMovementStrategy
{
    const float CheckPeriod = 0.1f;

    private Transform target;

    public void SetTarget(Transform target)
    {
        this.target = target;
    }

    protected override IEnumerator PerformMoving()
    {
        while (target != null)
        {
            Debug.Log(target);
            movingObject.Move(target.position);
            
            yield return new WaitForSeconds(CheckPeriod);
        }

        movingCoroutine = null;
    }
}
