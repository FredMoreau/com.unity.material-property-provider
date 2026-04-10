using System;

namespace UnityEngine.MaterialPropertyProvider
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class MaterialPropertyAttribute : Attribute
    {
        private string Name;

        public MaterialPropertyAttribute(string name)
        {
            Name = name;
        }

        public string GetName() => Name;
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class MaterialKeywordAttribute : Attribute
    {
        private string Name;

        public MaterialKeywordAttribute(string name)
        {
            Name = name;
        }

        public string GetName() => Name;
    }
}
