using UnityEngine;

public abstract class MovingObject : MonoBehaviour
{
    [SerializeField] 
    protected float speed = 3;
    [SerializeField]
    protected float rotationSpeed = 60;
    
    public float Speed => speed;
    public float RotationSpeed => rotationSpeed;
    public virtual Vector3 CurrentVelocity => GetCurrentVelocity();
    public virtual Vector3 Position => transform.position;

    public virtual void Move(Vector3 movementVector)
    {
    }
    public virtual void MoveToPoint(Vector3 point)
    {
    }
    public virtual void Rotate(Vector3 lookAtPoint)
    {
    }
    public virtual void Rotate(float rotation)
    {
    }
    public virtual Vector3 GetCurrentVelocity()
    {
        return Vector3.zero;
    }
    public virtual int GetTurnDirection()
    {
        return 0;
    }
}
