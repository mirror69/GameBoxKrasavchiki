using UnityEngine;

public abstract class AttackBeamRenderer : MonoBehaviour
{
    public virtual void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }
    public abstract void SetPosition(Vector3 startPoint, Vector3 endPoint);
}
