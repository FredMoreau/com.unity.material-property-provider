using UnityEngine;

namespace UnityEngine.MaterialPropertyProvider
{
    public class RangePropertyAttribute : PropertyAttribute
    {
        public float min;
        public float max;
        public RangePropertyAttribute(float min, float max)
        {
            this.min = min;
            this.max = max;
        }
    }
}
