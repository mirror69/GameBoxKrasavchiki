using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AISearchingStrategy : AIMovementStrategy
{
    private Transform target;

    public void SetTarget(Transform target)
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
        movingObject.Move(target.position);
        while (!movingObject.IsDestinationReached())
        {
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);
       
        float rotationAngle = 360;
        float rotationSpeed = movingObject.RotationSpeed;
        float fullRotationTime = rotationAngle / rotationSpeed;

        Coroutine rotationCoroutine = StartCoroutine(PerformRotating(rotationAngle, rotationSpeed));
        float endTime = Time.time + fullRotationTime;
        while (Time.time < endTime)
        {
            yield return null;
        }
        StopCoroutine(rotationCoroutine);

        yield return new WaitForSeconds(0.5f);

        movingCoroutine = null;
    }
}
