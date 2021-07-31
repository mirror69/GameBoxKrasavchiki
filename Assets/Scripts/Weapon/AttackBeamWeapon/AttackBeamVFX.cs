using UnityEngine;
using UnityEngine.VFX;

public class AttackBeamVFX : AttackBeamRenderer
{
    [SerializeField]
    VisualEffect visualEffect;

    public override void SetPosition(Vector3 startPoint, Vector3 endPoint)
    {
        transform.position = startPoint;
        transform.LookAt(endPoint);
        visualEffect.SetFloat("Distance", Vector3.Distance(startPoint, endPoint));
    }
}
