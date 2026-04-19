#if UNITY_EDITOR
using UnityEditor;

using SETB._CustomEditor.ScriptableObjects;
using SETB.SuperClasses;
using static SETB.EditorGUI_Base;

namespace SETB._CustomEditor.Drawers
{
    [CustomEditor(typeof(InspectorCreationAsset))]
    public class InspectorCreationAsset_Drawer : Editor_Base<InspectorCreationAsset_Drawer>
    {
        protected override void DrawInspector()
        {
            serializedObject.Update();


            DrawButton("Generate Methods", () => ((InspectorCreationAsset)target).GenerateMethods());

            Space(10f);

            base.DrawInspector();


            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif
