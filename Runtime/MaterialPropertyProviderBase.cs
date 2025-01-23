using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Unity.MaterialPropertyProvider
{
    /// <summary>
    /// A <see cref="MonoBehaviour"/> based class that will automatically set its <see cref="Renderer"/> with a <see cref="MaterialPropertyBlock"/>
    /// <para>When deriving from <see cref="MaterialPropertyProviderBase"/>, fields and properties with <see cref="MaterialPropertyAttribute"/> will automatically be set on the <see cref="Renderer"/>.</para>
    /// <para>Properties are automatically updated upon Awake(), Start(), Reset() and OnValidate(). To enable animated properties, or force update, call <seealso cref="UpdateProperties"/>.</para>
    /// </summary>
    public abstract class MaterialPropertyProviderBase : MonoBehaviour
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

        private MaterialPropertyBlock _materialPropertyBlock;

        static bool SrpBatcherEnabled { get => GraphicsSettings.isScriptableRenderPipelineEnabled && GraphicsSettings.useScriptableRenderPipelineBatching; }

        /// <summary>
        /// Override to true to force using Material Property Blocks.
        /// </summary>
        protected virtual bool AlwaysUseMaterialPropertyBlocks => false;

        protected abstract Renderer[] Renderers { get; }

        private Dictionary<Material, Material> materials = new();
        private List<Material> materialDuplicates = new List<Material>();

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

        protected virtual void Awake()
        {
            UpdateProperties();
        }

        protected virtual void OnDestroy()
        {
            ResetMaterialPropertyBlock();
        }

        protected virtual void OnEnable()
        {
            ResetMaterialPropertyBlock();
            UpdateProperties();
        }

        protected virtual void OnDisable()
        {
            ResetMaterialPropertyBlock();
        }

        protected virtual void Start()
        {
            if ((!Application.isEditor || Application.isPlaying) && SrpBatcherEnabled && !AlwaysUseMaterialPropertyBlocks)
            {
                ResetMaterialPropertyBlock();
                MakeMaterialsUnique();
            }
            UpdateProperties();
        }

        protected virtual void Reset()
        {
            ResetMaterialPropertyBlock();
            UpdateProperties();
        }

        protected virtual void OnValidate()
        {
            if (enabled)
                UpdateProperties();
            else
                ResetMaterialPropertyBlock();
        }

        protected virtual void OnDidApplyAnimationProperties()
        {
            UpdateProperties();
        }

        [ContextMenu("Reset Material Property Block")]
        private void ResetMaterialPropertyBlock()
        {
            if (Renderers == null || Renderers.Length == 0)
                return;

            foreach (var renderer in Renderers)
                if (renderer != null)
                    renderer.SetPropertyBlock(null);
        }

        private void MakeMaterialsUnique()
        {
            if (!Application.isPlaying)
                return;

            materials.Clear();
            materialDuplicates.Clear();
            foreach (var renderer in Renderers)
            {
                if (renderer.sharedMaterials.Length > 0)
                {
                    var rendererMaterials = renderer.sharedMaterials;

                    for (int i = 0; i < rendererMaterials.Length; i++)
                    {
                        if (rendererMaterials[i] is null)
                            continue;
                        if (materials.ContainsKey(rendererMaterials[i]))
                        {
                            rendererMaterials[i] = materials[rendererMaterials[i]];
                        }
                        else
                        {
                            var duplicate = new Material(rendererMaterials[i]);
                            materialDuplicates.Add(duplicate);
                            materials.Add(rendererMaterials[i], duplicate);
                            rendererMaterials[i] = duplicate;
                        }
                    }

                    renderer.sharedMaterials = rendererMaterials;
                }
            }
        }

        /// <summary>
        /// Updates the <seealso cref="renderer"/>'s properties.
        /// </summary>
        protected void UpdateProperties()
        {
            if (Renderers == null || Renderers.Length == 0)
                return;

            if (!AlwaysUseMaterialPropertyBlocks && SrpBatcherEnabled && Application.isPlaying)
            {
                if (_allFields.ContainsKey(type))
                    foreach (var field in _allFields[type])
                        SetMaterialProperty(field.Key, field.Value.GetValue(this));

                if (_allProperties.ContainsKey(type))
                    foreach (var property in _allProperties[type])
                        SetMaterialProperty(property.Key, property.Value.GetValue(this));
            }
            else
            {
                materialPropertyBlock.Clear();

                if (_allFields.ContainsKey(type))
                    foreach (var field in _allFields[type])
                        AddToMaterialPropertyBlock(field.Key, field.Value.GetValue(this));

                if (_allProperties.ContainsKey(type))
                    foreach (var property in _allProperties[type])
                        AddToMaterialPropertyBlock(property.Key, property.Value.GetValue(this));

                foreach(var renderer in Renderers)
                    if (renderer != null)
                        renderer.SetPropertyBlock(materialPropertyBlock);
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

        private void SetMaterialProperty<V>(int nameID, V value)
        {
            foreach (var material in materialDuplicates)
            {
                switch (value)
                {
                    case bool b:
                        material.SetFloat(nameID, b ? 1 : 0);
                        break;
                    case float f:
                        material.SetFloat(nameID, f);
                        break;
                    case int i:
                        material.SetInteger(nameID, i);
                        break;
                    case Color c:
                        material.SetColor(nameID, c);
                        break;
                    case Vector2 v:
                        material.SetVector(nameID, v);
                        break;
                    case Vector3 v:
                        material.SetVector(nameID, v);
                        break;
                    case Vector4 v:
                        material.SetVector(nameID, v);
                        break;
                    case Matrix4x4 m:
                        material.SetMatrix(nameID, m);
                        break;
                    case RenderTexture rt:
                        material.SetTexture(nameID, rt);
                        break;
                    case Texture t:
                        material.SetTexture(nameID, t);
                        break;
                    default:
                        break;
                }
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
