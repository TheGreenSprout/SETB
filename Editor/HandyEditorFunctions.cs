#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

using SETB.SuperClasses;
using static SETB.EditorGUI_Base;

namespace SETB
{
    public static class HandyEditorFunctions
    {
        #region Variables
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
            string[] guids = AssetDatabase.FindAssets(name + " t:" + typeof(T).Name);

            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                return AssetDatabase.LoadAssetAtPath<T>(path);
            }
            else Debug.LogWarning($"Asset of type {typeof(T).Name} with name '{name}' not found.");


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
        public static bool HasEditorPref(string key, EditorPrefID? id = null) => EditorPrefs.HasKey(GetEditorPrefID(key, id));


        #region XML doc
        /// <summary>
        /// Adds an EditorPref key to a list in order to keep track of it.
        /// </summary>
        /// <param name="key">The key to track.</param>
        #endregion
        public static void TrackKey(string key)
        {
            WithTrackedEditorPrefs((string[] allKeys) =>
            {
                if (!allKeys.Contains(key)) return allKeys.Length > 0 ? string.Join(";", allKeys) + ";" + key + ";" : key + ";";
                else return null;
            });
        }

        #region XML doc
        /// <summary>
        /// Removes an EditorPref key from the list.
        /// </summary>
        /// <param name="key">The key to detrack.</param>
        #endregion
        public static void DeTrackKey(string key)
        {
            WithTrackedEditorPrefs((List<string> keys) =>
            {
                if (keys.Remove(key)) return string.Join(";", keys) + (keys.Count > 0 ? ";" : "");
                else return null;
            });
        }


