using UnityEngine;

public class AttackBeamLineRenderer : AttackBeamRenderer
{
    [SerializeField]
    LineRenderer lineRenderer;

    public override void SetPosition(Vector3 startPoint, Vector3 endPoint)
    {
        lineRenderer.SetPosition(0, startPoint);
        lineRenderer.SetPosition(1, endPoint);
    }

    private void Awake()
    {
        lineRenderer.useWorldSpace = true;       
    }
}
