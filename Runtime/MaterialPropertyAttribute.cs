using System;

namespace Unity.MaterialPropertyProvider
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
}
