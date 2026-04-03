
using System;

namespace UnityEngine.MaterialPropertyProvider
{
    [Serializable]
    public abstract class MaterialKeyword<T> : IMaterialKeyword
    {
        [SerializeField] string name;
        public string Name => name;

        [SerializeField] T value;
        public T Value { get => value; set => this.value = value; }

        public MaterialKeyword() { }

        public MaterialKeyword(string name)
        {
            this.name = name;
        }

        public MaterialKeyword(string name, T value) : this(name)
        {
            this.value = value;
        }
    }

    [Serializable]
    public class MaterialKeyword : MaterialKeyword<bool>
    {
        public MaterialKeyword() { }
        public MaterialKeyword(string name) : base(name) { }
        public MaterialKeyword(string name, bool value) : base(name, value) { }
    }
}
