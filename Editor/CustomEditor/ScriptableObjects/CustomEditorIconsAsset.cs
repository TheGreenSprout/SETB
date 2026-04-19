#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace SETB._CustomEditor.ScriptableObjects
{
    [CreateAssetMenu(fileName = "CustomIconsAsset", menuName = "SETB/Custom Editor/CustomIconsAsset")]
    public class CustomEditorIconsAsset : ScriptableObject
    {
        public CustomEditorIcon_Data[] data;
    }




    [Serializable]
    public class CustomEditorIcon_Data
    {
        public MonoScript script;
        
        public Texture2D icon;
    }
}
#endif
