using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Отрисовка всопогательной информации в SceneView для настройки компонента FieldOfView
/// </summary>
[CustomEditor(typeof(FieldOfView))]
public class FieldOfViewEditor : Editor
{
    private void OnSceneGUI()
    {
        FieldOfView fov = target as FieldOfView;

        Vector3 sightCenter = fov.PointOfSight.transform.position;
        float fovRotation = fov.PointOfSight.transform.eulerAngles.y;

        // Визуализируем поле зрения
        VisualizeInsideFOV(sightCenter, fovRotation, fov.Angle, fov.Distance);
        VisualizeOutsideFOV(sightCenter, fovRotation, fov.Angle, fov.Distance);

        // Если цель в поле зрения, нарисуем красный отрезок до цели 
        if (fov.DetectingState == FieldOfView.TargetDetectingState.Detected)
        {
            Handles.color = Color.red;
            Handles.DrawLine(sightCenter, fov.TargetPosition);
        }
    }

    /// <summary>
    /// Визуализировать внутреннюю часть поля зрения
    /// </summary>
    /// <param name="fovCenter"></param>
    /// <param name="fovRotation"></param>
    /// <param name="fowAngle"></param>
    /// <param name="fowDistance"></param>
    private void VisualizeInsideFOV(Vector3 fovCenter, float fovRotation, float fowAngle, float fowDistance)
    {
        List<Vector3> fovLines = FieldOfViewVisualizer.GetFovVectors(fovRotation, fowAngle);
        Handles.color = Color.green;
        foreach (var item in fovLines)
        {
            Handles.DrawLine(fovCenter, fovCenter + item * fowDistance);
        }
    }

    /// <summary>
    /// Визуализировать внешнюю часть поля зрения
    /// </summary>
    /// <param name="fovCenter"></param>
    /// <param name="fovRotation"></param>
    /// <param name="fowAngle"></param>
    /// <param name="fowDistance"></param>
    private void VisualizeOutsideFOV(Vector3 fovCenter, float fovRotation, float fowAngle, float fowDistance)
    {
        Vector3 fovEdgeVector1 = FieldOfViewVisualizer.GetFovVector(fovRotation - fowAngle / 2);
        Vector3 fovEdgeVector2 = FieldOfViewVisualizer.GetFovVector(fovRotation + fowAngle / 2);

        Handles.color = Color.yellow;

        Handles.DrawWireArc(fovCenter, Vector3.up, fovEdgeVector1, fowAngle, fowDistance);

        Handles.DrawLine(fovCenter, fovCenter + fovEdgeVector1 * fowDistance);
        Handles.DrawLine(fovCenter, fovCenter + fovEdgeVector2 * fowDistance);
    }
}
