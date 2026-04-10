using UnityEngine;

namespace UnityEngine.MaterialPropertyProvider
{
    public class ColorUsagePropertyAttribute : PropertyAttribute
    {
        public readonly bool showAlpha = true;
        public readonly bool hdr = false;

        public ColorUsagePropertyAttribute(bool showAlpha = false, bool hdr = false)
        {
            this.showAlpha = showAlpha;
            this.hdr = hdr;
        }
    }
}
