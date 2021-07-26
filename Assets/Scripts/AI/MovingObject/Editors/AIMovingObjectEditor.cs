using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.AI;

[CustomEditor(typeof(AIMovingObject))]
[CanEditMultipleObjects]
public class AIMovingObjectEditor : Editor
{
    private AIMovingObject movingObject;
    private AIPatrolStrategy patrolStrategy;

    public static void DrawPatrolStrategy(AIPatrolStrategy patrolStrategy)
    {
        if (patrolStrategy == null || patrolStrategy.PathData == null 
            || patrolStrategy.PathData.PathCreator == null)
        {
            return;
        }

        PathEditor.CallOnSceneGUI(patrolStrategy.PathData.PathCreator);
    }

    public override void OnInspectorGUI()
    {
        serializedObject.ApplyModifiedProperties();
        base.OnInspectorGUI();
    }

    public void OnSceneGUI()
    {
        DrawPatrolStrategy(patrolStrategy);
    }

    private void OnEnable()
    {
        movingObject = (AIMovingObject)target;
        patrolStrategy = movingObject.transform.GetComponentInChildren<AIPatrolStrategy>();
    }
}
