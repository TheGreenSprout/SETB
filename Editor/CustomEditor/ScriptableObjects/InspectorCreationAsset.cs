#if UNITY_EDITOR
using System;
using UnityEngine;

using Alchemy.Inspector;

using SETB.InnerWorkings;

namespace SETB._CustomEditor.ScriptableObjects
{
    [CreateAssetMenu(fileName = "InspectorCreationAsset", menuName = "SETB/Custom Editor/InspectorCreationAsset")]
    public class InspectorCreationAsset : ScriptableObject
    {
        [HideInInspector] public string lastName;


        public bool removeMethodsOnDelete = true;

        public InspectorCreationData[] data;



        [Button]
        public void GenerateMethods() => lastName = InspectorCreation_InnerWorkings.GenerateMethods(this, lastName);

        [Button]
        public void DeleteMethods() => lastName = InspectorCreation_InnerWorkings.GenerateMethods(this, lastName);
    }




    [Serializable]
    public class InspectorCreationData
    {
        public string path;
        
        public GameObject prefab;


        public bool validateFunction = false;
        public int priority = 50;

        public bool unpack = true;

        [Tooltip("If the object ends up created under a RectTransform (like a Canvas), add a RectTransform to it too instead of leaving a plain Transform that the UI layout ignores.")]
        public bool addRectTransformUnderCanvas = false;



        public InspectorCreationData()
        {
            validateFunction = false;
            priority = 50;

            unpack = true;
            addRectTransformUnderCanvas = false;
        }
    }
}
#endif
