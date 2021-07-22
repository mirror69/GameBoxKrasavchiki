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
        const float RotationAngle = 360;

        if (target == null)
        {
            movingCoroutine = null;
            yield break;
        }

        Vector3 lastTargetPoint = target.position;
        float angleToLastTargetPoint = Vector3.SignedAngle(movingObject.transform.forward,
            lastTargetPoint - movingObject.Position, Vector3.up);

        movingObject.SetEnabledAutomaticRotation(false);
        Coroutine rotatingCoroutine = StartCoroutine(PerformLookAt(lastTargetPoint, movingObject.RotationSpeed));

        movingObject.Move(lastTargetPoint);
        while (!movingObject.IsDestinationReached())
        {
            yield return null;
        }
        this.StopAndNullCoroutine(ref rotatingCoroutine);      

        float rotationAngle = angleToLastTargetPoint >= 0 ? RotationAngle : -RotationAngle;

        float rotationSpeed = movingObject.RotationSpeed;
        float fullRotationTime = Mathf.Abs(rotationAngle) / rotationSpeed;

        rotatingCoroutine = StartCoroutine(PerformRotating(rotationAngle, rotationSpeed));
        float endTime = Time.time + fullRotationTime;
        while (Time.time < endTime)
        {
            yield return null;
        }

        this.StopAndNullCoroutine(ref rotatingCoroutine);

        yield return new WaitForSeconds(0.5f);

        movingCoroutine = null;
    }
}
