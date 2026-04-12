using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace SETB
{
    public static class HandyEditorFunctions
    {
        #region Variables
        public enum AssetTypes
        {
            AudioClip,
            Texture2D,
            Sprite,
            Font
        }



        private static string ProjectGUID = null;

        public static string GetProjectGUID()
        {
            if (!string.IsNullOrEmpty(ProjectGUID)) return ProjectGUID;


            string path = Path.Combine(Application.dataPath, "../ProjectSettings/ProjectSettings.asset");

            if (!File.Exists(path)) ProjectGUID = "UnknownProject";
            else
            {
                string content = File.ReadAllText(path);

                var match = Regex.Match(content, @"productGUID:\s*([a-f0-9]+)");

                if (match.Success) ProjectGUID = match.Groups[1].Value;
                else ProjectGUID = "UnknownProject";
            }


            return ProjectGUID;
        }



            #region EditorPrefs Cache
            private class EditorPrefFieldData
            {
                public Func<UnityEngine.Object, object> GetValue;
                public Action<UnityEngine.Object, object> SetValue;

                public Func<string, object, object> Getter;
                public Action<string, object> Setter;

                public string Key;
                public object DefaultValue;
            }


            private static Dictionary<Type, List<EditorPrefFieldData>> _cachedFields = new();
            #endregion
        #endregion




        #region Assets
        #region XML doc
        /// <summary>
        /// Finds and retrieves an asset from your project.
        /// </summary>
        /// <param name="name">The name of the asset (file extension is optional).</param>
        /// <returns>Returns the found asset. If multiple assets have the same name, the one with the same file extension as the one written in name will be retrieved (if name doesn't have an extension, it will retrieve the first element found).</returns>
        #endregion
        public static T FindAssetByName<T>(string name) where T : UnityEngine.Object
        {
            #if UNITY_EDITOR
            
            string[] guids = AssetDatabase.FindAssets(name + " t:" + typeof(T).Name);

            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                return AssetDatabase.LoadAssetAtPath<T>(path);
            }
            else
            {
                Debug.LogWarning($"Asset of type {typeof(T).Name} with name '{name}' not found.");
            }

            #endif


            #if !UNITY_EDITOR
            throw new InvalidOperationException("FindAssetByName can only be used in the Unity Editor.");
            #endif


            return null;
        }


        #region XML doc
        /// <summary>
        /// Tries to play a sound (when failing plays the default EditorApplication.Beep()).
        /// </summary>
        /// <param name="sound">The sound to play.</param>
        #endregion
        public static void TryPlaySound(AudioClip sound = null)
        {
            // If sound is null, play default beep
            if (sound == null)
            {
                EditorApplication.Beep();

                return;
            }


            var audioUtilType = typeof(AudioImporter).Assembly.GetType("UnityEditor.AudioUtil");

            if (audioUtilType != null)
            {
                var methodNames = new[] { "PlayPreviewClip", "PlayClip" };

                foreach (var name in methodNames)
                {
                    var m = audioUtilType.GetMethod(
                        name,
                        BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic,
                        null,
                        new Type[] { typeof(AudioClip), typeof(int), typeof(bool) },
                        null
                    );


                    if (m != null)
                    {
                        m.Invoke(null, new object[] { sound, 0, false });

                        return;
                    }
                }
            }


            // Fallback
            Debug.LogWarning("Could not find AudioUtil.PlayPreviewClip; playing default beep.");

            EditorApplication.Beep();
        }
        #region XML doc
        /// <summary>
        /// Tries to play a sound (when failing plays the default EditorApplication.Beep()).
        /// </summary>
        /// <param name="soundName">The name of the sound to play (file extension is optional).</param>
        #endregion
        public static void TryPlaySound(string soundName = null)
        {
            AudioClip sound = FindAssetByName<AudioClip>(soundName);

            TryPlaySound(sound);
        }
        #endregion



        #region Manage EditorPrefs
        #region XML doc
        /// <summary>
        /// Checks if an EditorPref exists.
        /// </summary>
        /// <param name="key">The EditorPref's key (aka their "name").</param>
        /// <param name="localized">Whether the key was localized when saving the EditorPref.</param>
        /// <returns>Returns whether the EditorPref exists.</returns>
        #endregion
        public static bool HasEditorPref(string key, bool localized = true, string tag = null) => EditorPrefs.HasKey(localized ? LocalizeString(key, tag) : key);


        #region XML doc
        /// <summary>
        /// Adds an EditorPref key to a list in order to keep track of it.
        /// </summary>
        /// <param name="key">The key to track.</param>
        #endregion
        public static void TrackKey(string key)
        {
            string keyListKey = LocalizeString("EditorPrefsKeys");
            string allKeys = EditorPrefs.GetString(keyListKey, "");

            if (!allKeys.Contains(key))
            {
                allKeys += key + ";";
                EditorPrefs.SetString(keyListKey, allKeys);
            }
        }

        #region XML doc
        /// <summary>
        /// Removes an EditorPref key from the list.
        /// </summary>
        /// <param name="key">The key to detrack.</param>
        #endregion
        public static void DeTrackKey(string key)
        {
            string keyListKey = LocalizeString("EditorPrefsKeys");

            string allKeys = EditorPrefs.GetString(keyListKey, "");
            var keys = allKeys.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();

            if (keys.Remove(key))
            {
                string updated = string.Join(";", keys) + (keys.Count > 0 ? ";" : "");
                EditorPrefs.SetString(keyListKey, updated);
            }
        }


        #region XML doc
        /// <summary>
        /// Saves an EditorPref.
        /// </summary>
        /// <param name="key">The EditorPref's key (aka their "name").</param>
        /// <param name="value">The value to save.</param>
        /// <param name="localized">Whether the key is to be localized.</param>
        #endregion
        public static void SetEditorPref<T>(string key, T value, bool localized = true, string tag = null)
        {
            string newKey;
            if (localized)
            {
                newKey = LocalizeString(key, tag);

                TrackKey(newKey);
            }
            else newKey = key;

            if (typeof(T) == typeof(string)) EditorPrefs.SetString(newKey, (string)(object)value);
            else if (typeof(T) == typeof(bool)) EditorPrefs.SetBool(newKey, (bool)(object)value);
            else if (typeof(T) == typeof(int)) EditorPrefs.SetInt(newKey, (int)(object)value);
            else if (typeof(T) == typeof(float)) EditorPrefs.SetFloat(newKey, (float)(object)value);
            else if (typeof(T) == typeof(Vector2))
            {
                SetEditorPref(newKey + "_x", ((Vector2)(object)value).x, false);
                SetEditorPref(newKey + "_y", ((Vector2)(object)value).y, false);
            }
            else if (typeof(T) == typeof(Vector3))
            {
                SetEditorPref(newKey + "_x", ((Vector3)(object)value).x, false);
                SetEditorPref(newKey + "_y", ((Vector3)(object)value).y, false);
                SetEditorPref(newKey + "_z", ((Vector3)(object)value).z, false);
            }
            else if (typeof(T) == typeof(Color))
            {
                string hex = ColorUtility.ToHtmlStringRGBA((Color)(object)value);

                SetEditorPref(newKey, hex, false);
            }
            else if (typeof(T).IsEnum) EditorPrefs.SetString(newKey, value.ToString());
            else if (!typeof(T).IsSerializable)
            {
                string json = JsonUtility.ToJson(value);
                EditorPrefs.SetString(newKey, json);
            }
            else
            {
                DeTrackKey(newKey);

                throw new NotSupportedException($"Type {typeof(T)} is not supported by SetEditorPref and isn't Serializable.");
            }
        }

        #region XML doc
        /// <summary>
        /// Retrieves an EditorPref's value.
        /// </summary>
        /// <param name="key">The EditorPref's key (aka their "name").</param>
        /// <param name="localized">Whether the key was localized when saving the EditorPref.</param>
        /// <param name="defaultValue">The default value of this EditorPref.</param>
        /// <returns>Returns the value of the EditorPref.</returns>
        #endregion
        public static T GetEditorPref<T>(string key, T defaultValue = default, bool localized = true, string tag = null)
        {
            if (!HasEditorPref(key, localized, tag)) return defaultValue;


            string newKey = localized ? LocalizeString(key, tag) : key;

            if (typeof(T) == typeof(string)) return (T)(object)EditorPrefs.GetString(newKey, (string)(object)defaultValue);
            else if (typeof(T) == typeof(bool)) return (T)(object)EditorPrefs.GetBool(newKey, (bool)(object)defaultValue);
            else if (typeof(T) == typeof(int)) return (T)(object)EditorPrefs.GetInt(newKey, (int)(object)defaultValue);
            else if (typeof(T) == typeof(float)) return (T)(object)EditorPrefs.GetFloat(newKey, (float)(object)defaultValue);
            else if (typeof(T) == typeof(Vector2))
            {
                float x = GetEditorPref(newKey + "_x", ((Vector2)(object)defaultValue).x, false);
                float y = GetEditorPref(newKey + "_y", ((Vector2)(object)defaultValue).y, false);
                return (T)(object)new Vector2(x, y);
            }
            else if (typeof(T) == typeof(Vector3))
            {
                float x = GetEditorPref(newKey + "_x", ((Vector3)(object)defaultValue).x, false);
                float y = GetEditorPref(newKey + "_y", ((Vector3)(object)defaultValue).y, false);
                float z = GetEditorPref(newKey + "_z", ((Vector3)(object)defaultValue).z, false);
                return (T)(object)new Vector3(x, y, z);
            }
            else if (typeof(T) == typeof(Color))
            {
                string hex = EditorPrefs.GetString(newKey, ColorUtility.ToHtmlStringRGBA((Color)(object)defaultValue));

                if (ColorUtility.TryParseHtmlString("#" + hex, out var color)) return (T)(object)color;

                return defaultValue;
            }
            else if (typeof(T).IsEnum)
            {
                string str = EditorPrefs.GetString(newKey, defaultValue.ToString());

                try { return (T)Enum.Parse(typeof(T), str); }
                catch { return defaultValue; }
            }
            else if (typeof(T).IsSerializable)
            {
                string json = EditorPrefs.GetString(newKey, "");
                if (string.IsNullOrEmpty(json)) return defaultValue;

                try { return JsonUtility.FromJson<T>(json); }
                catch { return defaultValue; }
            }
            else throw new NotSupportedException($"Type {typeof(T)} is not supported by GetEditorPref and isn't Serializable.");
        }


        #region XML doc
        /// <summary>
        /// Deletes an EditorPref.
        /// </summary>
        /// <param name="key">The EditorPref's key (aka their "name").</param>
        /// <param name="localized">Whether the key was localized when saving the EditorPref.</param>
        #endregion
        public static void DeleteEditorPref(string key, bool localized = true, string tag = null)
        {
            if (!HasEditorPref(key, localized, tag)) return;


            string newKey;
            if (localized)
            {
                newKey = LocalizeString(key, tag);

                DeTrackKey(newKey);
            }
            else newKey = key;

            EditorPrefs.DeleteKey(newKey);
        }

        #region XML doc
        /// <summary>
        /// Deletes all tracked EditorPrefs.
        /// </summary>
        #endregion
        public static void ClearAllTrackedEditorPrefs()
        {
            string keyListKey = LocalizeString("EditorPrefsKeys");

            string allKeys = EditorPrefs.GetString(keyListKey, "");
            string[] keys = allKeys.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string key in keys)
            {
                if (HasEditorPref(key, false)) EditorPrefs.DeleteKey(key);
            }


            EditorPrefs.DeleteKey(keyListKey);
        }


        #region XML doc
        /// <summary>
        /// Turns a string into a "local" version of itself (for the project).
        /// </summary>
        /// <param name="key">The string to be localized.</param>
        /// <returns>Returns the localized string.</returns>
        #endregion
        public static string LocalizeString(string key, string tag = null) => GetProjectGUID() + "_" + (String.IsNullOrEmpty(tag) ? "" : tag + "_") + key;
        #endregion

    
        
        #region Custom Classes Logic
        #region XML doc
        /// <summary>
        /// Instantiates a popup window.
        /// </summary>
        /// <param name="message">The message to display in this popup.</param>
        /// <param name="width">The width of the popup.</param>
        /// <param name="height">The height of the popup.</param>
        /// <param name="options">The PopupOptions of this popup.</param>
        #endregion
        public static BasicPopup PopupWindow(string message, float width, float height, PopupOptions options)
        {
            if (options == null) options = new PopupOptions();


            BasicPopup popup = ScriptableObject.CreateInstance<BasicPopup>();

            popup.CreatePopup(message, width, height, options.Title, options.CloseButtonText, options.Sound, options.Silent, options.Image, options.ImageWidth, options.ImageHeight, options.Centered, options.Locked);
            
            return popup;
        }
        #endregion



        #region Custom Attributes Logic
        private static List<EditorPrefFieldData> GetCachedFields(Type type)
        {
            if (_cachedFields.TryGetValue(type, out var cached)) return cached;

            var list = new List<EditorPrefFieldData>();

            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (var field in fields)
            {
                var attr = field.GetCustomAttribute<EditorPrefAttribute>();
                if (attr == null) continue;

                var fieldType = field.FieldType;

                // ---------- Compile field getter ----------
                var objParam = Expression.Parameter(typeof(UnityEngine.Object), "obj");
                var castObj = Expression.Convert(objParam, type);
                var fieldAccess = Expression.Field(castObj, field);
                var castToObject = Expression.Convert(fieldAccess, typeof(object));

                var getValue = Expression.Lambda<Func<UnityEngine.Object, object>>(
                    castToObject, objParam
                ).Compile();

                // ---------- Compile field setter ----------
                var valueParam = Expression.Parameter(typeof(object), "value");
                var castValue = Expression.Convert(valueParam, fieldType);

                var assign = Expression.Assign(fieldAccess, castValue);

                var setValue = Expression.Lambda<Action<UnityEngine.Object, object>>(
                    assign, objParam, valueParam
                ).Compile();

                // ---------- Compile EditorPrefs getter ----------
                var getMethod = typeof(HandyEditorFunctions)
                    .GetMethod(nameof(HandyEditorFunctions.GetEditorPref))
                    .MakeGenericMethod(fieldType);

                var keyParam = Expression.Parameter(typeof(string), "key");
                var defaultParam = Expression.Parameter(typeof(object), "default");

                var castDefault = Expression.Convert(defaultParam, fieldType);

                var tagConst = Expression.Constant(string.IsNullOrEmpty(attr.TagOverride) ? type.FullName : attr.TagOverride);

                var callGet = Expression.Call(
                    getMethod,
                    keyParam,
                    castDefault,
                    Expression.Constant(true),
                    tagConst
                );

                var castResult = Expression.Convert(callGet, typeof(object));

                var getter = Expression.Lambda<Func<string, object, object>>(
                    castResult, keyParam, defaultParam
                ).Compile();

                // ---------- Compile EditorPrefs setter ----------
                var setMethod = typeof(HandyEditorFunctions)
                    .GetMethod(nameof(HandyEditorFunctions.SetEditorPref))
                    .MakeGenericMethod(fieldType);

                var valueParam2 = Expression.Parameter(typeof(object), "value");
                var castValue2 = Expression.Convert(valueParam2, fieldType);

                var callSet = Expression.Call(
                    setMethod,
                    keyParam,
                    castValue2,
                    Expression.Constant(true),
                    tagConst
                );

                var setter = Expression.Lambda<Action<string, object>>(
                    callSet, keyParam, valueParam2
                ).Compile();

                list.Add(new EditorPrefFieldData
                {
                    GetValue = getValue,
                    SetValue = setValue,
                    Getter = getter,
                    Setter = setter,
                    Key = attr.Key,
                    DefaultValue = attr.DefaultValue
                });
            }

            _cachedFields[type] = list;
            return list;
        }


        #region XML doc
        /// <summary>
        /// Loads all the tracked EditorPrefs on enable.
        /// </summary>
        #endregion
        public static void Load_AttributeEditorPrefs<T>(this T obj) where T : UnityEngine.Object
        {
            var fields = GetCachedFields(obj.GetType());

            foreach (var f in fields)
            {
                var value = f.Getter(f.Key, f.DefaultValue);
                f.SetValue(obj, value);
            }
        }

        #region XML doc
        /// <summary>
        /// Saves all the tracked EditorPrefs on enable.
        /// </summary>
        #endregion
        public static void Save_AttributeEditorPrefs<T>(this T obj) where T : UnityEngine.Object
        {
            var fields = GetCachedFields(obj.GetType());

            foreach (var f in fields)
            {
                var value = f.GetValue(obj);
                f.Setter(f.Key, value);
            }
        }
        #endregion



        #region Misc
        public static void Record(SerializedProperty property, string name = "Change")
        {
            if (property.serializedObject.targetObject != null) Undo.RecordObject(property.serializedObject.targetObject, name);
        }
        public static void Record(UnityEngine.Object target, string name = "Change")
        {
            if (target != null) Undo.RecordObject(target, name);
        }


        public static void UtilitySetDirty(SerializedProperty property) => EditorUtility.SetDirty(property.serializedObject.targetObject);
        public static void UtilitySetDirty(UnityEngine.Object target) => EditorUtility.SetDirty(target);
        #endregion
    }




    #region Custom Classes
    public class PopupOptions
    {
        public string Title { get; set; } = "PopUp";

        public string CloseButtonText { get; set; } = "OK";


        public AudioClip Sound { get; set; } = null;

        public bool Silent { get; set; } = false;


        public Texture2D Image { get; set; } = null;

        public float ImageWidth { get; set; } = 64;
        public float ImageHeight { get; set; } = 64;


        public bool Centered { get; set; } = true;
        public bool Locked { get; set; } = false;
    }


    public class BasicPopup : EditorWindow_Base<BasicPopup>
    {
        private string message;

        private string closeButtonText;


        private AudioClip sound;

        private Texture2D image;


        private float windowWidth;
        //private float windowHeight;

        private float imageWidth;
        private float imageHeight;



        public void CreatePopup(string m, float width, float height, string title = "PopUp", string c = "OK", AudioClip s = null, bool silentWindow = false, Texture2D i = null, float w = 64, float h = 64, bool centered = true, bool locked = true)
        {
            message = m;
            windowWidth = width;
            //windowHeight = height;
            closeButtonText = c;
            sound = s;
            image = i;
            imageWidth = w;
            imageHeight = h;


            CreateWindow(title, true, centered, locked, width, height, width, height);


            if (!silentWindow)
            {
                HandyEditorFunctions.TryPlaySound(sound);
            }
        }


        protected void OnGUI()
        {
            GUILayout.Space(20);


            if (image != null)
            {
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();

                GUILayout.Label(image, GUILayout.Width(imageWidth), GUILayout.Height(imageHeight));

                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }


            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(
                message,
                new GUIStyle(EditorStyles.wordWrappedLabel) { alignment = TextAnchor.MiddleCenter },
                GUILayout.Width(windowWidth)
            );
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();


            GUILayout.FlexibleSpace();

            if (GUILayout.Button(closeButtonText))
            {
                Close();
            }
        }
    }
    #endregion



    #region Custom Attributes
    [AttributeUsage(AttributeTargets.Field)]
    public class EditorPrefAttribute : Attribute
    {
        public string Key { get; }

        public object DefaultValue { get; }

        public string TagOverride;


        public EditorPrefAttribute(string key, object defaultValue = default)
        {
            Key = key;

            DefaultValue = defaultValue;
        }
    }
    #endregion
}
