using UnityEngine;

namespace UnityEngine.MaterialPropertyProvider
{
    public static class MaterialExtensions
    {
        /// <summary>
        /// Notifies MaterialPropertyProviders of a change in source Material.
        /// </summary>
        /// <param name="material"></param>
        public static void HasChanged(this Material material)
        {
            MaterialPropertyProviderBase.hasChanged?.Invoke(material);
        }
    }
}
