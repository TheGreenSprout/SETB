using UnityEngine;
using UnityEditor;

using SETB;
using static SETB.EditorGUI_Base;
using static SETB.HandyEditorFunctions;

//[CustomEditor(typeof(ClassName), true /*Children Scripts are affected*/)]
public class #SCRIPTNAME# : Editor_Base<#SCRIPTNAME#>
{
    protected override void DrawInspector()
    {
        base.DrawInspector(); // Draws the default inspector. Remove this line if you don't want that.

        // Your custom inspector code goes here.
    }
}
