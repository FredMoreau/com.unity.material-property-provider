using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.MaterialPropertyProvider;

namespace UnityEditor.MaterialPropertyProvider
{
    [CustomPropertyDrawer(typeof(MaterialPropertyOverride<>), true)]
    internal class MaterialPropertyOverrideDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            RangePropertyAttribute rangeAttribute = fieldInfo.GetCustomAttribute<RangePropertyAttribute>();
            ColorUsagePropertyAttribute colorUsageAttribute = fieldInfo.GetCustomAttribute<ColorUsagePropertyAttribute>();

            EditorGUI.BeginProperty(position, label, property);

            // Draw label
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            // Don't make child fields be indented
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            // Calculate rects
            var setRect = new Rect(position.x, position.y, 15, position.height);
            var consumed = setRect.width + 5;
            var valueRect = new Rect(position.x + consumed, position.y, position.width - consumed, position.height);

            // Draw fields - pass GUIContent.none to each so they are drawn without labels
            var hasValueProp = property.FindPropertyRelative(MaterialPropertyOverride<int>.enabledFieldName);
            EditorGUI.PropertyField(setRect, hasValueProp, GUIContent.none);
            bool guiEnabled = GUI.enabled;
            GUI.enabled = guiEnabled && hasValueProp.boolValue;
            ValueField(valueRect, property.FindPropertyRelative(MaterialPropertyOverride<int>.valueFieldName), GUIContent.none, rangeAttribute, colorUsageAttribute);
            GUI.enabled = guiEnabled;

            // Set indent back to what it was
            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }

        public virtual void ValueField(Rect position, SerializedProperty property, GUIContent label, RangePropertyAttribute range = null, ColorUsagePropertyAttribute colorUsage = null)
        {
            if (range != null && property.propertyType == SerializedPropertyType.Float)
            {
                property.floatValue = EditorGUI.Slider(position, label, property.floatValue, range.min, range.max);
            }
            else if (colorUsage != null && property.propertyType == SerializedPropertyType.Color)
            {
                property.colorValue = EditorGUI.ColorField(position, label, property.colorValue, true, colorUsage.showAlpha, colorUsage.hdr);
            }
            else
            {
                EditorGUI.PropertyField(position, property, label);
            }
        }
    }
}
