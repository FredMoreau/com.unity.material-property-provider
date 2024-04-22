# Material Property Provider
A _MonoBehaviour_ based class that will automatically set its _Renderer_ with a _MaterialPropertyBlock_ built from _fields_ and _properties_ marked with _**MaterialProperty**_ _Attribute_.

Derive from ```MaterialPropertyProviderBase``` and add ```[MaterialProperty("_Reference")]``` **Attributes** to _Fields_ and _Properties_ to add them to the _MaterialPropertyBlock_

Supported Types are: bool, float, int, Color, Vector2, Vector3, Vector4, Matrix4x4, Texture, Texture2D, Texture3D, Cubemap, RenderTexture.

Any other type will just be discarded.

## Example
```cs
public class RadialGradientPropertyProvider : MaterialPropertyProviderBase
{
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

    // this allows for animated properties
    //private void Update()
    //{
    //    UpdatePropertyBlock();
    //}
}
```
The component will automatically set the fields' and properties' values to the Renderer's Material Property Block.

![alt text](Documentation~/images/radial-gradient-sample.png)

