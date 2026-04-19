#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

using SETB.SuperClasses;
using static SETB.EditorGUI_Base;
using static SETB.HandyEditorFunctions;

#NAMESPACE_START#
public class #SCRIPTNAME# : EditorWindow_Base<#SCRIPTNAME#>
{
    [MenuItem("Tools/...")]
    public static void ShowWindow() => CreateWindow("Window Title");


    protected void OnGUI()
    {
        
    }
}
#NAMESPACE_END#
#endif
