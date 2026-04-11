using System;

//#nullable enable

namespace UnityEngine.MaterialPropertyProvider
{
    public interface IMaterialPropertyOverride
    {
        Type ValueType { get; }
        bool Enabled { get; }
        object GetValue();
    }

    [Serializable]
    public class MaterialPropertyOverride<T> : IMaterialPropertyOverride
    {
        public static string valueFieldName => nameof(value);
        public static string enabledFieldName => nameof(enabled);

        [SerializeField]
        protected T value;

        [SerializeField]
        protected bool enabled;

        public T Value { get => value; set => this.value = value; }

        public object GetValue() => value;

        public bool Enabled { get => enabled; set => enabled = value; }

        public Type ValueType => typeof(T);

        public MaterialPropertyOverride()
        {
            this.value = default;
            this.enabled = false;
        }

        public MaterialPropertyOverride(T value = default) : this()
        {
            this.value = value;
        }

        public MaterialPropertyOverride(T value = default, bool enabled = true) : this(value)
        {
            this.enabled = enabled;
        }
    }
    
    [Serializable]
    public class MaterialPropertyValueOverride<T> : MaterialPropertyOverride<T> where T : struct
    {
        public MaterialPropertyValueOverride() : base() {}

        public MaterialPropertyValueOverride(T value = default) : base(value) {}

        public static implicit operator MaterialPropertyValueOverride<T>(T value) => new MaterialPropertyValueOverride<T>(value);
        public static implicit operator T?(MaterialPropertyValueOverride<T> value) => value.Enabled ? value.Value : (T?)null;
    }

    [Serializable]
    public class MaterialPropertyReferenceOverride<T> : MaterialPropertyOverride<T> where T : class
    {
        public MaterialPropertyReferenceOverride() : base() {}

        public MaterialPropertyReferenceOverride(T value = default) : base(value) {}

        public static implicit operator MaterialPropertyReferenceOverride<T>(T value) => new MaterialPropertyReferenceOverride<T>(value);
        public static implicit operator T(MaterialPropertyReferenceOverride<T> value) => value.Enabled ? value.Value : null;
    }
}
