using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PathData))]
public class PathDataEditor : Editor
{
    private PathData pathData;

    private bool globalParametersFoldout = true;
    private bool pointParametersFoldout = true;

    private void AddParameters(PathPointData pathPointData, bool addGlobalTrigger)
    {
        foreach (var item in PathPointData.allParamNameList)
        {
            if (!pathPointData.TryGetParameter(item, out PathPointParameter param))
            {
                continue;
            }

            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            GUILayout.Label(item, GUILayout.Width(100));

            if (param.IsFloatValue())
            {
                float newValue = EditorGUILayout.FloatField(param.FloatValue, GUILayout.Width(30));
                if (param.FloatValue != newValue)
                {
                    Undo.RecordObject(target, $"Edited {item}");
                    param.FloatValue = newValue;
                }
            }
            else if (param.IsIntValue())
            {
                int newValue = EditorGUILayout.IntField(param.IntValue, GUILayout.Width(30));
                if (param.IntValue != newValue)
                {
                    Undo.RecordObject(target, $"Edited {item}");
                    param.IntValue = newValue;
                }
            }

            if (addGlobalTrigger)
            {
                GUILayout.Space(10);

                bool newValue = EditorGUILayout.Toggle(param.useGlobalValue, GUILayout.Width(15));
                if (param.useGlobalValue != newValue)
                {
                    Undo.RecordObject(target, $"Edited {nameof(param.useGlobalValue)} of {item}");
                    param.useGlobalValue = newValue;
                }

                GUILayout.Label("Use global");
            }

            GUILayout.EndHorizontal();

        }
    }
    public override void OnInspectorGUI()
    {
        globalParametersFoldout = EditorGUILayout.Foldout(globalParametersFoldout, "Global parameters");
        if (globalParametersFoldout)
        {
            GUILayout.BeginVertical();
            AddParameters(pathData.GlobalPointsData, false);
            GUILayout.EndVertical();
        }

        GUILayout.Space(10);

        pointParametersFoldout = EditorGUILayout.Foldout(pointParametersFoldout, "Local points parameters");
        if (pointParametersFoldout)
        {
            GUILayout.BeginVertical();
            GUILayout.Space(10);

            for (int i = 0; i < pathData.LocalPointsData.Count; i++)
            {               
                GUILayout.BeginVertical();
                GUILayout.Label($"<Point {i + 1}>");

                AddParameters(pathData.LocalPointsData[i], true);

                GUILayout.EndVertical();
            }
            GUILayout.EndVertical();
        }
        
        serializedObject.ApplyModifiedProperties();
        SceneView.RepaintAll();
    }

    private void OnEnable()
    {
        pathData = (PathData)target;
    }
    
}
