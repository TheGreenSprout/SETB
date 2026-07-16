#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

using SETB.SuperClasses;

using static SETB.HandyEditorFunctions;

#NAMESPACE_START#
//[CustomPropertyDrawer(typeof(ClassName.PropertyName), true /*Children Scripts are affected*/)]
public class #SCRIPTNAME# : PropertyDrawer_Base<#SCRIPTNAME#>
{
    #region Main
    protected override void Build(SerializedProperty property)
    {
        // Draws the default property field. Remove this line if you want custom logic.
        DrawProperty(property, true);
    }
    #endregion
}
#NAMESPACE_END#
#endif
