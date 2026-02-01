using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace SETB
{
    public static class HandyEditorFunctions
    {
        public enum AssetTypes
        {
            AudioClip,
            Texture2D,
            Sprite,
            Font
        }



        public static string ProjectKey => Application.dataPath.GetHashCode().ToString();




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


            // Try UnityEditor.AudioUtil via reflection
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


            // Fallback, play default beep
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
        public static bool HasEditorPref(string key, bool localized = true)
        {
            return EditorPrefs.HasKey(localized ? LocalizeString(key) : key);
        }


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
        public static void SetEditorPref<T>(string key, T value, bool localized = true)
        {
            string newKey;
            if (localized)
            {
                newKey = LocalizeString(key);

                TrackKey(newKey);
            }
            else
            {
                newKey = key;
            }

            if (typeof(T) == typeof(string))
            {
                EditorPrefs.SetString(newKey, (string)(object)value);
            }
            else if (typeof(T) == typeof(bool))
            {
                EditorPrefs.SetBool(newKey, (bool)(object)value);
            }
            else if (typeof(T) == typeof(int))
            {
                EditorPrefs.SetInt(newKey, (int)(object)value);
            }
            else if (typeof(T) == typeof(float))
            {
                EditorPrefs.SetFloat(newKey, (float)(object)value);
            }
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
            else if (typeof(T).IsEnum)
            {
                EditorPrefs.SetString(newKey, (string)(object)value);
            }
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
        public static T GetEditorPref<T>(string key, bool localized = true, T defaultValue = default)
        {
            if (!HasEditorPref(key, localized))
            {
                return default;
            }


            string newKey = localized ? LocalizeString(key) : key;

            if (typeof(T) == typeof(string))
            {
                return (T)(object)EditorPrefs.GetString(newKey, (string)(object)defaultValue);
            }
            else if (typeof(T) == typeof(bool))
            {
                return (T)(object)EditorPrefs.GetBool(newKey, (bool)(object)defaultValue);
            }
            else if (typeof(T) == typeof(int))
            {
                return (T)(object)EditorPrefs.GetInt(newKey, (int)(object)defaultValue);
            }
            else if (typeof(T) == typeof(float))
            {
                return (T)(object)EditorPrefs.GetFloat(newKey, (float)(object)defaultValue);
            }
            else if (typeof(T) == typeof(Vector2))
            {
                float x = GetEditorPref(newKey + "_x", false, ((Vector2)(object)defaultValue).x);
                float y = GetEditorPref(newKey + "_y", false, ((Vector2)(object)defaultValue).y);
                return (T)(object)new Vector2(x, y);
            }
            else if (typeof(T) == typeof(Vector3))
            {
                float x = GetEditorPref(newKey + "_x", false, ((Vector3)(object)defaultValue).x);
                float y = GetEditorPref(newKey + "_y", false, ((Vector3)(object)defaultValue).y);
                float z = GetEditorPref(newKey + "_z", false, ((Vector3)(object)defaultValue).z);
                return (T)(object)new Vector3(x, y, z);
            }
            else if (typeof(T) == typeof(Color))
            {
                string hex = GetEditorPref(key, localized, ((Color)(object)defaultValue).ToString());

                if (ColorUtility.TryParseHtmlString("#" + hex, out var color))
                {
                    return (T)(object)color;
                }

                return defaultValue;
            }
            else if (typeof(T).IsEnum)
            {
                string str = EditorPrefs.GetString(newKey, defaultValue.ToString());

                try
                {
                    return (T)Enum.Parse(typeof(T), str);
                }
                catch
                {
                    return defaultValue;
                }
            }
            else if (typeof(T).IsSerializable)
            {
                string json = EditorPrefs.GetString(newKey, "");
                if (string.IsNullOrEmpty(json)) return defaultValue;

                try { return JsonUtility.FromJson<T>(json); }
                catch { return defaultValue; }
            }
            else
            {
                throw new NotSupportedException($"Type {typeof(T)} is not supported by GetEditorPref and isn't Serializable.");
            }
        }


        #region XML doc
        /// <summary>
        /// Deletes an EditorPref.
        /// </summary>
        /// <param name="key">The EditorPref's key (aka their "name").</param>
        /// <param name="localized">Whether the key was localized when saving the EditorPref.</param>
        #endregion
        public static void DeleteEditorPref(string key, bool localized = true)
        {
            if (!HasEditorPref(key, localized))
            {
                return;
            }


            string newKey;
            if (localized)
            {
                newKey = LocalizeString(key);

                DeTrackKey(newKey);
            }
            else
            {
                newKey = key;
            }

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
                if (HasEditorPref(key, false))
                {
                    EditorPrefs.DeleteKey(key);
                }
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
        public static string LocalizeString(string key)
        {
            return ProjectKey + "_" + key;
        }
        #endregion
    }
}
