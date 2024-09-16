using UnityEngine;
using Unity.MaterialPropertyProvider;

[ExecuteAlways]
public class MaterialPropertyDriver : MaterialPropertyProviderBase
{
#pragma warning disable CS0414
    [SerializeField, ColorUsage(true, true), MaterialProperty("_BaseColor")] Color _color = Color.yellow;
    [SerializeField, Range(0f, 1f), MaterialProperty("_Smoothness")] float _smoothness = .5f;
#pragma warning restore CS0414

    private Renderer[] _renderers;
    protected override Renderer[] renderers
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
