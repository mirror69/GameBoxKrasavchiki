using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof (FsmCore))]
[CanEditMultipleObjects]
public class FsmCoreCustomEditor : Editor {

	public override void OnInspectorGUI () {
        base.OnInspectorGUI();
    }
}
