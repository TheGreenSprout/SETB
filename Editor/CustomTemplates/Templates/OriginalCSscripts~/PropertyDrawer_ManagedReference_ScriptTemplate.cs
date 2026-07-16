#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

using SETB.SuperClasses;

using static SETB.HandyEditorFunctions;

#NAMESPACE_START#
//[CustomPropertyDrawer(typeof(ClassName.PropertyName), true /*Children Scripts are affected*/)]
public class #SCRIPTNAME# : PropertyDrawer_Base<#SCRIPTNAME#>
{
    #region Types
    private static readonly string[] typeNames = new[]
    {
        "None",
        "Name1",
        "Name2"
    };

    private static readonly Type[] types = new Type[]
    {
        null,
        typeof(ClassName.PropertyName1),
        typeof(ClassName.PropertyName2)
    };
    #endregion




    #region Main
    protected override void Build(SerializedProperty property) => DrawManagedReferenceDropdown(property, "Gradient Type", types, typeNames);
    #endregion
}
#NAMESPACE_END#
#endif
