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
                    if (!entry.script || !entry.icon) continue;

                    EditorGUIUtility.SetIconForObject(entry.script, entry.icon);
                }
            }


            if (instances.Count > 0) EditorApplication.RepaintProjectWindow();
        }
        #endregion
    }
}
#endif
