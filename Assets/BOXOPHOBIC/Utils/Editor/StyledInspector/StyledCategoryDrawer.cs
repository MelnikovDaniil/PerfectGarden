﻿// Cristian Pop - https://boxophobic.com/

using UnityEditor;
using UnityEngine;

namespace Boxophobic.StyledGUI
{
    [CustomPropertyDrawer(typeof(StyledCategory))]
    public class StyledCategoryAttributeDrawer : PropertyDrawer
    {
        StyledCategory a;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            a = (StyledCategory)attribute;

            property.boolValue = StyledGUI.DrawInspectorCategory(a.category, property.boolValue, a.colapsable, a.top, a.down);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return -2;
        }
    }
}
