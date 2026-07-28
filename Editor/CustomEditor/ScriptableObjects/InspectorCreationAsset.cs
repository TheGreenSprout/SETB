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


        public InspectorCreationData[] data;



        [Button]
        public void GenerateMethods() => lastName = InspectorCreation_InnerWorkings.GenerateMethods(this, lastName);
    }




    [Serializable]
    public class InspectorCreationData
    {
        public string path;
        
        public GameObject prefab;


        public bool validateFunction = false;
        public int priority = 50;

        public bool unpack = true;



        public InspectorCreationData()
        {
            validateFunction = false;
            priority = 50;

            unpack = true;
        }
    }
}
#endif
