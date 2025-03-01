using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlantInfo))]
public class DataWithTimeSpanEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Получаем объект-цель редактирования
        PlantInfo data = (PlantInfo)target;

        // Используем Reflection для получения последовательности полей
        FieldInfo[] fields = typeof(PlantInfo).GetFields(BindingFlags.Public | BindingFlags.Instance);

        serializedObject.Update();

        foreach (FieldInfo field in fields)
        {
            if (field.FieldType == typeof(TimeSpan))
            {
                GUILayout.Label("Time Between Stages", EditorStyles.boldLabel);

                GUILayout.BeginVertical();
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Hours", GUILayout.MaxWidth(100));
                    GUILayout.Label("Minutes", GUILayout.MaxWidth(100));
                    GUILayout.Label("Seconds", GUILayout.MaxWidth(100));
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    int hours = data.timeBetweenStages.Hours;
                    int minutes = data.timeBetweenStages.Minutes;
                    int seconds = data.timeBetweenStages.Seconds;

                    hours = EditorGUILayout.IntField(hours, GUILayout.MaxWidth(100));
                    minutes = EditorGUILayout.IntField(minutes, GUILayout.MaxWidth(100));
                    seconds = EditorGUILayout.IntField(seconds, GUILayout.MaxWidth(100));

                    data.timeBetweenStages = new TimeSpan(hours, minutes, seconds);
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndVertical();
            }
            else
            {
                SerializedProperty property = serializedObject.FindProperty(field.Name);

                if (property != null)
                {
                    EditorGUILayout.PropertyField(property, true);
                }
            }
        }

        serializedObject.ApplyModifiedProperties();
        EditorUtility.SetDirty(target);
    }
}