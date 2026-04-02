using System;
using System.Collections.Generic;

namespace UnityEngine.MaterialPropertyProvider
{
    [ExecuteAlways]
    public class MaterialProperties : MaterialPropertyManager
    {
        [SerializeField]
        Renderer[] _renderers;

        [SerializeReference]
        List<IMaterialProperty> materialProperties = new();

        Dictionary<int, IMaterialProperty> materialPropertyIds = new();

        protected override Renderer[] Renderers => _renderers;

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

        protected override void OnValidate()
        {
            Init();
            base.OnValidate();
        }

        protected override void Reset()
        {
            Init();
            base.Reset();
        }
#endif

        protected override void Awake()
        {
            Init();
            base.Awake();
        }

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

        protected override void UpdateProperties()
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
                            materialPropertyBlock.AddProperty(prop.Key, floatProp.Value);
                            break;
                        case MaterialProperty<Color> colorProp:
                            materialPropertyBlock.AddProperty(prop.Key, colorProp.Value);
                            break;
                        case MaterialProperty<Vector4> vectorProp:
                            materialPropertyBlock.AddProperty(prop.Key, vectorProp.Value);
                            break;
                        case MaterialProperty<Texture> textureProp:
                            materialPropertyBlock.AddProperty(prop.Key, textureProp.Value);
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
