#if UNITY_EDITOR
using System;
using UnityEngine;

using SETB.InnerWorkings;

namespace SETB._CustomEditor.ScriptableObjects
{
    [CreateAssetMenu(fileName = "InspectorCreationAssets", menuName = "SETB/Custom Editor/InspectorCreationAssets")]
    public class InspectorCreationAssets : ScriptableObject
    {
        [HideInInspector] public string lastName;


        public InspectorCreationData[] data;




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
