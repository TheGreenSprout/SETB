using UnityEngine;
using UnityEditor;

using SETB;
using static SETB.HandyEditorFunctions;

#NAMESPACE_START#
//[CustomPropertyDrawer(typeof(ClassName.PropertyName), true /*Children Scripts are affected*/)]
public class #SCRIPTNAME# : PropertyDrawer_Base<#SCRIPTNAME#>
{
    protected override void Build(SerializedProperty property)
    {
        // Draws the default property field. Remove this line if you want custom logic.
        DrawProperty(property, true);
    }
}
#NAMESPACE_END#
