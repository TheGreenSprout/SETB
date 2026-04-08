using UnityEngine;
using UnityEditor;

using SETB;
using static SETB.HandyEditorFunctions;

//[CustomPropertyDrawer(typeof(ClassName.PropertyName), true /*Children Scripts are affected*/)]
public class PropertyDrawer_ScriptTemplate : PropertyDrawer_Base<PropertyDrawer_ScriptTemplate>
{
    
    protected override void Draw(SerializedProperty property)
    {
        base.Draw(property); // Draws the default property field. Remove this line if you don't want that.

        // Your custom property drawer code goes here.
    }
}
