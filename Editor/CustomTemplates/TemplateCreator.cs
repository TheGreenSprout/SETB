using UnityEngine;
using UnityEditor;
using System.IO;

namespace SETB.CustomTemplates
{
    public static class TemplateCreator
    {
        #region Internal Logic
        private static void CreateScriptFromTemplate(string templateName, string defaultFileName)
        {
            string template = LoadTemplate(templateName);

            if (string.IsNullOrEmpty(template)) return;


            string folder = GetSelectedPathOrFallback();
            string filePath = AssetDatabase.GenerateUniqueAssetPath(
                Path.Combine(folder, defaultFileName)
            );

            template = template.Replace("#SCRIPTNAME#", Path.GetFileNameWithoutExtension(filePath));

            File.WriteAllText(filePath, template);


            AssetDatabase.Refresh();
        }


        private static string LoadTemplate(string templateName)
        {
            string packagePath = $"Packages/com.sproutinggames.sprouts.etb/Editor/CustomTemplates/Resources/SETB_Templates/{templateName}.txt";
            var templateAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(packagePath);

            if (templateAsset != null) return templateAsset.text;


            var resourceAsset = Resources.Load<TextAsset>("SETB_Templates/" + templateName);

            if (resourceAsset != null) return resourceAsset.text;


            Debug.LogError($"Template not found in UPM or Resources: {templateName}");
            return null;
        }

        private static string GetSelectedPathOrFallback()
        {
            string path = "Assets";

            foreach (var obj in Selection.GetFiltered(typeof(Object), SelectionMode.Assets))
            {
                path = AssetDatabase.GetAssetPath(obj);

                if (File.Exists(path)) path = Path.GetDirectoryName(path);

                break;
            }


            return path;
        }
        #endregion



        #region Template Creation Methods
        [MenuItem("Assets/Create/Scripting/SETB/EditorWindow Script", false, 80)]
        public static void Create_EditorWindowScript() => CreateScriptFromTemplate("EditorWindow_ScriptTemplate", "NewEditorWindowScript.cs");


        [MenuItem("Assets/Create/Scripting/SETB/Editor Script", false, 80)]
        public static void Create_EditorScript() => CreateScriptFromTemplate("Editor_ScriptTemplate", "NewEditorScript.cs");


        [MenuItem("Assets/Create/Scripting/SETB/PropertyDrawer Script", false, 80)]
        public static void Create_PropertyDrawerScript() => CreateScriptFromTemplate("PropertyDrawer_ScriptTemplate", "NewPropertyDrawerScript.cs");
        #endregion
    }
}
