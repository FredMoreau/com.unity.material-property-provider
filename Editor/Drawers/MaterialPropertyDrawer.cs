using UnityEngine;
using UnityEngine.MaterialPropertyProvider;

namespace UnityEditor.MaterialPropertyProvider
{
    [CustomPropertyDrawer(typeof(IMaterialProperty))]
    public class MaterialPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var valueProp = property.FindPropertyRelative("value");
            EditorGUI.PropertyField(position, valueProp, label);

            //var enabledProp = property.FindPropertyRelative("enabled");

            //var enabledRect = new Rect(position.x, position.y, 24, position.height);
            //var valueRect = new Rect(enabledRect.max.x + EditorGUIUtility.labelWidth, position.y, position.width - EditorGUIUtility.labelWidth - 24, position.height);

            //EditorGUI.PropertyField(enabledRect, enabledProp, label);
            //EditorGUI.PropertyField(valueRect, valueProp, new GUIContent(""));
        }
    }
}
