using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PathCreator))]
public class PathEditor : Editor
{
    const float LineWidth = 2f;
    const float ArrowAngle = 20f;
    const float ArrowLength = 1f;
    const float AnchorSize = .5f;

    private PathCreator creator;
    private NPCPath path;

    public static void CallOnSceneGUI(PathCreator creator)
    {
        Input(creator);
        Draw(creator);
    }

    private static void Input(PathCreator creator)
    {
        const float MinSqrDistToDeletePoint = AnchorSize * AnchorSize;

        NPCPath path = creator.Path;

        // Отключаем возможность переходить на другие объекты по нажатию левой кнопки мыши
        int id = GUIUtility.GetControlID(FocusType.Passive);
        HandleUtility.AddDefaultControl(id);

        Event guiEvent = Event.current;

        // Добавляем точки левой кнопкой мыши с зажатым шифтом.
        // Удаляем точки правой кнопкой мыши с зажатым шифтом
        if (guiEvent.type == EventType.MouseUp && guiEvent.button == 0 &&
            guiEvent.shift)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition);
            if (creator.TryGetPointByRay(ray, out Vector3 point))
            {
                Undo.RecordObject(creator, "Add path point");
                creator.AddPoint(point);
            }
        }
        else if (guiEvent.type == EventType.MouseUp && guiEvent.button == 1 &&
            guiEvent.shift && path.PointsCount > 1)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition);
            if (creator.TryGetPointByRay(ray, out Vector3 point))
            {
                int index = path.GetNearestPoint(point);
                if (Vector3.SqrMagnitude(point - path[index]) < MinSqrDistToDeletePoint)
                {
                    Undo.RecordObject(creator, "Delete point");
                    creator.DeletePoint(index);
                }
            }
        }
        
    }

    private static Vector3 RotateUnitVector(Vector3 vector, float angleInDegrees)
    {
        float angleInRad = Mathf.Deg2Rad * angleInDegrees;
        float cos = Mathf.Cos(angleInRad);
        float sin = Mathf.Sin(angleInRad);

        float x = vector.x * cos - vector.z * sin;
        float z = vector.x * sin + vector.z * cos;

        return new Vector3(x, 0, z);
    }

    private static void DrawArrowOnLine(Vector3 startPoint, Vector3 endPoint, float length, 
        float lineScale, float angleInDegrees, Color color)
    {
        Handles.color = color;
        Vector3 lineVector = (startPoint - endPoint);
        float lineLength = lineVector.magnitude;

        Vector3 unitVector = lineVector / lineLength;
        Vector3 arrowPoint = endPoint + .5f * lineVector - length * .5f * unitVector;

        Vector3 vect1 = RotateUnitVector(unitVector, angleInDegrees) * length;
        Handles.DrawLine(vect1 + arrowPoint, arrowPoint, lineScale);

        Vector3 vect2 = RotateUnitVector(unitVector, -angleInDegrees) * length;
        Handles.DrawLine(vect2 + arrowPoint, arrowPoint, lineScale);

        Handles.DrawLine(arrowPoint + vect1, arrowPoint + vect2, lineScale);
    }

    private static void DrawLine(Vector3 startPoint, Vector3 endPoint)
    {
        Color lineColor = Color.green;

        Handles.color = lineColor;
        Handles.DrawLine(startPoint, endPoint, LineWidth);
        DrawArrowOnLine(startPoint, endPoint, ArrowLength, LineWidth, ArrowAngle, lineColor);
    }

    private static void Draw(PathCreator creator)
    {
        NPCPath path = creator.Path;

        Handles.color = Color.green;
        for (int i = 1; i < path.PointsCount; i++)
        {
            DrawLine(path[i - 1], path[i]);
        }
        if (path.IsClosed)
        {
            DrawLine(path[path.PointsCount - 1], path[0]);
        }

        for (int i = 0; i < path.PointsCount; i++)
        {
            Handles.color = Color.blue;
            Vector3 newPosition = Handles.FreeMoveHandle(path[i], Quaternion.identity, .5f,
                Vector3.zero, Handles.CubeHandleCap);
            if (path[i] != newPosition)
            {
                Undo.RecordObject(creator, "Move point");
                creator.MovePoint(i, newPosition);
            }
            Handles.color = Color.black;

            Handles.Label(path[i] + 0.5f * Vector3.up, $"{(i + 1)}");
        }

    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        bool isClosed = GUILayout.Toggle(path.IsClosed, "Is closed");
        if (isClosed != path.IsClosed)
        {
            Undo.RecordObject(creator, "Change Is closed");
            path.SetClosed(isClosed);
        }
        if (GUILayout.Button("Clear path"))
        {
            Undo.RecordObject(creator, "Clear path");
            creator.RefreshPath();
            path = creator.Path;
            SceneView.RepaintAll();
        }

        GUILayout.Space(20);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Add point"))
        {
            Undo.RecordObject(creator, "Add path point");
            creator.AddNextPoint();
            SceneView.RepaintAll();
        }
        if (GUILayout.Button("Delete point"))
        {
            if (creator.CanDeletePoints())
            {
                Undo.RecordObject(creator, "Delete path point");
                creator.DeletePoint(path.PointsCount - 1);
                SceneView.RepaintAll();
            }
        }
        GUILayout.EndHorizontal();

        serializedObject.ApplyModifiedProperties();
    }

    private void OnEnable()
    {
        creator = (PathCreator)target;
        creator.RefreshPath();
        path = creator.Path;
    }

    private void OnSceneGUI()
    {
        CallOnSceneGUI(creator);
    }

}
