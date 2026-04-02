using UnityEngine;
using UnityEngine.MaterialPropertyProvider;

namespace UnityEditor.MaterialPropertyProvider
{
    [CustomPropertyDrawer(typeof(IMaterialProperty))]
    public class MaterialPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var nameProp = property.FindPropertyRelative("name");
            var valueProp = property.FindPropertyRelative("value");

            EditorGUI.PropertyField(position, valueProp, label);
        }
    }
}
