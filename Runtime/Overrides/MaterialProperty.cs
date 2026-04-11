using System;

namespace UnityEngine.MaterialPropertyProvider
{
    [Serializable]
    public class MaterialProperty<T> : IMaterialProperty
    {
        [SerializeField] string name;
        public string Name => name;

        [SerializeField]
        MaterialPropertyOverride<T> propertyOverride;

        public T value { get => propertyOverride.Value; set => propertyOverride.Value = value; }

        public object Value => propertyOverride.Enabled ? propertyOverride.Value : null;

        public MaterialProperty()
        {
            if (typeof(T).IsValueType)
                propertyOverride = (MaterialPropertyOverride<T>)Activator.CreateInstance(typeof(MaterialPropertyValueOverride<>).MakeGenericType(typeof(T)));
            else
                propertyOverride = (MaterialPropertyOverride<T>)Activator.CreateInstance(typeof(MaterialPropertyReferenceOverride<>).MakeGenericType(typeof(T)));
        }

        public MaterialProperty(string name)
        {
            this.name = name;
        }

        public MaterialProperty(string name, T value) : this(name)
        {
            this.value = value;
        }
    }

    [Serializable]
    public class MaterialPropertyRange : IMaterialProperty
    {
        [SerializeField] string name;
        public string Name => name;

        [SerializeField, RangeProperty(0f, 1f)]
        MaterialPropertyOverride<float> propertyOverride;
        public object Value => propertyOverride.Enabled ? propertyOverride.Value : null;

        public MaterialPropertyRange()
        {
            propertyOverride = new MaterialPropertyValueOverride<float>();
        }

        public MaterialPropertyRange(string name)
        {
            this.name = name;
        }
    }
}
