using UnityEngine;
using UnityEngine.MaterialPropertyProvider;

namespace UnityEditor.MaterialPropertyProvider
{
    [CustomPropertyDrawer(typeof(FloatRangeProperty))]
    public class FloatRangePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var valueProp = property.FindPropertyRelative("value");
            var minValueProp = property.FindPropertyRelative("minValue");
            var maxValueProp = property.FindPropertyRelative("maxValue");

            EditorGUI.Slider(position, valueProp, minValueProp.floatValue, maxValueProp.floatValue, label);
        }
    }
}
