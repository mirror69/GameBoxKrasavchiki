using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.AI;

[CustomEditor(typeof(AIMovingObject))]
public class AIMovingObjectEditor : Editor
{
    private AIMovingObject movingObject;

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

    public override void OnInspectorGUI()
    {
        DrawAgentTypeProperty("agentType");
        DrawAgentTypeProperty("agentTypeWalkingThroughWalls");
        serializedObject.ApplyModifiedProperties();
        base.OnInspectorGUI();
    }

    public void OnSceneGUI()
    {
        DrawMovingStrategy(movingObject);
    }

    private void OnEnable()
    {
        movingObject = (AIMovingObject)target;
    }

    private void DrawAgentTypeProperty(string propName)
    {
        SerializedProperty agentTypeID = serializedObject.FindProperty(propName);
        NavMeshComponentsGUIUtility.AgentTypePopup(propName, agentTypeID);
    }
}
