using System;
using UnityEngine;

namespace UnityEngine.MaterialPropertyProvider
{
    public static class MaterialPropertyBlockExtensions
    {
        public static void AddProperty<V>(this MaterialPropertyBlock materialPropertyBlock, int nameID, V value)
        {
            //if (value is IMaterialPropertyOverride overrideValue)
            //{
            //    if (!overrideValue.Enabled)
            //        return;
            //    value = (V)Convert.ChangeType(overrideValue, overrideValue.ValueType);
            //}

            switch (value)
            {
                //case MaterialPropertyOverride<bool> overrideBool:
                //    if (overrideBool.Enabled)
                //        materialPropertyBlock.SetFloat(nameID, overrideBool.Value ? 1 : 0);
                //    break;
                //case MaterialPropertyOverride<float> overrideFloat:
                //    if (overrideFloat.Enabled)
                //        materialPropertyBlock.SetFloat(nameID, overrideFloat.Value);
                //    break;
                //case MaterialPropertyOverride<int> overrideInt:
                //    if (overrideInt.Enabled)
                //        materialPropertyBlock.SetInteger(nameID, overrideInt.Value);
                //    break;
                ////case MaterialPropertyOverride<Enum> overrideEnum: // knowing the enum type would require reflection, which is not ideal for performance.
                ////    if (overrideEnum.HasValue)
                ////    materialPropertyBlock.SetFloat(nameID, Convert.ToSingle(overrideEnum.Value));
                ////    break;
                //case MaterialPropertyOverride<Color> overrideColor:
                //    if (overrideColor.Enabled)
                //        materialPropertyBlock.SetColor(nameID, overrideColor.Value);
                //    break;
                //case MaterialPropertyOverride<Vector2> overrideVector2:
                //    if (overrideVector2.Enabled)
                //        materialPropertyBlock.SetVector(nameID, overrideVector2.Value);
                //    break;
                //case MaterialPropertyOverride<Vector3> overrideVector3:
                //    if (overrideVector3.Enabled)
                //        materialPropertyBlock.SetVector(nameID, overrideVector3.Value);
                //    break;
                //case MaterialPropertyOverride<Vector4> overrideVector4:
                //    if (overrideVector4.Enabled)
                //        materialPropertyBlock.SetVector(nameID, overrideVector4.Value);
                //    break;
                case bool b:
                    materialPropertyBlock.SetFloat(nameID, b ? 1 : 0);
                    break;
                case float f:
                    materialPropertyBlock.SetFloat(nameID, f);
                    break;
                case int i:
                    materialPropertyBlock.SetInteger(nameID, i);
                    break;
                case Enum e:
                    materialPropertyBlock.SetFloat(nameID, Convert.ToSingle(e));
                    break;
                case Color c:
                    materialPropertyBlock.SetColor(nameID, c);
                    break;
                case Vector2 v:
                    materialPropertyBlock.SetVector(nameID, v);
                    break;
                case Vector3 v:
                    materialPropertyBlock.SetVector(nameID, v);
                    break;
                case Vector4 v:
                    materialPropertyBlock.SetVector(nameID, v);
                    break;
                case Matrix4x4 m:
                    materialPropertyBlock.SetMatrix(nameID, m);
                    break;
                case RenderTexture rt:
                    if (rt != null)
                        materialPropertyBlock.SetTexture(nameID, rt);
                    break;
                case Texture t:
                    if (t != null)
                        materialPropertyBlock.SetTexture(nameID, t);
                    break;
                case float[] fArray:
                    materialPropertyBlock.SetFloatArray(nameID, fArray);
                    break;
                //case Color[] cArray:
                //    materialPropertyBlock.SetColorArray(nameID, cArray);
                //    break;
                case Vector4[] v4Array:
                    materialPropertyBlock.SetVectorArray(nameID, v4Array);
                    break;
                case Matrix4x4[] mArray:
                    materialPropertyBlock.SetMatrixArray(nameID, mArray);
                    break;
                case GraphicsBuffer gBuffer:
                    materialPropertyBlock.SetBuffer(nameID, gBuffer);
                    break;
                case ComputeBuffer cBuffer:
                    materialPropertyBlock.SetBuffer(nameID, cBuffer);
                    break;
                default:
                    break;
            }
        }
    }
}
