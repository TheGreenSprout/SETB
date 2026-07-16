#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEditor.ProjectWindowCallback;

using static SETB.EditorHelpers;

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
            string path = Path.Combine(folder, defaultFileName);

            var endNameEdit = ScriptableObject.CreateInstance<CreateTemplateScriptAction>();
            endNameEdit.templateText = template;

            var icon = EditorGUIUtility.IconContent("cs Script Icon").image as Texture2D;

            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(
                0,
                endNameEdit,
                path,
                icon,
                null
            );
        }

        private static string LoadTemplate(string templateName)
        {
            var assets = GetAllAssets<TextAsset>(templateName);
            if (assets == null || assets.Count == 0 || assets[0] == null)
            {
                Debug.LogError($"Template not found: {templateName}");

                return null;
            }

            return assets[0].text;
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


        #region Namespace Logic
        public static string GetRootNamespace(string folderPath)
        {
            while (!string.IsNullOrEmpty(folderPath))
            {
                var guids = AssetDatabase.FindAssets("t:asmdef", new[] { folderPath });

                if (guids.Length > 0)
                {
                    string asmdefPath = AssetDatabase.GUIDToAssetPath(guids[0]);
                    string json = File.ReadAllText(asmdefPath);

                    var data = JsonUtility.FromJson<AssemblyDefinitionData>(json);

                    if (!string.IsNullOrEmpty(data.rootNamespace)) return data.rootNamespace;

                    return data.name;
                }

                folderPath = Path.GetDirectoryName(folderPath);
            }

            return null;
        }

        
        public static string Indent(string text, string indent = "\t")
        {
            var lines = text.Split('\n');

            for (int i = 0; i < lines.Length; i++)
            {
                if (!string.IsNullOrWhiteSpace(lines[i])) lines[i] = indent + lines[i];
            }

            return string.Join("\n", lines);
        }
        #endregion



        #region Template Creation Methods
        [MenuItem("Assets/Create/SETB/Scripting/EditorWindow Script", false, 80)]
        public static void Create_EditorWindowScript() => CreateScriptFromTemplate("EditorWindow_ScriptTemplate", "NewEditorWindowScript.cs");


        [MenuItem("Assets/Create/SETB/Scripting/Editor Script", false, 80)]
        public static void Create_EditorScript() => CreateScriptFromTemplate("Editor_ScriptTemplate", "NewEditorScript.cs");


        [MenuItem("Assets/Create/SETB/Scripting/PropertyDrawer Script", false, 80)]
        public static void Create_PropertyDrawerScript() => CreateScriptFromTemplate("PropertyDrawer_ScriptTemplate", "NewPropertyDrawerScript.cs");


        [MenuItem("Assets/Create/SETB/Scripting/PropertyDrawer Managed Reference Script", false, 80)]
        public static void Create_PropertyDrawerManagedReferenceScript() => CreateScriptFromTemplate("PropertyDrawer_ManagedReference_ScriptTemplate", "NewPropertyDrawerManagedReferenceScript.cs");
        #endregion
    }



    #region Helper Classes
    [System.Serializable]
    public class AssemblyDefinitionData
    {
        public string name;
        public string rootNamespace;
    }


    public class CreateTemplateScriptAction : EndNameEditAction
    {
        public string templateText;

        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            string scriptName = Path.GetFileNameWithoutExtension(pathName);
            string folder = Path.GetDirectoryName(pathName);

            string namespaceName = TemplateCreator.GetRootNamespace(folder);

            string finalText = templateText.Replace("#SCRIPTNAME#", scriptName);

            // Handle namespace wrapping
            if (!string.IsNullOrEmpty(namespaceName))
            {
                int startIndex = finalText.IndexOf("#NAMESPACE_START#") + "#NAMESPACE_START#".Length;
                int endIndex = finalText.IndexOf("#NAMESPACE_END#");

                string before = finalText.Substring(0, startIndex);
                string body = finalText.Substring(startIndex, endIndex - startIndex);
                string after = finalText.Substring(endIndex);

                body = TemplateCreator.Indent(body);

                finalText = before + body + after;

                finalText = finalText
                    .Replace("#NAMESPACE#", namespaceName)
                    .Replace("#NAMESPACE_START#", $"namespace {namespaceName}\n{{")
                    .Replace("#NAMESPACE_END#", "}");
            }
            else
            {
                finalText = finalText
                    .Replace("#NAMESPACE#", "")
                    .Replace("#NAMESPACE_START#", "")
                    .Replace("#NAMESPACE_END#", "");
            }

            File.WriteAllText(pathName, finalText);
            AssetDatabase.ImportAsset(pathName);
        }
    }
    #endregion
}
#endif
