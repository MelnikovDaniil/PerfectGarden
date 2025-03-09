using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CameraManager))]
[CanEditMultipleObjects]
public class CameraEditor : Editor
{
    public override void OnInspectorGUI()
    {
        CameraManager myComponent = (CameraManager)target;

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
