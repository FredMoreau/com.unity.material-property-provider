using System;

namespace UnityEngine.MaterialPropertyProvider
{
    [Serializable]
    public class MaterialProperty<T> : IMaterialProperty where T : IMaterialPropertyOverride
    {
        [SerializeField] string name;
        public string Name => name;

        [SerializeField] T value;

        //public T Value { get => value; set => this.value = value; }

        public object Value => value;

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
}
