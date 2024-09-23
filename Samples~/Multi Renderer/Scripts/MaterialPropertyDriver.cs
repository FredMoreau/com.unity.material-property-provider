using UnityEngine;
using Unity.MaterialPropertyProvider;

[ExecuteAlways] // this allows for animated properties preview in Timeline when in Edit Mode
public class MaterialPropertyDriver : MaterialPropertyProviderBase
{
    // Add the [MaterialProperty("_Reference")] Attribute to Fields and Properties
    // to add them to the MaterialPropertyBlock
    // Supported Types are:
    // bool, float, int
    // Color, Vector2, Vector3, Vector4, Matrix4x4
    // Texture, Texture2D, Texture3D, Cubemap, RenderTexture
    // Any other type will just be discarded.

#pragma warning disable CS0414
    [SerializeField, ColorUsage(true, true), MaterialProperty("_BaseColor")] Color _color = Color.yellow;
    [SerializeField, Range(0f, 1f), MaterialProperty("_Smoothness")] float _smoothness = .5f;
#pragma warning restore CS0414

    private Renderer[] _renderers;
    protected override Renderer[] Renderers // <-- passing the renderers to work with
    {
        get
        {
            if (_renderers == null)
            {
                _renderers = GetComponentsInChildren<Renderer>();
            }
            return _renderers;
        }
    }
}
