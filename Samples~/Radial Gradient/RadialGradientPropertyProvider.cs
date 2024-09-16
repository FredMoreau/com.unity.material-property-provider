using UnityEngine;

namespace Unity.MaterialPropertyProvider.Samples
{
    [ExecuteAlways] // this allows for animated properties preview in Timeline when in Edit Mode
    [RequireComponent(typeof(Renderer))]
    public class RadialGradientPropertyProvider : MaterialPropertyProviderBase
    {
        // Add the [MaterialProperty("_Reference")] Attribute to Fields and Properties
        // to add them to the MaterialPropertyBlock
        // Supported Types are:
        // bool, float, int
        // Color, Vector2, Vector3, Vector4, Matrix4x4
        // Texture, Texture2D, Texture3D, Cubemap, RenderTexture
        // Any other type will just be discarded.

#pragma warning disable CS0414
        [SerializeField, ColorUsage(true, true), MaterialProperty("_ColorA")] Color _colorA = Color.yellow;
        [SerializeField, ColorUsage(true, true), MaterialProperty("_ColorB")] Color _colorB = Color.cyan;

        [SerializeField, MaterialProperty("_Center")] Vector2 _center;
        [SerializeField, MaterialProperty("_Radius")] float _radius = .5f;

        [SerializeField, Range(.1f, 5f), MaterialProperty("_Power")] float _power = 1f;

        [SerializeField] bool _useTexture;

        [SerializeField, MaterialProperty("_Texture")] Texture _texture;
#pragma warning restore CS0414

        [MaterialProperty("_UseTexture")]
        public bool UseTexture { get => _useTexture && _texture != null; }

        Renderer[] _renderers;
        protected override Renderer[] renderers
        {
            get
            {
                if (_renderers == null)
                {
                    _renderers = new Renderer[1] { GetComponent<Renderer>() };
                }
                return _renderers;
            }
        }
    }
}
