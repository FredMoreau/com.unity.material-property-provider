using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine.Rendering;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityEngine.MaterialPropertyProvider
{
    /// <summary>
    /// A <see cref="MonoBehaviour"/> based class that will automatically set its <see cref="Renderer"/> with a <see cref="MaterialPropertyBlock"/>
    /// <para>When deriving from <see cref="MaterialPropertyProviderBase"/>, fields and properties with <see cref="MaterialPropertyAttribute"/> will automatically be set on the <see cref="Renderer"/>.</para>
    /// <para>Properties are automatically updated upon Awake(), Start(), Reset() and OnValidate(). To enable animated properties, or force update, call <seealso cref="UpdateProperties"/>.</para>
    /// </summary>
    public abstract class MaterialPropertyProviderBase : MaterialPropertyManager
    {
        private static Dictionary<Type, Dictionary<int, FieldInfo>> _allFields = new();
        private static Dictionary<Type, Dictionary<int, PropertyInfo>> _allProperties = new();

        private Type _type;
        private Type type
        {
            get
            {
                if (_type == null)
                    _type = GetType();
                return _type;
            }
        }

        private static void Add(Type type, string name, FieldInfo fieldInfo)
        {
            int nameID = Shader.PropertyToID(name);

            if (!_allFields.ContainsKey(type))
                _allFields.Add(type, new Dictionary<int, FieldInfo>());

            if (!_allFields[type].ContainsKey(nameID))
                _allFields[type].Add(nameID, fieldInfo);
        }

        private static void Add(Type type, string name, PropertyInfo propertyInfo)
        {
            int nameID = Shader.PropertyToID(name);

            if (!_allProperties.ContainsKey(type))
                _allProperties.Add(type, new Dictionary<int, PropertyInfo>());

            if (!_allProperties[type].ContainsKey(nameID))
                _allProperties[type].Add(nameID, propertyInfo);
        }

#if UNITY_EDITOR
        static Dictionary<Type, MonoScript> classSources = new();
        [InitializeOnLoadMethod]
#endif
        [RuntimeInitializeOnLoadMethod]
        private static void Init()
        {
            var types = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                         from type in assembly.GetTypes()
                         where type.IsSubclassOf(typeof(MaterialPropertyProviderBase))
                         select type).ToList();

#if UNITY_EDITOR
            classSources.Clear();
            var scripts = AssetDatabase.FindAssets("t:MonoScript");
            foreach (var guid in scripts)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var mScript = AssetDatabase.LoadAssetAtPath<MonoScript>(path);
                var type = mScript.GetClass();
                if (types.Contains(type) && !classSources.ContainsKey(type))
                    classSources.Add(type, mScript);
            }
#endif

            _allFields.Clear();
            _allProperties.Clear();

            foreach (Type type in types)
            {
                foreach (var field in type.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
                {
                    var attrs = (MaterialPropertyAttribute[])field.GetCustomAttributes(typeof(MaterialPropertyAttribute), false);

                    if (attrs.Length > 0 && IsSupported(field.FieldType, type))
                        Add(type, attrs[0].GetName(), field);
                }

                foreach (var property in type.GetProperties(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
                {
                    var attrs = (MaterialPropertyAttribute[])property.GetCustomAttributes(typeof(MaterialPropertyAttribute), false);

                    if (attrs.Length > 0 && IsSupported(property.PropertyType, type))
                        Add(type, attrs[0].GetName(), property);
                }
            }
        }

        /// <summary>
        /// Updates the <seealso cref="renderer"/>'s properties.
        /// </summary>
        protected override void UpdateProperties()
        {
            if (Renderers == null || Renderers.Length == 0)
                return;

            if (!AlwaysUseMaterialPropertyBlocks && SrpBatcherEnabled && Application.isPlaying)
            {
                if (_allFields.ContainsKey(type))
                    foreach (var field in _allFields[type])
                        materialDuplicates.SetProperty(field.Key, field.Value.GetValue(this));

                if (_allProperties.ContainsKey(type))
                    foreach (var property in _allProperties[type])
                        materialDuplicates.SetProperty(property.Key, property.Value.GetValue(this));
            }
            else
            {
                materialPropertyBlock.Clear();

                if (_allFields.ContainsKey(type))
                    foreach (var field in _allFields[type])
                        materialPropertyBlock.AddProperty(field.Key, field.Value.GetValue(this));

                if (_allProperties.ContainsKey(type))
                    foreach (var property in _allProperties[type])
                        materialPropertyBlock.AddProperty(property.Key, property.Value.GetValue(this));

                foreach(var renderer in Renderers)
                    if (renderer != null)
                        renderer.SetPropertyBlock(materialPropertyBlock);
            }
        }

        private static List<Type> _supportedTypes = new List<Type>()
        {
            typeof(bool),
            typeof(float),
            typeof(int),
            typeof(Color),
            typeof(Vector2),
            typeof(Vector3),
            typeof(Vector4),
            typeof(Matrix4x4),
            typeof(Texture),
            typeof(Texture2D),
            typeof(Texture3D),
            typeof(Cubemap),
            typeof(RenderTexture),
            typeof(float[]),
            //typeof(Color[]), // we cannot support Color Arrays as MaterialPropertyBlock doesn't have a Set Method for them.
            typeof(Vector4[]),
            typeof(Matrix4x4[]),
            typeof(GraphicsBuffer),
            typeof(ComputeBuffer)
        };

        private static bool IsSupported(Type type, Type declaringType = null)
        {
            if (_supportedTypes.Contains(type) || type.IsEnum)
            {
                return true;
            }
            else
            {
#if UNITY_EDITOR
                Debug.LogWarning($"{declaringType} : {type} is not supported as a ShaderProperty Type.", classSources[declaringType]);
#endif
                return false;
            }
        }
    }
}
