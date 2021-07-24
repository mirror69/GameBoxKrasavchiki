using System.Collections;
using UnityEngine;

public abstract class AIMovementStrategy : MonoBehaviour
{
    const float AngleDifferenceEpsilon = 1f;

    protected AIMovingObject movingObject;

    protected Coroutine movingCoroutine = null;

    public void BaseInitialize(AIMovingObject movingObject)
    {
        this.movingObject = movingObject;
    }

    public bool IsStopped()
    {
        return movingCoroutine == null;
    }

    public virtual void StartMoving()
    {
        movingObject.Stop();
        if (movingCoroutine == null)
        {
            movingCoroutine = StartCoroutine(PerformMoving());
        }
    }

    public virtual void StopMoving()
    {
        if (movingCoroutine != null)
        {
            StopAllCoroutines();
            movingCoroutine = null;
        }
        movingObject.Stop();
    }

    protected abstract IEnumerator PerformMoving();

    protected IEnumerator PerformRotatingLeftRight(float fullAngle, float rotationSpeed)
    {
        // Вначале берем только половину максимального угла, т.к. персонаж смотрит прямо,
        // и нам нужно будет повернуться только наполовину
        // При повороте в обратную сторону будетм брать уже целый угол, т.к. именно на него
        // нужно будет повернуться, чтобы посмотреть в противоположную сторону
        float maxRotationAngle = fullAngle * .5f;
        int rotationMultiplier = 1;
        float fullRotation = 0;
        while (true)
        {
            yield return null;

            float rotationDelta = rotationSpeed * Time.deltaTime;
            movingObject.Rotate(rotationMultiplier * rotationDelta);

            fullRotation += rotationDelta;
            if (fullRotation >= maxRotationAngle)
            {
                fullRotation = 0;
                rotationMultiplier = -rotationMultiplier;
                maxRotationAngle = fullAngle;
            }
        }
    }

    protected IEnumerator PerformRotating(float angle, float rotationSpeed)
    {
        int direction = angle > 0 ? 1 : -1;
        float absAngle = Mathf.Abs(angle);
        float currentAngle = 0;
        while (currentAngle < absAngle)
        {
            yield return null;

            float rotationDelta = rotationSpeed * Time.deltaTime;
            movingObject.Rotate(rotationDelta * direction);
            currentAngle += rotationDelta;
        }
    }

    protected IEnumerator PerformInfiniteLookAt(Transform target, float rotationSpeed)
    {      
        while (target != null)
        {
            yield return null;

            PerformOneTickRotation(target.position, rotationSpeed);
        }
    }

    protected IEnumerator PerformInfiniteLookAt(Vector3 position, float rotationSpeed)
    {
        while (true)
        {
            yield return null;

            PerformOneTickRotation(position, rotationSpeed);
        }
    }

    protected IEnumerator PerformLookAt(Vector3 targetPosition, float rotationSpeed)
    {
        float angle = GetLookAtAngle(targetPosition);
        while (Mathf.Abs(angle) > AngleDifferenceEpsilon)
        {
            yield return null;

            PerformOneTickRotation(targetPosition, rotationSpeed);
        }
    }

    private float GetLookAtAngle(Vector3 targetPosition)
    {
        return Vector3.SignedAngle(movingObject.transform.forward,
            targetPosition - movingObject.Position, Vector3.up);
    }

    private void PerformOneTickRotation(Vector3 targetPosition, float rotationSpeed)
    {
        float angle = GetLookAtAngle(targetPosition);

        if (Mathf.Abs(angle) < AngleDifferenceEpsilon)
        {
            return;
        }
        int direction = angle > 0 ? 1 : -1;

        float rotationDelta = rotationSpeed * Time.deltaTime;
        movingObject.Rotate(rotationDelta * direction);
    }

}
