using UnityEngine;
using UnityEditor;

using SETB;
using static SETB.EditorGUI_Base;
using static SETB.HandyEditorFunctions;

public class EditorWindow_ScriptTemplate : EditorWindow_Base<EditorWindow_ScriptTemplate>
{
    [MenuItem("Tools/...")]
    public static void ShowWindow() => CreateWindow("Window Title");
}
