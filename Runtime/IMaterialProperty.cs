using System;
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
                    return new MaterialProperty<float>(name);
                case ShaderPropertyType.Range:
                    return new FloatRangeProperty(name);
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
}
