using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[ExecuteInEditMode]
/// <summary>
/// Визуализатор поля зрения
/// </summary>
public class FieldOfViewVisualizer : MonoBehaviour
{

    public struct ViewCastInfo
    {
        public bool hit;
        public Vector3 point;
        public float dist;
        public float angle;

        public ViewCastInfo(bool hit, Vector3 point, float dist,
            float angle)
        {
            this.hit = hit;
            this.point = point;
            this.dist = dist;
            this.angle = angle;
        }
    }

    const int EdgeResolveIterationCount = 6;
    const float EdgeDistanceThreshold = 0.5f;
    const float DefaultLinesStepInDegrees = 10f;

    [SerializeField]
    [Range(1, 20)]
    private float linesStepInDegrees = DefaultLinesStepInDegrees;
    [SerializeField]
    private Material fovMaterial;
    [SerializeField]
    private Material fovDetectedMaterial;

    public static float LinesStepInDegrees = DefaultLinesStepInDegrees;
    /// <summary>
    /// Получить единичный вектор визуализации поля зрения
    /// </summary>
    /// <param name="globalAngle"></param>
    /// <returns></returns>
    public static Vector3 GetFovVector(float globalAngle)
    {
        float vectorAngleInRad = globalAngle * Mathf.Deg2Rad;
        return new Vector3(Mathf.Sin(vectorAngleInRad), 0, Mathf.Cos(vectorAngleInRad));
    }

    /// <summary>
    /// Получить вектора поля зрения
    /// </summary>
    /// <param name="fovRotation"></param>
    /// <param name="fowAngle"></param>
    /// <returns></returns>
    public static List<Vector3> GetFovVectors(float fovRotation, float fowAngle)
    {
        List<Vector3> fovLines = new List<Vector3>();

        float currentAngle = -fowAngle / 2;
        while (currentAngle <= fowAngle / 2)
        {
            Vector3 fovEdgeVector = GetFovVector(fovRotation + currentAngle);
            fovLines.Add(fovEdgeVector);

            currentAngle += LinesStepInDegrees;
        }
        return fovLines;
    }

    public static List<Vector3> GetFovEdgePointsWithObstacleInfluence(Vector3 fovCenter, float fovRotation, float fowAngle,
        float fovDistance)
    {
        List<Vector3> fovLines = new List<Vector3>();
        ViewCastInfo newViewCastInfo = new ViewCastInfo();
        ViewCastInfo oldViewCastInfo = new ViewCastInfo();

        float minAngle = fovRotation - fowAngle / 2;
        float maxAngle = fovRotation - fowAngle / 2;
        float currentAngle = minAngle;

        bool isFirstIteration = true;
        while (currentAngle < fovRotation + fowAngle / 2)
        {
            newViewCastInfo = ViewCast(fovCenter, currentAngle, fovDistance);
            currentAngle += LinesStepInDegrees;

            if (!isFirstIteration)
            {
                bool isThresholdEsceeded =
                    Mathf.Abs(newViewCastInfo.dist - oldViewCastInfo.dist) > EdgeDistanceThreshold;

                if (newViewCastInfo.hit != oldViewCastInfo.hit ||
                    newViewCastInfo.hit && oldViewCastInfo.hit && isThresholdEsceeded)
                {
                    newViewCastInfo = FindEdge2(fovLines, fovCenter, fovDistance, oldViewCastInfo,
                        newViewCastInfo);
                    currentAngle = newViewCastInfo.angle;
                }
            }

            fovLines.Add(newViewCastInfo.point);
          
            isFirstIteration = false;
            oldViewCastInfo = newViewCastInfo;
        }
        newViewCastInfo = ViewCast(fovCenter, fovRotation + fowAngle / 2, fovDistance);
        fovLines.Add(newViewCastInfo.point);
        return fovLines;
    }

    private static ViewCastInfo ViewCast(Vector3 fovCenter, float globalAngle, float fovDistance)
    {
        Vector3 fovEdgeVector = GetFovVector(globalAngle);

        if (Physics.Raycast(fovCenter, fovEdgeVector, out RaycastHit hitInfo, fovDistance,
            ~FieldOfViewManager.Instance.NotObstacleLayers))
        {
            return new ViewCastInfo(true, hitInfo.point, hitInfo.distance, globalAngle);
        }
        else
        {
            return new ViewCastInfo(false, fovCenter + fovEdgeVector * fovDistance, fovDistance, globalAngle);
        }
    }

    private static ViewCastInfo FindEdge2(List<Vector3> list, Vector3 fovCenter, float fovDistance, ViewCastInfo minViewCast,
        ViewCastInfo maxViewCast)
    {
        float minAngle = minViewCast.angle;
        float maxAngle = maxViewCast.angle;
        ViewCastInfo newViewCast = new ViewCastInfo();

        for (int i = 0; i < EdgeResolveIterationCount; i++)
        {
            float angle = (minAngle + maxAngle) * .5f;
            newViewCast = ViewCast(fovCenter, angle, fovDistance);

            bool isThresholdEsceeded =
               Mathf.Abs(newViewCast.dist - minViewCast.dist) > EdgeDistanceThreshold;

            // Будем уточнять только правую границу
            if (newViewCast.hit == minViewCast.hit && !isThresholdEsceeded)
            {
                break;
            }
            else
            {
                maxAngle = angle;
            }
        }
        return newViewCast;

    }

    [ExecuteInEditMode]
    private void Update()
    {
        LinesStepInDegrees = linesStepInDegrees;
    }

    private void OnEnable()
    {
        RenderPipelineManager.endCameraRendering += OnEndCameraRendering;
    }

    private void OnDisable()
    {
        RenderPipelineManager.endCameraRendering -= OnEndCameraRendering;
    }

    private void OnPostRender()
    {
        DrawAllFovs();
    }

    private void OnEndCameraRendering(ScriptableRenderContext context, Camera camera)
    {
        OnPostRender();
    }

    /// <summary>
    /// Визуализировать все объекты поля зрения
    /// </summary>
    private void DrawAllFovs()
    {
        if (FieldOfViewManager.Instance == null)
        {
            return;
        }
        List<FieldOfView> fieldsOfView = FieldOfViewManager.Instance.FieldsOfView;
        if (fieldsOfView == null)
        {
            return;
        }

        foreach (var fov in FieldOfViewManager.Instance.FieldsOfView)
        {
            DrawFov(fov);
        }
    }

    /// <summary>
    /// Визуализировать поле зрения
    /// </summary>
    /// <param name="fov"></param>
    private void DrawFov(FieldOfView fov) 
    {
        Vector3 fovCenter = fov.PointOfSight.transform.position;
        float fovRotation = fov.PointOfSight.transform.eulerAngles.y;

        //List<Vector3> fovLines = GetFovVectors(fovRotation, fov.Angle);
        //SetFovLinesLengthWithObstacleInfluence(fovLines, sightCenter, fov.Distance);

        List<Vector3> fovPoints = GetFovEdgePointsWithObstacleInfluence(fovCenter, fovRotation, fov.Angle, fov.Distance);

        GL.PushMatrix();

        if (fov.DetectingState == FieldOfView.TargetDetectingState.Detected)
        {
            fovDetectedMaterial.SetPass(0);
        }
        else
        {
            fovMaterial.SetPass(0);
        }

        GL.Begin(GL.TRIANGLES);
        for (int i = 1; i < fovPoints.Count; i++)
        {
            GL.Vertex(fovCenter);
            GL.Vertex(fovPoints[i - 1]);
            GL.Vertex(fovPoints[i]);
        }
        GL.End();
        GL.PopMatrix();
    }

}
