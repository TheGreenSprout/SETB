using UnityEngine;
using UnityEditor;

using SETB;
using static SETB.HandyEditorFunctions;

//[CustomPropertyDrawer(typeof(ClassName.PropertyName), true /*Children Scripts are affected*/)]
public class #SCRIPTNAME# : PropertyDrawer_Base<#SCRIPTNAME#>
{
    #region Draw
    protected override void Draw(SerializedProperty property)
    {
        base.Draw(property); // Draws the default property field. Remove this line if you want custom logic.

        // Your custom property drawer code goes here.
    }
    #endregion



    #region Layout
    protected override void Build(SerializedProperty property)
    {
        // Default layout (matches DrawProperty)
        Space(EditorGUI.GetPropertyHeight(property, true));


        // If using children start from:
        //Space(GetAllChildrenHeight(property));


        // If fully custom, start from:
        //Space(EditorGUIUtility.singleLineHeight);
    }
    #endregion
}
