#if UNITY_EDITOR
using UnityEditor;

using SETB._CustomEditor.ScriptableObjects;

namespace SETB.InnerWorkings
{
    public class InspectorCreationAsset_DeletionHandler : AssetModificationProcessor
    {
        private static AssetDeleteResult OnWillDeleteAsset(string assetPath, RemoveAssetOptions options)
        {
            var obj = AssetDatabase.LoadAssetAtPath<InspectorCreationAsset>(assetPath);

            if (obj != null && obj.removeMethodsOnDelete) InspectorCreation_InnerWorkings.DeleteMethods(obj, obj.lastName);

            return AssetDeleteResult.DidNotDelete;
        }
    }
}
#endif
