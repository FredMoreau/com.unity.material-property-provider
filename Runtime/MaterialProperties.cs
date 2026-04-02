using System;
using System.Collections.Generic;
using UnityEngine.Rendering;

namespace UnityEngine.MaterialPropertyProvider
{
    internal interface IMaterialProperty
    {
        string Name { get; }
        internal static IMaterialProperty FromPropertyType(ShaderPropertyType type, string name)
        {
            switch (type)
            {
                case ShaderPropertyType.Float:
                case ShaderPropertyType.Range:
                    return new MaterialProperty<float>(name);
                case ShaderPropertyType.Int:
                    return new MaterialProperty<float>(name);
                case ShaderPropertyType.Color:
                    return new MaterialProperty<Color>(name);
                case ShaderPropertyType.Vector:
                    return new MaterialProperty<Vector4>(name);
                case ShaderPropertyType.Texture:
                    return new MaterialProperty<Texture>(name);
                default:
                    throw new ArgumentException($"Unsupported property type {type}");
            }
        }
    }

    public class MaterialProperty<T> : IMaterialProperty
    {
        [SerializeField] string name;
        public string Name => name;

        [SerializeField] T value;
        public T Value { get => value; set => this.value = value; }

        public MaterialProperty(string name)
        {
            this.name = name;
        }
    }

    // TODO mutualise with MaterialPropertyProviderBase
    [ExecuteAlways]
    public class MaterialProperties : MonoBehaviour
    {
        static bool SrpBatcherEnabled { get => GraphicsSettings.isScriptableRenderPipelineEnabled && GraphicsSettings.useScriptableRenderPipelineBatching; }
        const bool AlwaysUseMaterialPropertyBlocks = false;

        [SerializeField]
        Renderer[] _renderers;

        [SerializeReference]
        List<IMaterialProperty> materialProperties = new();

        Dictionary<int, IMaterialProperty> materialPropertyIds = new();

        Renderer[] Renderers => _renderers;
        private Dictionary<Material, Material> materials = new();
        private List<Material> materialDuplicates = new List<Material>();
        internal static Action<Material> hasChanged;

        internal void Add(IMaterialProperty property)
        {
            if (!materialProperties.Contains(property))
            {
                materialProperties.Add(property);
                var id = Shader.PropertyToID(property.Name);
                if (!materialPropertyIds.ContainsKey(id))
                    materialPropertyIds.Add(id, property);
                UpdateProperties();
            }
        }

        #region MaterialPropertyBlock
        private MaterialPropertyBlock _materialPropertyBlock;
        private MaterialPropertyBlock materialPropertyBlock
        {
            get
            {
                if (_materialPropertyBlock == null)
                    _materialPropertyBlock = new MaterialPropertyBlock();

                return _materialPropertyBlock;
            }
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
        #endregion

#if UNITY_EDITOR
        [ContextMenu("Scan")]
        void Scan()
        {
            materialProperties.Clear();
            foreach (var renderer in _renderers)
            {
                foreach (var material in renderer.sharedMaterials)
                {
                    var props = new IMaterialProperty[material.shader.GetPropertyCount()];
                    for (int i = 0; i < props.Length; i++)
                    {
                        props[i] = IMaterialProperty.FromPropertyType(material.shader.GetPropertyType(i), material.shader.GetPropertyName(i));
                    }
                    materialProperties.AddRange(props);
                }
            }
        }

        private void OnValidate()
        {
            Init();
            if (enabled)
                UpdateProperties();
            else
                ResetMaterialPropertyBlock();
        }
#endif

        void Init()
        {
            materialPropertyIds.Clear();
            foreach (var prop in materialProperties)
            {
                var id = Shader.PropertyToID(prop.Name);
                if (!materialPropertyIds.ContainsKey(id))
                    materialPropertyIds.Add(id, prop);
            }
        }

        #region MonoBehaviour Messages
        protected virtual void Awake()
        {
            Init();
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
            hasChanged += UpdateFromSourceMaterial;
        }

        protected virtual void OnDisable()
        {
            ResetMaterialPropertyBlock();
            hasChanged -= UpdateFromSourceMaterial;
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
            Init();
            ResetMaterialPropertyBlock();
            UpdateProperties();
        }

        protected virtual void OnDidApplyAnimationProperties()
        {
            UpdateProperties();
        }
        #endregion

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

        protected void Revert(Material material)
        {
            if (materials.ContainsKey(material))
                materials[material].CopyPropertiesFromMaterial(material);
        }

        protected void RevertAllMaterials()
        {
            foreach (KeyValuePair<Material, Material> kvp in materials)
                kvp.Value.CopyPropertiesFromMaterial(kvp.Key);
        }

        void UpdateFromSourceMaterial(Material material)
        {
            Revert(material);
            UpdateProperties();
        }

        void UpdateProperties()
        {
            if (Renderers == null || Renderers.Length == 0)
                return;

            if (!AlwaysUseMaterialPropertyBlocks && SrpBatcherEnabled && Application.isPlaying)
            {
                foreach (var prop in materialPropertyIds)
                {
                    switch (prop.Value)
                    {
                        case MaterialProperty<float> floatProp:
                            materialDuplicates.SetProperty(prop.Key, floatProp.Value);
                            break;
                        case MaterialProperty<Color> colorProp:
                            materialDuplicates.SetProperty(prop.Key, colorProp.Value);
                            break;
                        case MaterialProperty<Vector4> vectorProp:
                            materialDuplicates.SetProperty(prop.Key, vectorProp.Value);
                            break;
                        case MaterialProperty<Texture> textureProp:
                            materialDuplicates.SetProperty(prop.Key, textureProp.Value);
                            break;
                    }
                }
            }
            else
            {
                this.materialPropertyBlock.Clear();

                foreach (var prop in materialPropertyIds)
                {
                    switch (prop.Value)
                    {
                        case MaterialProperty<float> floatProp:
                            materialPropertyBlock.SetFloat(prop.Key, floatProp.Value);
                            break;
                        case MaterialProperty<Color> colorProp:
                            materialPropertyBlock.SetColor(prop.Key, colorProp.Value);
                            break;
                        case MaterialProperty<Vector4> vectorProp:
                            materialPropertyBlock.SetVector(prop.Key, vectorProp.Value);
                            break;
                        case MaterialProperty<Texture> textureProp:
                            materialPropertyBlock.SetTexture(prop.Key, textureProp.Value);
                            break;
                    }
                }

                foreach (var renderer in Renderers)
                    if (renderer != null)
                        renderer.SetPropertyBlock(materialPropertyBlock);
            }
        }
    }
}