        #region XML doc
        /// <summary>
        /// Saves an EditorPref.
        /// </summary>
        /// <param name="key">The EditorPref's key (aka their "name").</param>
        /// <param name="value">The value to save.</param>
        /// <param name="localized">Whether the key is to be localized.</param>
        #endregion
        public static void SetEditorPref<T>(string key, T value, EditorPrefID? id = null)
        {
            string newKey;
            newKey = GetEditorPrefID(key, id);

            TrackKey(newKey);


            if (typeof(T) == typeof(string)) EditorPrefs.SetString(newKey, (string)(object)value);
            else if (typeof(T) == typeof(bool)) EditorPrefs.SetBool(newKey, (bool)(object)value);
            else if (typeof(T) == typeof(int)) EditorPrefs.SetInt(newKey, (int)(object)value);
            else if (typeof(T) == typeof(float)) EditorPrefs.SetFloat(newKey, (float)(object)value);
            else if (typeof(T) == typeof(Vector2))
            {
                SetEditorPref(newKey + "_x", ((Vector2)(object)value).x, null);
                SetEditorPref(newKey + "_y", ((Vector2)(object)value).y, null);
            }
            else if (typeof(T) == typeof(Vector3))
            {
                SetEditorPref(newKey + "_x", ((Vector3)(object)value).x, null);
                SetEditorPref(newKey + "_y", ((Vector3)(object)value).y, null);
                SetEditorPref(newKey + "_z", ((Vector3)(object)value).z, null);
            }
            else if (typeof(T) == typeof(Color))
            {
                string hex = ColorUtility.ToHtmlStringRGBA((Color)(object)value);

                SetEditorPref(newKey, hex, null);
            }
            else if (typeof(T).IsEnum) EditorPrefs.SetString(newKey, value.ToString());
            else if (typeof(T).IsSerializable)
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
        public static T GetEditorPref<T>(string key, T defaultValue = default, EditorPrefID? id = null)
        {
            if (!HasEditorPref(key, id)) return defaultValue;


            string newKey = GetEditorPrefID(key, id);

            if (typeof(T) == typeof(string)) return (T)(object)EditorPrefs.GetString(newKey, (string)(object)defaultValue);
            else if (typeof(T) == typeof(bool)) return (T)(object)EditorPrefs.GetBool(newKey, (bool)(object)defaultValue);
            else if (typeof(T) == typeof(int)) return (T)(object)EditorPrefs.GetInt(newKey, (int)(object)defaultValue);
            else if (typeof(T) == typeof(float)) return (T)(object)EditorPrefs.GetFloat(newKey, (float)(object)defaultValue);
            else if (typeof(T) == typeof(Vector2))
            {
                float x = GetEditorPref(newKey + "_x", ((Vector2)(object)defaultValue).x, null);
                float y = GetEditorPref(newKey + "_y", ((Vector2)(object)defaultValue).y, null);
                return (T)(object)new Vector2(x, y);
            }
            else if (typeof(T) == typeof(Vector3))
            {
                float x = GetEditorPref(newKey + "_x", ((Vector3)(object)defaultValue).x, null);
                float y = GetEditorPref(newKey + "_y", ((Vector3)(object)defaultValue).y, null);
                float z = GetEditorPref(newKey + "_z", ((Vector3)(object)defaultValue).z, null);
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
        public static void DeleteEditorPref(string key, EditorPrefID? id = null)
        {
            if (!HasEditorPref(key, id)) return;


            string newKey;
            newKey = GetEditorPrefID(key, id);

            DeTrackKey(newKey);


            EditorPrefs.DeleteKey(newKey);
        }

        public static void DeleteEditorPrefBy(EditorPrefID filter)
        {
            WithTrackedEditorPrefs((string[] keys) =>
            {
                List<string> remaining = new List<string>();

                foreach (var fullKey in keys)
                {
                    if (EditorPrefKeyMatchesFilter(fullKey, filter)) EditorPrefs.DeleteKey(fullKey);
                    else remaining.Add(fullKey);
                }


                return remaining.Count > 0 ? string.Join(";", remaining) + ";" : "";
            }, true);
        }

        public static void MoveAndDeleteEditorPrefs(EditorPrefID from, EditorPrefID to)
        {
            WithTrackedEditorPrefs((List<string> keys) =>
            {
                List<string> updatedKeys = new List<string>();

                foreach (var fullKey in keys)
                {
                    if (!EditorPrefKeyMatchesFilter(fullKey, from))
                    {
                        updatedKeys.Add(fullKey);
                        continue;
                    }

                    string workingKey = fullKey;
                    if (workingKey.StartsWith(guidPrefix)) workingKey = workingKey.Substring(guidPrefix.Length);

                    string prefix = from.ToString() + "_";
                    int prefixIndex = workingKey.IndexOf(prefix);

                    if (prefixIndex < 0)
                    {
                        updatedKeys.Add(fullKey);
                        continue;
                    }

                    string actualKey = workingKey.Substring(prefixIndex + prefix.Length);

                    
                    string newKey = GetEditorPrefID(actualKey, to);

                    if (EditorPrefs.HasKey(fullKey))
                    {
                        string value = EditorPrefs.GetString(fullKey, null);

                        if (value != null) EditorPrefs.SetString(newKey, value);
                        else
                        {
                            if (EditorPrefs.GetInt(fullKey, int.MinValue) != int.MinValue) EditorPrefs.SetInt(newKey, EditorPrefs.GetInt(fullKey));
                            else if (EditorPrefs.GetFloat(fullKey, float.MinValue) != float.MinValue) EditorPrefs.SetFloat(newKey, EditorPrefs.GetFloat(fullKey));
                            else EditorPrefs.SetBool(newKey, EditorPrefs.GetBool(fullKey));
                        }

                        EditorPrefs.DeleteKey(fullKey);
                        updatedKeys.Add(newKey);
                    }
                }

                return updatedKeys.Count > 0 ? string.Join(";", updatedKeys) + ";" : "";
            }, true);
        }


        #region XML doc
        /// <summary>
        /// Deletes all tracked EditorPrefs.
        /// </summary>
        #endregion
        public static void ClearAllTrackedEditorPrefs()
        {
            string keyListKey = WithTrackedEditorPrefs((string[] keys) =>
            {
                foreach (string key in keys)
                {
                    if (HasEditorPref(key, null)) EditorPrefs.DeleteKey(key);
                }

                return null;
            });


            EditorPrefs.DeleteKey(keyListKey);
        }


        public static string guidPrefix => GetProjectGUID() + "_";
        #region XML doc
        /// <summary>
        /// Turns a string into a "local" version of itself (for the project).
        /// </summary>
        /// <param name="key">The string to be localized.</param>
        /// <returns>Returns the localized string.</returns>
        #endregion
        public static string GetEditorPrefID(string key, EditorPrefID? id = null)
            => id == null ? key : (id.Value.localized ? guidPrefix : "") + (id.ToString() + "_" ?? "") + key;


        #region Helpers
        private static void SetRawEditorPref(string key, string value) => EditorPrefs.SetString(key, value);

        private static string trackedKeys_key => GetProjectGUID() + "__EDITOR_PREFS_TRACKED_KEYS__";
        private static string WithTrackedEditorPrefs(Func<string, string> logic, bool mustWrite = false)
        {
            string allKeys = EditorPrefs.GetString(trackedKeys_key, "");

            string newKeys = logic.Invoke(allKeys);

            if (mustWrite || (!string.IsNullOrWhiteSpace(newKeys) && allKeys != newKeys)) SetRawEditorPref(trackedKeys_key, newKeys);


            return trackedKeys_key;
        }
        private static string WithTrackedEditorPrefs(Func<string[], string> logic, bool mustWrite = false)
        {
            string allKeys = EditorPrefs.GetString(trackedKeys_key, "");

            string newKeys = logic.Invoke(allKeys.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries));

            if (mustWrite || (!string.IsNullOrWhiteSpace(newKeys) && allKeys != newKeys)) SetRawEditorPref(trackedKeys_key, newKeys);


            return trackedKeys_key;
        }
        private static string WithTrackedEditorPrefs(Func<List<string>, string> logic, bool mustWrite = false)
        {
            string allKeys = EditorPrefs.GetString(trackedKeys_key, "");

            string newKeys = logic.Invoke(allKeys.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList());

            if (mustWrite || (!string.IsNullOrWhiteSpace(newKeys) && allKeys != newKeys)) SetRawEditorPref(trackedKeys_key, newKeys);


            return trackedKeys_key;
        }

        private static bool EditorPrefKeyMatchesFilter(string fullKey, EditorPrefID filter)
        {
            string key = fullKey;

            if (key.StartsWith(guidPrefix)) key = key.Substring(guidPrefix.Length);

            var parts = key.Split('_');
            if (parts.Length < 4) return false;


            string owner = parts[0];
            string ns = parts[1];
            string tag = parts[2];

            if (!IsMatch(filter.owner, owner)) return false;
            if (!IsMatch(filter.ns, ns)) return false;
            if (!IsMatch(filter.tag, tag)) return false;


            return true;
        }
        public static bool IsMatch(string filter, string value) => filter != null && (filter == EditorPrefID.ANY || filter == value);
        #endregion
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
        public static BasicPopup PopupWindow(string message, float width = 250, float height = 100, PopupOptions options = null)
        {
            if (options == null) options = new PopupOptions();


            BasicPopup popup = ScriptableObject.CreateInstance<BasicPopup>();

            popup.CreatePopup(message, width, height, options);
            
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
        public static SerializedProperty FindProperty(SerializedObject serializedObject, string name) => serializedObject.FindProperty(name);
        public static SerializedProperty FindPropertyRelative(SerializedProperty target, string relativePath) => target.FindPropertyRelative(relativePath);


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
    
    

        
        #region Quick Tools
        [MenuItem("Tools/Sprout's Editor Tool Base/Quick Tools/ClearEditorPrefs")]
        private static void ClearEditorPrefs()
            => PopupWindow("Are you sure? This will delete ALL EditorPrefs (not only from this project).",
                            250,
                            100,
                            new PopupOptions{CloseButtonLogic = EditorPrefs.DeleteAll, CloseButtonText = "YES"});

        [MenuItem("Tools/Sprout's Editor Tool Base/Quick Tools/ClearTrackedEditorPrefs")]
        private static void ClearTrackedEditorPrefs()
            => PopupWindow("Are you sure? This will delete ALL currently tracked EditorPrefs (in this project).",
                            250,
                            100,
                            new PopupOptions{CloseButtonLogic = ClearAllTrackedEditorPrefs, CloseButtonText = "YES"});
        #endregion
    }




    #region Custom Classes
    public struct EditorPrefID
    {
        public const string ANY = "*";


        public string owner { get; private set; }
        public string ns { get; private set; }
        public string tag { get; private set; }

        public bool localized { get; private set; }


        public EditorPrefID(string owner = null, string ns = null, string tag = null, bool localized = true)
        {
            this.owner = owner ?? "Unknown";
            this.ns = ns ?? "Unknown";
            this.tag = tag ?? "Unknown";

            this.localized = localized;
        }
        public EditorPrefID(EditorPrefID other)
        {
            this.owner = other.owner ?? "Unknown";
            this.ns = other.ns ?? "Unknown";
            this.tag = other.tag ?? "Unknown";

            this.localized = other.localized;
        }

        public static EditorPrefID Any(string owner = ANY, string ns = ANY, string tag = ANY) => new EditorPrefID(owner, ns, tag);


        public override string ToString() => $"{owner}_{ns}_{tag}";
    }



    #region Popup
    public class PopupOptions
    {
        public string Title { get; set; } = "Popup";

        public string CloseButtonText { get; set; } = "OK";
        public Action CloseButtonLogic { get; set; } = null;


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
        #region Variables
        private string message;

        private PopupOptions popupOptions;


        private float windowWidth;
        //private float windowHeight;
        #endregion




        #region Creation
        public void CreatePopup(string m, float width, float height, PopupOptions options = null)
        {
            message = m;
            popupOptions = options ?? new PopupOptions();

            windowWidth = width;
            //windowHeight = height;


            CreateWindow(popupOptions.Title, true, popupOptions.Centered, popupOptions.Locked, width, height, width, height);


            if (!popupOptions.Silent) HandyEditorFunctions.TryPlaySound(popupOptions.Sound);
        }
        #endregion



        #region Main
        protected void OnGUI()
        {
            if (popupOptions == null || message == null)
            {
                Close();
                return;
            }


            Space(10);


            if (popupOptions.Image != null)
            {
                Space(10);

                Horizontal(() => CenterHorizontal(() => DrawImage(popupOptions.Image,
                                                                    null,
                                                                    GUILayout.Width(popupOptions.ImageWidth),
                                                                    GUILayout.Height(popupOptions.ImageHeight))));

                Space(10);
            }


            Center(() => DrawLabel(message, new GUIStyle(EditorStyles.wordWrappedLabel) { alignment = TextAnchor.MiddleCenter }, GUILayout.Width(windowWidth)));


            FlexibleSpace();

            DrawButton(popupOptions.CloseButtonText, () => {
                if (popupOptions == null) return;

                popupOptions.CloseButtonLogic?.Invoke();
                Close();
            });
        }
        #endregion
    }
    #endregion
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
#endif
