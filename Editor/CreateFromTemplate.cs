using UnityEngine;
using UnityEditor;

namespace Unity.MaterialPropertyProvider.Editor
{
    public static class CreateFromTemplate
    {
        [MenuItem("Assets/Create/Scripting/Material Property Provider")]
        private static void CreateExample()
        {
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile("Packages/com.unity.material-property-provider/Editor/example.txt", "NewMaterialPropertyProvider.cs");
        }
    }
}
