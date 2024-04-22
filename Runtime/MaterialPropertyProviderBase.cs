using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Unity.MaterialPropertyProvider
{
    /// <summary>
    /// A <see cref="MonoBehaviour"/> based class that will automatically set its <see cref="Renderer"/> with a <see cref="MaterialPropertyBlock"/>
    /// <para>When deriving from <see cref="MaterialPropertyProviderBase"/>, fields and properties with <see cref="MaterialPropertyAttribute"/> will automatically be set on the <see cref="Renderer"/>.</para>
    /// <para>Properties are automatically updated upon Awake(), Start(), Reset() and OnValidate(). To enable animated properties, or force update, call <seealso cref="UpdatePropertyBlock"/>.</para>
    /// </summary>
    [RequireComponent(typeof(Renderer))]
    public abstract class MaterialPropertyProviderBase : MonoBehaviour
    {
        //private static Dictionary<Type, Dictionary<string, FieldInfo>> _allFields = new();
        //private static Dictionary<Type, Dictionary<string, PropertyInfo>> _allProperties = new();

        private static Dictionary<Type, Dictionary<int, FieldInfo>> _allFields = new();
        private static Dictionary<Type, Dictionary<int, PropertyInfo>> _allProperties = new();

        private MaterialPropertyBlock _materialPropertyBlock;
        private Renderer _renderer;

        protected new Renderer renderer
        {
            get
            {
                if (_renderer == null)
                    _renderer = GetComponent<Renderer>();

                return _renderer;
            }
        }

        private MaterialPropertyBlock materialPropertyBlock
        {
            get
            {
                if (_materialPropertyBlock == null)
                    _materialPropertyBlock = new MaterialPropertyBlock();

                return _materialPropertyBlock;
            }
        }

        private static void Add(Type type, string name, FieldInfo fieldInfo)
        {
            //if (!_allFields.ContainsKey(type))
            //    _allFields.Add(type, new Dictionary<string, FieldInfo>());

            //if (!_allFields[type].ContainsKey(name))
            //    _allFields[type].Add(name, fieldInfo);

            int nameID = Shader.PropertyToID(name);

            if (!_allFields.ContainsKey(type))
                _allFields.Add(type, new Dictionary<int, FieldInfo>());

            if (!_allFields[type].ContainsKey(nameID))
                _allFields[type].Add(nameID, fieldInfo);
        }

        private static void Add(Type type, string name, PropertyInfo propertyInfo)
        {
            //if (!_allProperties.ContainsKey(type))
            //    _allProperties.Add(type, new Dictionary<string, PropertyInfo>());

            //if (!_allProperties[type].ContainsKey(name))
            //    _allProperties[type].Add(name, propertyInfo);

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

        protected virtual void Awake()
        {
            UpdatePropertyBlock();
        }

        protected virtual void Start()
        {
            UpdatePropertyBlock();
        }

        protected virtual void Reset()
        {
            UpdatePropertyBlock();
        }

        protected virtual void OnValidate()
        {
            UpdatePropertyBlock();
        }

        /// <summary>
        /// Updates the <seealso cref="renderer"/>'s <see cref="MaterialPropertyBlock"/>
        /// </summary>
        protected void UpdatePropertyBlock()
        {
            if (_allFields.ContainsKey(GetType()))
                foreach (var field in _allFields[GetType()])
                    AddToMaterialPropertyBlock(field.Key, field.Value.GetValue(this));

            if (_allProperties.ContainsKey(GetType()))
                foreach (var property in _allProperties[GetType()])
                    AddToMaterialPropertyBlock(property.Key, property.Value.GetValue(this));

            renderer.SetPropertyBlock(materialPropertyBlock);
        }

        private void AddToMaterialPropertyBlock<V>(string name, V value)
        {
            switch (value)
            {
                case bool b:
                    materialPropertyBlock.SetFloat(name, b ? 1 : 0);
                    break;
                case float f:
                    materialPropertyBlock.SetFloat(name, f);
                    break;
                case int i:
                    materialPropertyBlock.SetInteger(name, i);
                    break;
                case Color c:
                    materialPropertyBlock.SetColor(name, c);
                    break;
                case Vector2 v:
                    materialPropertyBlock.SetVector(name, v);
                    break;
                case Vector3 v:
                    materialPropertyBlock.SetVector(name, v);
                    break;
                case Vector4 v:
                    materialPropertyBlock.SetVector(name, v);
                    break;
                case Matrix4x4 m:
                    materialPropertyBlock.SetMatrix(name, m);
                    break;
                case Texture t:
                    if (t != null)
                        materialPropertyBlock.SetTexture(name, t);
                    break;
                default:
                    break;
            }
        }

        private void AddToMaterialPropertyBlock<V>(int nameID, V value)
        {
            switch (value)
            {
                case bool b:
                    materialPropertyBlock.SetFloat(nameID, b ? 1 : 0);
                    break;
                case float f:
                    materialPropertyBlock.SetFloat(nameID, f);
                    break;
                case int i:
                    materialPropertyBlock.SetInteger(nameID, i);
                    break;
                case Color c:
                    materialPropertyBlock.SetColor(nameID, c);
                    break;
                case Vector2 v:
                    materialPropertyBlock.SetVector(nameID, v);
                    break;
                case Vector3 v:
                    materialPropertyBlock.SetVector(nameID, v);
                    break;
                case Vector4 v:
                    materialPropertyBlock.SetVector(nameID, v);
                    break;
                case Matrix4x4 m:
                    materialPropertyBlock.SetMatrix(nameID, m);
                    break;
                case RenderTexture rt:
                    if (rt != null)
                        materialPropertyBlock.SetTexture(nameID, rt);
                    break;
                case Texture t:
                    if (t != null)
                        materialPropertyBlock.SetTexture(nameID, t);
                    break;
                default:
                    break;
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
        typeof(RenderTexture)
    };

        private static bool IsSupported(Type type, Type declaringType = null)
        {
            if (_supportedTypes.Contains(type))
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
