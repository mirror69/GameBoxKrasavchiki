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
        Handles.color = Color.white;

        Vector3 sightCenter = fov.PointOfSight.transform.position;

        float fovRotation = fov.PointOfSight.transform.eulerAngles.y;
        VisualizeFOV(sightCenter, fovRotation, fov.Angle, fov.Distance);

        Vector3 fovEdgeVector1 = GetFovEdgeVector(fovRotation, -fov.Angle / 2);
        Vector3 fovEdgeVector2 = GetFovEdgeVector(fovRotation, fov.Angle / 2);

        Handles.color = Color.yellow;

        Handles.DrawWireArc(sightCenter, Vector3.up, fovEdgeVector1, fov.Angle, fov.Distance);
      
        Handles.DrawLine(sightCenter, sightCenter + fovEdgeVector1 * fov.Distance);
        Handles.DrawLine(sightCenter, sightCenter + fovEdgeVector2 * fov.Distance);

        if (fov.DetectingState == FieldOfView.TargetDetectingState.Detected)
        {
            Handles.color = Color.red;
            Handles.DrawLine(sightCenter, fov.TargetPosition);
        }
    }

    private Vector3 GetFovEdgeVector(float forwardVectorAngle, float shiftAngle)
    {
        float vectorAngleInRad = (forwardVectorAngle + shiftAngle) * Mathf.Deg2Rad;
        return new Vector3(Mathf.Sin(vectorAngleInRad), 0, Mathf.Cos(vectorAngleInRad));
    }

    private void VisualizeFOV(Vector3 fovCenter, float fovRotation, float fowAngle, float fowDistance)
    {
        const float linesStepByDegree = 10f;
        float currentAngle = -fowAngle / 2;
        Handles.color = Color.green;
        while (currentAngle < fowAngle / 2)
        {
            Vector3 fovEdgeVector = GetFovEdgeVector(fovRotation, currentAngle);
            Handles.DrawLine(fovCenter, fovCenter + fovEdgeVector * fowDistance);
            currentAngle += linesStepByDegree;
        }
    }
}
