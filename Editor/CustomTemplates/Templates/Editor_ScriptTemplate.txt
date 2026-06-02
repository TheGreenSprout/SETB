#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

using SETB.SuperClasses;
using static SETB.EditorGUI_Base;
using static SETB.HandyEditorFunctions;

#NAMESPACE_START#
//[CustomEditor(typeof(ClassName), true /*Children Scripts are affected*/)]
public class #SCRIPTNAME# : Editor_Base<#SCRIPTNAME#>
{
    #region Main
    protected override void DrawInspector()
    {
        base.DrawInspector(); // Draws the default inspector. Remove this line if you want custom logic.

        // Your custom inspector code goes here.
    }
    #endregion
}
#NAMESPACE_END#
#endif
