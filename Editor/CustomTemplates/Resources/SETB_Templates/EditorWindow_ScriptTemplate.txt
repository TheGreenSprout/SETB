using UnityEngine;
using UnityEditor;

using SETB;
using static SETB.EditorGUI_Base;
using static SETB.HandyEditorFunctions;

public class #SCRIPTNAME# : EditorWindow_Base<#SCRIPTNAME#>
{
    [MenuItem("Tools/...")]
    public static void ShowWindow() => CreateWindow("Window Title");


    protected void OnGUI()
    {
        
    }
}
