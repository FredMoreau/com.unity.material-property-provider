using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace UnityEngine.MaterialPropertyProvider
{
    public abstract class MaterialPropertyManager : MonoBehaviour
    {
        protected static bool SrpBatcherEnabled { get => GraphicsSettings.isScriptableRenderPipelineEnabled && GraphicsSettings.useScriptableRenderPipelineBatching; }

        /// <summary>
        /// Override to true to force using Material Property Blocks.
        /// </summary>
        protected virtual bool AlwaysUseMaterialPropertyBlocks => false;

        protected abstract Renderer[] Renderers { get; }
        private Dictionary<Material, Material> materials = new();
        protected List<Material> materialDuplicates = new List<Material>();
        internal static Action<Material> hasChanged;

        #region MaterialPropertyBlock
        protected MaterialPropertyBlock _materialPropertyBlock;
        protected MaterialPropertyBlock materialPropertyBlock
        {
            get
            {
                if (_materialPropertyBlock == null)
                    _materialPropertyBlock = new MaterialPropertyBlock();

                return _materialPropertyBlock;
            }
        }

        [ContextMenu("Reset Material Property Block")]
        protected void ResetMaterialPropertyBlock()
        {
            if (Renderers == null || Renderers.Length == 0)
                return;

            foreach (var renderer in Renderers)
                if (renderer != null)
                    renderer.SetPropertyBlock(null);
        }
        #endregion

        #region MonoBehaviour Messages
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

#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            if (enabled)
                UpdateProperties();
            else
                ResetMaterialPropertyBlock();
        }
#endif

        protected virtual void Reset()
        {
            ResetMaterialPropertyBlock();
            UpdateProperties();
        }

        protected virtual void OnDidApplyAnimationProperties()
        {
            UpdateProperties();
        }
        #endregion

        protected abstract void UpdateProperties();
        protected virtual void UpdateKeywords() { }

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
    }
}
