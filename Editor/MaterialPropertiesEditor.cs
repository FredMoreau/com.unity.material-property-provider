using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.MaterialPropertyProvider;
using UnityEditorInternal;

namespace UnityEditor.MaterialPropertyProvider
{
    [CustomEditor(typeof(MaterialProperties))]
    public class MaterialPropertiesEditor : Editor
    {
        SerializedProperty renderersProp;
        SerializedProperty materialPropertiesProp;
        ReorderableList materialPropertiesList;

        private void OnEnable()
        {
            renderersProp = serializedObject.FindProperty("_renderers");
            renderersProp = serializedObject.FindProperty("materialProperties");
            materialPropertiesList = new ReorderableList(serializedObject, materialPropertiesProp, true, true, true, true);
            materialPropertiesList.drawHeaderCallback = (rect) =>
            {
                EditorGUI.LabelField(rect, "Material Properties");
            };
            materialPropertiesList.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                var element = materialPropertiesProp.GetArrayElementAtIndex(index);
                EditorGUI.PropertyField(rect, element);
            };
            materialPropertiesList.onAddDropdownCallback = (buttonRect, list) =>
            {
                var menu = new GenericMenu();
                foreach (ShaderPropertyType type in System.Enum.GetValues(typeof(ShaderPropertyType)))
                {
                    menu.AddItem(new GUIContent(type.ToString()), false, () =>
                    {
                        Undo.RecordObject(target, "Add Renderer Property");
                        (target as MaterialProperties).Add(IMaterialProperty.FromPropertyType(type, $"New {type} Property"));
                        //materialPropertiesProp.arraySize++;
                        //var newElement = materialPropertiesProp.GetArrayElementAtIndex(materialPropertiesProp.arraySize - 1);
                        //newElement.managedReferenceValue = IMaterialProperty.FromPropertyType(type, $"New {type} Property");
                        serializedObject.ApplyModifiedProperties();
                    });
                }
                menu.ShowAsContext();
            };
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(renderersProp);
            materialPropertiesList.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }
    }
}
