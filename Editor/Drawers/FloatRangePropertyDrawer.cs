//using UnityEngine;
//using UnityEngine.MaterialPropertyProvider;

//namespace UnityEditor.MaterialPropertyProvider
//{
//    [CustomPropertyDrawer(typeof(FloatRangeProperty))]
//    public class FloatRangePropertyDrawer : PropertyDrawer
//    {
//        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
//        {
//            var valueProp = property.FindPropertyRelative("value");
//            var minValueProp = property.FindPropertyRelative("minValue");
//            var maxValueProp = property.FindPropertyRelative("maxValue");

//            EditorGUI.BeginProperty(position, label, property);
//            EditorGUI.BeginChangeCheck();
//            var newvalue = EditorGUI.Slider(position, label, valueProp.floatValue, minValueProp.floatValue, maxValueProp.floatValue);
//            if (EditorGUI.EndChangeCheck())
//            {
//                valueProp.floatValue = Mathf.Clamp(newvalue, minValueProp.floatValue, maxValueProp.floatValue);
//            }
//            EditorGUI.EndProperty();
//        }
//    }
//}
