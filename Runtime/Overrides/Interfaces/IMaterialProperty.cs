using System;
using UnityEngine.Rendering;

namespace UnityEngine.MaterialPropertyProvider
{
    internal interface IMaterialProperty
    {
        string Name { get; }

        object Value { get; }

        internal static IMaterialProperty FromPropertyType(ShaderPropertyType type, string name)
        {
            switch (type)
            {
                case ShaderPropertyType.Float:
                case ShaderPropertyType.Range: // TODO : add a specific override for range that also stores the min/max values for editor display
                    return new MaterialProperty<MaterialPropertyValueOverride<float>>(name);
                case ShaderPropertyType.Int:
                    return new MaterialProperty<MaterialPropertyValueOverride<int>>(name);
                case ShaderPropertyType.Color:
                    return new MaterialProperty<MaterialPropertyValueOverride<Color>>(name);
                case ShaderPropertyType.Vector:
                    return new MaterialProperty<MaterialPropertyValueOverride<Vector4>>(name);
                case ShaderPropertyType.Texture:
                    return new MaterialProperty<MaterialPropertyReferenceOverride<Texture>>(name);
                default:
                    throw new ArgumentException($"Unsupported property type {type}");
            }
        }
    }
}
