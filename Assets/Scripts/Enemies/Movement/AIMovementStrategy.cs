using UnityEngine;

public abstract class AIMovementStrategy : MonoBehaviour
{
    protected AIMovingObject movingObject;

    public virtual void Initialize(AIMovingObject movingObject)
    {
        this.movingObject = movingObject;
    }
    public abstract void StartMoving();

    public abstract void StopMoving();
}
