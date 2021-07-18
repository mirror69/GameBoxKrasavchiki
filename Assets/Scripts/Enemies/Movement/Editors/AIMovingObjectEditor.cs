using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AIMovingObject))]
public class AIMovingObjectEditor : Editor
{
    const float AnchorSize = 1f;

    private AIMovingObject movingObject;

    public static void DrawMovingObject(AIMovingObject movingObject)
    {
        if (movingObject.ObjectTransform == null)
        {
            return;
        }

        Transform objTransform = movingObject.ObjectTransform;

        Handles.color = Color.red;
        Vector3 newPosition = Handles.FreeMoveHandle(objTransform.position, Quaternion.identity, AnchorSize,
            Vector3.zero, Handles.CylinderHandleCap);
        if (objTransform.position != newPosition)
        {
            Undo.RecordObject(objTransform, $"Move {objTransform.name}");
            objTransform.position = newPosition;
        }
        Handles.color = Color.black;

        Handles.Label(newPosition + AnchorSize * Vector3.up, $"{objTransform.name}");
    }

    public static void DrawMovingStrategy(AIMovingObject movingObject)
    {
        if (movingObject.PatrolMovementStrategy == null)
        {
            return;
        }

        if (movingObject.PatrolMovementStrategy.GetType() != typeof(AIPatrolStrategy))
        {
            return;
        }
        AIPatrolStrategy patrolStrategy = (AIPatrolStrategy)movingObject.PatrolMovementStrategy;
        if (patrolStrategy.PathData == null || patrolStrategy.PathData.PathCreator == null)
        {
            return;
        }
        PathEditor.CallOnSceneGUI(patrolStrategy.PathData.PathCreator);
    }

    public void OnSceneGUI()
    {
        DrawMovingObject(movingObject);
        DrawMovingStrategy(movingObject);
    }

    private void OnEnable()
    {
        movingObject = (AIMovingObject)target;
    }
}
