#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SETB
{
    public static class EditorHelpers
    {
        #region File Fetchers
        public static IEnumerable<string> EnumerateAllGUIDs<T>(string filter = "") where T : Object
            => GetAllGUIDs<T>(filter);

        public static string[] GetAllGUIDs<T>(string filter = "") where T : Object
            => AssetDatabase.FindAssets(string.IsNullOrEmpty(filter) ? $"t:{typeof(T).Name}" : $"{filter} t:{typeof(T).Name}");


        private static IEnumerable<(T asset, string path)> EnumerateInternal<T>(string filter) where T : Object
        {
            foreach (var guid in EnumerateAllGUIDs<T>(filter))
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                var obj = AssetDatabase.LoadAssetAtPath<T>(path);

                if (obj != null) yield return (obj, path);
            }
        }

        public static IEnumerable<T> EnumerateAssets<T>(string filter = "") where T : Object
        {
            foreach (var (asset, _) in EnumerateInternal<T>(filter))
            {
                yield return asset;
            }
        }
        public static IEnumerable<(T asset, string path)> EnumerateAssetsWithPath<T>(string filter = "") where T : Object
            => EnumerateInternal<T>(filter);

        public static List<T> GetAllAssets<T>(string filter = "") where T : Object
        {
            var list = new List<T>();

            foreach (var (asset, _) in EnumerateInternal<T>(filter))
            {
                list.Add(asset);
            }


            return list;
        }
        public static List<(T asset, string path)> GetAllAssetsWithPath<T>(string filter = "") where T : Object
            => new(EnumerateInternal<T>(filter));


        public static IEnumerable<System.Type> EnumerateAllChildren<T>() where T : System.Type
        => GetAllChildren<T>();
        public static List<System.Type> GetAllChildren<T>() where T : System.Type
            => new List<System.Type>(TypeCache.GetTypesDerivedFrom<T>());
        public static TypeCache.TypeCollection GetAllChildrenTypeCollection<T>() where T : System.Type
            => TypeCache.GetTypesDerivedFrom<T>();

        public static IEnumerable<System.Type> EnumerateAllChildrenAndSelf<T>(T baseType) where T : System.Type
            => GetAllChildrenAndSelf(baseType);
        public static List<System.Type> GetAllChildrenAndSelf<T>(T baseType) where T : System.Type
            => new(GetAllChildrenAndSelfTypeCollection(baseType));
        public static TypeCache.TypeCollection GetAllChildrenAndSelfTypeCollection<T>(T baseType) where T : System.Type
        {
            var ret = GetAllChildrenTypeCollection<T>();

            ret.Append(baseType);


            return ret;
        }
        #endregion
    }
}
#endif
