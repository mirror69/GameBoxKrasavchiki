using System.Collections;
using UnityEngine;

public abstract class AIMovementStrategy : MonoBehaviour
{
    protected AIMovingObject movingObject;
    protected Vector3 initialDirection;

    protected Coroutine movingCoroutine = null;

    public bool IsStopped()
    {
        return movingCoroutine == null;
    }

    public virtual void Initialize(AIMovingObject movingObject)
    {
        this.movingObject = movingObject;
        initialDirection = movingObject.transform.forward;
    }
    public virtual void StartMoving()
    {
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

}
