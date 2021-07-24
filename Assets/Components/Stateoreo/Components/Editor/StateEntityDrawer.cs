using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer (typeof (FsmCore.StateEntity))]
public class StateEntityDrawer : PropertyDrawer {

	public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) {
		int oldIndentLevel = EditorGUI.indentLevel;
		EditorGUI.BeginProperty (position, label, property);

		Color oldColor = GUI.backgroundColor;
		SerializedProperty stateProp = property.FindPropertyRelative("State");
		if (Application.isPlaying)
		{
			// В PlayMode выделяем цветом активное состояние State machine
			MonoBehaviour propertyObject = (MonoBehaviour)stateProp.objectReferenceValue;
			if (propertyObject != null && propertyObject.enabled)
			{
				GUI.backgroundColor = Color.red;
			}
			// Хак для обновления отображения в инспекторе
			EditorUtility.SetDirty(property.serializedObject.targetObject);
		}

		EditorGUILayout.PropertyField(stateProp);
		GUI.backgroundColor = oldColor;

		EditorGUI.PropertyField (position, stateProp);

        EditorGUI.indentLevel += 1;
        TransitionRuleDrawer.showTransitionRuleList (property.FindPropertyRelative ("Transitions"));

		EditorGUI.EndProperty ();
		EditorGUI.indentLevel = oldIndentLevel;
	}

	public static void showStateEntityList (SerializedProperty list) {
        EditorGUILayout.PropertyField(list);

        EditorGUI.indentLevel += 1;
        EditorGUIUtility.labelWidth = 100f;
        if (list.isExpanded) {
			EditorGUILayout.PropertyField (list.FindPropertyRelative ("Array.size"));
			for (int i = 0; i < list.arraySize; ++i) {
				showStateEntity (list.GetArrayElementAtIndex (i), i);
			}
		}
		EditorGUI.indentLevel -= 1;
	}

	public static void showStateEntity (SerializedProperty se, int index) {
		string label = index == 0 ? "Initial" : "State " + index;
		EditorGUILayout.PropertyField (se, new GUIContent (label));
	}
}
