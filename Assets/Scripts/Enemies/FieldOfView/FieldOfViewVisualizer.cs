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
    const float defaultLinesStepInDegrees = 10f;

    [SerializeField]
    [Range(1, 20)]
    private float linesStepInDegrees = defaultLinesStepInDegrees;
    [SerializeField]
    private Material fovMaterial;
    [SerializeField]
    private Material fovDetectedMaterial;

    public static float LinesStepInDegrees = defaultLinesStepInDegrees;
    /// <summary>
    /// Получить единичный вектор визуализации поля зрения
    /// </summary>
    /// <param name="forwardVectorAngle"></param>
    /// <param name="shiftAngle"></param>
    /// <returns></returns>
    public static Vector3 GetFovVector(float forwardVectorAngle, float shiftAngle)
    {
        float vectorAngleInRad = (forwardVectorAngle + shiftAngle) * Mathf.Deg2Rad;
        return new Vector3(Mathf.Sin(vectorAngleInRad), 0, Mathf.Cos(vectorAngleInRad));
    }

    /// <summary>
    /// Получить вектора поля зрения
    /// </summary>
    /// <param name="fovCenter"></param>
    /// <param name="fovRotation"></param>
    /// <param name="fowAngle"></param>
    /// <returns></returns>
    public static List<Vector3> GetFovVectors(Vector3 fovCenter, float fovRotation, float fowAngle)
    {
        //const float linesStepInDegrees = 10f;

        List<Vector3> fovLines = new List<Vector3>();

        float currentAngle = -fowAngle / 2;
        while (currentAngle <= fowAngle / 2)
        {
            Vector3 fovEdgeVector = GetFovVector(fovRotation, currentAngle);
            fovLines.Add(fovEdgeVector);

            currentAngle += LinesStepInDegrees;
        }
        return fovLines;
    }

    /// <summary>
    /// Установить длину векторов визуализации поля зрения с учетом препятствий
    /// </summary>
    /// <param name="fovVectors"></param>
    /// <param name="fovCenter"></param>
    /// <param name="fovDistance"></param>
    public static void SetFovLinesLengthWithObstacleInfluence(List<Vector3> fovVectors, Vector3 fovCenter,
        float fovDistance)
    {
        for (int i = 0; i < fovVectors.Count; i++)
        {
            if (Physics.Raycast(fovCenter, fovVectors[i], out RaycastHit hitInfo, fovDistance,
                ~FieldOfViewManager.Instance.NotObstacleLayers))
            {
                fovVectors[i] = fovVectors[i] * hitInfo.distance;
            }
            else
            {
                fovVectors[i] = fovVectors[i] * fovDistance;
            }
        }
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
        Vector3 sightCenter = fov.PointOfSight.transform.position;
        float fovRotation = fov.PointOfSight.transform.eulerAngles.y;

        List<Vector3> fovLines = GetFovVectors(sightCenter, fovRotation, fov.Angle);
        SetFovLinesLengthWithObstacleInfluence(fovLines, sightCenter, fov.Distance);

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
        for (int i = 1; i < fovLines.Count; i++)
        {
            GL.Vertex(sightCenter);
            GL.Vertex(sightCenter + fovLines[i - 1]);
            GL.Vertex(sightCenter + fovLines[i]);
        }
        GL.End();
        GL.PopMatrix();
    }

}
