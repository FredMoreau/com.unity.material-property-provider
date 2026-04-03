using System;

namespace UnityEngine.MaterialPropertyProvider
{
    [Serializable]
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
