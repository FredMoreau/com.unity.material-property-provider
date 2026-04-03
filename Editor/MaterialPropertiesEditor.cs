using System.Collections.Generic;
using System;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.MaterialPropertyProvider;
using UnityEngine.Rendering;

namespace UnityEditor.MaterialPropertyProvider
{
    [CustomEditor(typeof(MaterialProperties))]
    public class MaterialPropertiesEditor : Editor
    {
        SerializedProperty renderersProp;
        SerializedProperty materialPropertiesProp;
        SerializedProperty materialKeywordsProp;
        ReorderableList materialPropertiesList;
        ReorderableList materialKeywordsList;
        static List<string> dropDownLabels = new();
        static List<Type> propertyValueTypes = new();

        [InitializeOnLoadMethod]
        static void ReflectShaderPropertyTypesAndStoreMenuItems()
        {
            foreach (ShaderPropertyType type in Enum.GetValues(typeof(ShaderPropertyType)))
            {
                propertyValueTypes.Add(IMaterialProperty.FromPropertyType(type, string.Empty).GetType());
                dropDownLabels.Add(type.ToString());
            }
        }

        private void OnEnable()
        {
            renderersProp = serializedObject.FindProperty("_renderers");

            materialPropertiesProp = serializedObject.FindProperty("materialProperties");
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
                for (int optionIndex = 0; optionIndex < dropDownLabels.Count; optionIndex++)
                {
                    var index = optionIndex; // Capture the current index for the closure
                    menu.AddItem(new GUIContent(dropDownLabels[optionIndex]), false, () =>
                    {
                        var prop = Activator.CreateInstance(propertyValueTypes[index]) as IMaterialProperty;
                        if (prop != null)
                        {
                            Undo.RecordObject(target, "Add Renderer Property");
                            (target as MaterialProperties).Add(prop);
                            serializedObject.ApplyModifiedProperties();
                        }
                    });
                }
                menu.ShowAsContext();
            };

            materialKeywordsProp = serializedObject.FindProperty("materialKeywords");
            materialKeywordsList = new ReorderableList(serializedObject, materialKeywordsProp, true, true, true, true);
            materialKeywordsList.drawHeaderCallback = (rect) =>
            {
                EditorGUI.LabelField(rect, "Material Keywords");
            };
            materialKeywordsList.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                var element = materialKeywordsProp.GetArrayElementAtIndex(index);
                EditorGUI.PropertyField(rect, element);
            };
            materialKeywordsList.onAddCallback = (list) =>
            {
                Undo.RecordObject(target, "Add Renderer Keyword");
                (target as MaterialProperties).Add(new MaterialKeyword());
                serializedObject.ApplyModifiedProperties();
            };
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            materialPropertiesList.DoLayoutList();
            if (materialPropertiesList.index >= 0 && materialPropertiesList.index < materialPropertiesProp.arraySize)
            {
                var element = materialPropertiesProp.GetArrayElementAtIndex(materialPropertiesList.index);
                var nameProp = element.FindPropertyRelative("name");
                EditorGUILayout.PropertyField(nameProp);
            }

            materialKeywordsList.DoLayoutList();
            if (materialKeywordsList.index >= 0 && materialKeywordsList.index < materialKeywordsProp.arraySize)
            {
                var element = materialKeywordsProp.GetArrayElementAtIndex(materialKeywordsList.index);
                var nameProp = element.FindPropertyRelative("name");
                EditorGUILayout.PropertyField(nameProp);
            }

            EditorGUILayout.PropertyField(renderersProp);
            serializedObject.ApplyModifiedProperties();
        }
    }
}
