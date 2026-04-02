using System;

namespace UnityEngine.MaterialPropertyProvider
{
    [Serializable]
    public class MaterialProperty<T> : IMaterialProperty
    {
        [SerializeField] string name;
        public string Name => name;

        [SerializeField] T value;
        public T Value { get => value; set => this.value = value; }

        public MaterialProperty() { }

        public MaterialProperty(string name)
        {
            this.name = name;
        }

        public MaterialProperty(string name, T value) : this(name)
        {
            this.value = value;
        }
    }

    public class FloatRangeProperty : MaterialProperty<float>
    {
        [SerializeField] float minValue = 0;
        [SerializeField] float maxValue = 1;

        public FloatRangeProperty() { }

        public FloatRangeProperty(string name) : base(name)
        {

        }

        public FloatRangeProperty(string name, float value) : base(name, value)
        {

        }
    }
}
