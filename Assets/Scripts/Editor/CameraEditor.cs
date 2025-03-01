using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CameraManger))]
[CanEditMultipleObjects]
public class CameraEditor : Editor
{
    public override void OnInspectorGUI()
    {
        CameraManger myComponent = (CameraManger)target;

        DrawDefaultInspector();

        if (GUILayout.Button("Move Camera"))
        {
            myComponent.LookAtObjectPresetup();
        }
        EditorGUILayout.Space();
        if (GUILayout.Button("Reset position"))
        {
            myComponent.ReturnToOriginalPosition();
        }
    }
}
