using System;
using System.Collections.Generic;

namespace UnityEngine.MaterialPropertyProvider
{
    public static class MaterialExtensions
    {
        /// <summary>
        /// Notifies MaterialPropertyProviders of a change in source Material.
        /// </summary>
        /// <param name="material"></param>
        public static void HasChanged(this Material material)
        {
            MaterialPropertyProviderBase.hasChanged?.Invoke(material);
        }

        public static void SetProperty<V>(this List<Material> materials, int nameID, V value)
        {
            foreach (var material in materials)
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
                    case Enum e:
                        material.SetFloat(nameID, Convert.ToSingle(e));
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
                    case float[] fArray:
                        material.SetFloatArray(nameID, fArray);
                        break;
                    //case Color[] cArray:
                    //    material.SetColorArray(nameID, cArray);
                    //    break;
                    case Vector4[] v4Array:
                        material.SetVectorArray(nameID, v4Array);
                        break;
                    case Matrix4x4[] mArray:
                        material.SetMatrixArray(nameID, mArray);
                        break;
                    case GraphicsBuffer gBuffer:
                        material.SetBuffer(nameID, gBuffer);
                        break;
                    case ComputeBuffer cBuffer:
                        material.SetBuffer(nameID, cBuffer);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
