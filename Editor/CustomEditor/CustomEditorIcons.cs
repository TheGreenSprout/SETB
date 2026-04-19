#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

using SETB._CustomEditor.ScriptableObjects;
using static SETB.EditorHelpers;

namespace SETB._CustomEditor
{
    [InitializeOnLoad]
    public static class CustomEditorIcons
    {
        #region Main
        static CustomEditorIcons()
        {
            EditorApplication.projectChanged += ApplyIcons;
            AssemblyReloadEvents.afterAssemblyReload += ApplyIcons;
            EditorApplication.delayCall += ApplyIcons;
        }


        private static void ApplyIcons()
        {
            List<CustomEditorIconsAsset> instances = GetAllAssets<CustomEditorIconsAsset>();


            foreach (var instance in instances)
            {
                if (instance.data == null) continue;

                foreach (var entry in instance.data)
                {
                    ApplyIconToTypeAndChildren(entry.script, entry.icon);
                }
            }


            if (instances.Count > 0) EditorApplication.RepaintProjectWindow();
        }
        #endregion



        #region Logic
        private static void ApplyIconToTypeAndChildren(MonoScript script, Texture2D icon)
        {
            if (!script || !icon) return;


            var baseType = script.GetClass();
            if (baseType == null) return;


            foreach (var type in GetAllChildrenAndSelf(baseType))
            {
                ApplyIconToType(type, icon);
            }
        }
        

        private static void ApplyIconToType(System.Type baseType, Texture2D icon)
        {
            foreach (var asset in EnumerateAssets<Object>())
            {
                if (asset == null) continue;

                if (baseType.IsAssignableFrom(asset.GetType())) EditorGUIUtility.SetIconForObject(asset, icon);
            }
        }
        #endregion
    }
}
#endif
