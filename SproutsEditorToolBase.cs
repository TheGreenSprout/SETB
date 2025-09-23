using UnityEditor;
using UnityEngine;
using System;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;

//namespace SETB
//{

public class SproutsEditorToolBase : EditorWindow
{
    #region Variables
    protected enum AssetTypes
    {
        AudioClip,
        Texture2D,
        Sprite,
        Font
    }


    protected static string ProjectKey => Application.dataPath.GetHashCode().ToString();


    private Rect? originalRect = null;
    #endregion




    #region Creation
    protected static void CreateWindow<T>(string windowName, bool centered = false, bool locked = false, float minWidth = 0, float minHeight = 0, float maxWidth = 0, float maxHeight = 0) where T : EditorWindow
    {
        var window = GetWindow<T>();
        window.titleContent = new GUIContent(windowName);
        if (minWidth > 0 && minHeight > 0)
        {
            window.minSize = new Vector2(minWidth, minHeight);
        }
        if (maxWidth > 0 && maxHeight > 0)
        {
            window.maxSize = new Vector2(maxWidth, maxHeight);
        }
        window.Show();


        if (centered)
        {
            window.position = CenterWindow(window.position.width, window.position.height);
        }

        if (locked)
        {
            EditorApplication.delayCall += () =>
            {
                if (window != null)
                {
                    ((SproutsEditorToolBase)(object)window).originalRect = window.position; // now Unity has applied centering
                }
            };
        }
    }

    protected static void CreateUtilityWindow<T>(string windowName, bool centered = false, bool locked = false, float maxWidth = 0, float maxHeight = 0, float minWidth = 0, float minHeight = 0) where T : EditorWindow
    {
        var window = GetWindow<T>();
        window.titleContent = new GUIContent(windowName);
        if (minWidth > 0 && minHeight > 0)
        {
            window.minSize = new Vector2(minWidth, minHeight);
        }
        if (maxWidth > 0 && maxHeight > 0)
        {
            if (maxWidth == minWidth)
            {
                maxWidth += 0.2f;
            }
            if (maxHeight == minHeight)
            {
                maxHeight += 0.2f;
            }

            window.maxSize = new Vector2(maxWidth, maxHeight);
        }
        window.ShowUtility();

        if (centered)
        {
            window.position = CenterWindow(window.position.width, window.position.height);
        }

        if (locked)
        {
            EditorApplication.delayCall += () =>
            {
                if (window != null)
                {
                    ((SproutsEditorToolBase)(object)window).originalRect = window.position; // now Unity has applied centering
                }
            };
        }
    }
    #endregion



    #region Assets
    protected static T FindAssetByName<T>(string name, AssetTypes type) where T : UnityEngine.Object
    {
        UnityEngine.Object asset;

        switch (type)
        {
            case AssetTypes.AudioClip:
                asset = Resources.Load<AudioClip>(name);

                break;

            case AssetTypes.Texture2D:
                asset = Resources.Load<Texture2D>(name);

                break;

            case AssetTypes.Sprite:
                asset = Resources.Load<Sprite>(name);

                break;

            case AssetTypes.Font:
                asset = Resources.Load<Font>(name);

                break;

            default:
                Debug.LogWarning($"Unsupported AssetType {type}.");

                return null;
        }


        if (asset == null)
        {
            Debug.LogWarning($"Could not find sound asset at Resources/{name}.");
        }

        return asset as T;
    }


    protected static void TryPlaySound(AudioClip sound = null)
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
    protected static void TryPlaySound(string soundName = null)
    {
        AudioClip sound = FindAssetByName<AudioClip>(soundName, AssetTypes.AudioClip);

        TryPlaySound(sound);
    }
    #endregion



    #region Manage EditorPrefs
    protected static bool HasEditorPref(string key, bool localized = true)
    {
        return EditorPrefs.HasKey(localized ? LocalizeString(key) : key);
    }


    private static void TrackKey(string key)
    {
        string keyListKey = LocalizeString("EditorPrefsKeys");
        string allKeys = EditorPrefs.GetString(keyListKey, "");

        if (!allKeys.Contains(key))
        {
            allKeys += key + ";";
            EditorPrefs.SetString(keyListKey, allKeys);
        }
    }

    private static void DeTrackKey(string key)
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


    protected static void SetEditorPref<T>(string key, T value, bool localized = true)
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

    protected static T GetEditorPref<T>(string key, bool localized = true, T defaultValue = default)
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


    protected static void DeleteEditorPref(string key, bool localized = true)
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

    protected static void ClearAllLocalizedEditorPrefs()
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
    #endregion



    #region GUI Helpers
    #region Custom GUI customization classes
    public class List_GUILayoutOptions
    {
        public GUILayoutOption[] SearchOptions { get; set; } = Array.Empty<GUILayoutOption>();
        public GUILayoutOption[] HeaderOptions { get; set; } = Array.Empty<GUILayoutOption>();
        public GUILayoutOption[] ScrollOptions { get; set; } = Array.Empty<GUILayoutOption>();
        public GUILayoutOption[] ItemOptions { get; set; } = Array.Empty<GUILayoutOption>();

        public List_GUILayoutOptions()
        {
            SearchOptions = Array.Empty<GUILayoutOption>();
            HeaderOptions = Array.Empty<GUILayoutOption>();
            ScrollOptions = Array.Empty<GUILayoutOption>();
            ItemOptions = Array.Empty<GUILayoutOption>();
        }
    }
    public class List_GUIStyles
    {
        public GUIStyle SearchStyle { get; set; } = EditorStyles.textField;
        public GUIStyle HeaderStyle { get; set; } = EditorStyles.foldoutHeader;
        public GUIStyle ItemStyle { get; set; } = EditorStyles.label;

        public List_GUIStyles()
        {
            SearchStyle = EditorStyles.textField;
            HeaderStyle = EditorStyles.foldoutHeader;
            ItemStyle = EditorStyles.label;
        }
    }
    #endregion


    #region Layout Helpers
    protected void Indent(int indent)
    {
        EditorGUI.indentLevel = indent;
    }

    protected void Space(float size)
    {
        EditorGUILayout.Space(size);
    }

    protected void DrawSeparator(float thickness = 1f)
    {
        Rect rect = EditorGUILayout.GetControlRect(false, thickness);
        EditorGUI.DrawRect(rect, new Color(0.3f, 0.3f, 0.3f));
    }

    protected void FlexibleSpace()
    {
        GUILayout.FlexibleSpace();
    }

    protected void Horizontal(Action content, params GUILayoutOption[] options)
    {
        EditorGUILayout.BeginHorizontal(options);

        content?.Invoke();

        EditorGUILayout.EndHorizontal();
    }
    protected void Vertical(Action content, params GUILayoutOption[] options)
    {
        EditorGUILayout.BeginVertical(options);
        
        content?.Invoke();

        EditorGUILayout.EndVertical();
    }

    protected void GetControlRect(params GUILayoutOption[] options)
    {
        EditorGUILayout.GetControlRect(options);
    }

    protected void ScrollView(ref Vector2 scrollPos, Action content, params GUILayoutOption[] options)
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, options);

        content?.Invoke();

        EditorGUILayout.EndScrollView();
    }
    #endregion


    #region Text Display
    protected void DrawHelpBox(string message, MessageType type = MessageType.Info)
    {
        EditorGUILayout.HelpBox(message, type);
    }
    
    protected void DrawLabel(string text, GUIStyle style = null, params GUILayoutOption[] options)
    {
        if (style == null)
        {
            style = EditorStyles.label;
        }

        EditorGUILayout.LabelField(text, style, options);
    }
    protected void DrawSelectableLabel(string text, GUIStyle style = null, params GUILayoutOption[] options)
    {
        if (style == null)
        {
            style = EditorStyles.label;
        }

        EditorGUILayout.SelectableLabel(text, style, options);
    }

    protected void DrawTextArea(ref string text)
    {
        text = EditorGUILayout.TextArea(text);
    }
    #endregion


    #region Value Changers
    protected void DrawToggle(string label, ref bool value, GUIStyle style = null, params GUILayoutOption[] options)
    {
        if (style == null)
        {
            style = EditorStyles.toggle;
        }

        value = EditorGUILayout.Toggle(label, value, style, options);
    }
    protected void DrawToggleLeft(string label, ref bool value, GUIStyle style = null, params GUILayoutOption[] options)
    {
        if (style == null)
        {
            style = EditorStyles.toggle;
        }

        value = EditorGUILayout.ToggleLeft(label, value, style, options);
    }

    protected void DrawSelectionPopup<T>(ref T value, string[] intOrGeneric_options = null, int[] int_optionalOptions = null, GUIStyle style = null, params GUILayoutOption[] options)
    {
        if (style == null)
        {
            style = EditorStyles.popup;
        }

        Type t = typeof(T);
        switch (t)
        {
            case Type _ when t == typeof(Enum):
                value = (T)(object)EditorGUILayout.EnumPopup((Enum)(object)value, style, options);
                break;

            case Type _ when t == typeof(int):
                if (int_optionalOptions != null)
                {
                    value = (T)(object)EditorGUILayout.IntPopup((int)(object)value, intOrGeneric_options, int_optionalOptions, style, options);
                }
                else
                {
                    value = (T)(object)EditorGUILayout.Popup((int)(object)value, intOrGeneric_options, style, options);
                }
                break;

            default:
                throw new NotSupportedException($"Unsupported type: {t.Name} for DrawSelectionPopup<{typeof(T)}>");
        }
    }

    protected void DrawInputField<T>(string label, ref T field, GUIStyle style = null, bool delayed = false, bool layerOrMask = false, string[] displayedMaskOptions = null, bool? passwordOrTag = null, bool labelField = false, bool allowSceneObjects = true, params GUILayoutOption[] options)
    {
        Type t = typeof(T);
        switch (t)
        {
            case Type _ when t == typeof(string):
                if (style == null)
                {
                    style = EditorStyles.textField;
                }
                
                if (passwordOrTag != null)
                {
                    if (passwordOrTag == true)
                    {
                        field = (T)(object)EditorGUILayout.PasswordField(label, (string)(object)field, style, options);
                    }
                    else
                    {
                        field = (T)(object)EditorGUILayout.TagField(label, (string)(object)field, style, options);
                    }
                }
                else
                {
                    if (labelField)
                    {
                        EditorGUILayout.LabelField(label, (string)(object)field, style, options);
                    }
                    else
                    {
                        if (delayed)
                        {
                            field = (T)(object)EditorGUILayout.DelayedTextField(label, (string)(object)field, style, options);
                        }
                        else
                        {
                            field = (T)(object)EditorGUILayout.TextField(label, (string)(object)field, style, options);
                        }
                    }
                }
                break;

            case Type _ when t == typeof(int):
                if (style == null)
                {
                    style = EditorStyles.numberField;
                }
                
                if (layerOrMask)
                {
                    if (displayedMaskOptions == null)
                    {
                        field = (T)(object)EditorGUILayout.LayerField(label, (int)(object)field, style, options);
                    }
                    else
                    {
                        field = (T)(object)EditorGUILayout.MaskField(new GUIContent(label), (int)(object)field, displayedMaskOptions, style, options);
                    }
                }
                else
                {
                    if (delayed)
                    {
                        field = (T)(object)EditorGUILayout.DelayedIntField(label, (int)(object)field, style, options);
                    }
                    else
                    {
                        field = (T)(object)EditorGUILayout.IntField(label, (int)(object)field, style, options);
                    }
                }
                break;
            case Type _ when t == typeof(long):
                if (style == null)
                {
                    style = EditorStyles.numberField;
                }
                
                field = (T)(object)EditorGUILayout.LongField(label, (long)(object)field, style, options);
                break;

            case Type _ when t == typeof(float):
                if (style == null)
                {
                    style = EditorStyles.numberField;
                }
                
                if (delayed)
                {
                    field = (T)(object)EditorGUILayout.DelayedFloatField(label, (float)(object)field, style, options);
                }
                else
                {
                    field = (T)(object)EditorGUILayout.FloatField(label, (float)(object)field, style, options);
                }
                break;
            case Type _ when t == typeof(double):
                if (style == null)
                {
                    style = EditorStyles.numberField;
                }
                
                if (delayed)
                {
                    field = (T)(object)EditorGUILayout.DelayedDoubleField(label, (double)(object)field, style, options);
                }
                else
                {
                    field = (T)(object)EditorGUILayout.DoubleField(label, (double)(object)field, style, options);
                }
                break;

            case Type _ when t == typeof(Vector2):
                if (style == null)
                {
                    style = EditorStyles.numberField;
                }
                
                field = (T)(object)EditorGUILayout.Vector2Field(label, (Vector2)(object)field, options);
                break;
            case Type _ when t == typeof(Vector2Int):
                if (style == null)
                {
                    style = EditorStyles.numberField;
                }
                
                field = (T)(object)EditorGUILayout.Vector2IntField(label, (Vector2Int)(object)field, options);
                break;

            case Type _ when t == typeof(Vector3):
                if (style == null)
                {
                    style = EditorStyles.numberField;
                }
                
                field = (T)(object)EditorGUILayout.Vector3Field(label, (Vector3)(object)field, options);
                break;
            case Type _ when t == typeof(Vector3Int):
                if (style == null)
                {
                    style = EditorStyles.numberField;
                }
                
                field = (T)(object)EditorGUILayout.Vector3IntField(label, (Vector3Int)(object)field, options);
                break;

            case Type _ when t == typeof(Vector4):
                if (style == null)
                {
                    style = EditorStyles.numberField;
                }
                
                field = (T)(object)EditorGUILayout.Vector4Field(label, (Vector4)(object)field, options);
                break;

            case Type _ when t == typeof(Color):
                if (style == null)
                {
                    style = EditorStyles.colorField;
                }
                
                field = (T)(object)EditorGUILayout.ColorField(label, (Color)(object)field, options);
                break;
            case Type _ when t == typeof(Gradient):
                if (style == null)
                {
                    style = EditorStyles.colorField;
                }
                
                field = (T)(object)EditorGUILayout.GradientField(label, (Gradient)(object)field, options);
                break;

            case Type _ when t == typeof(Rect):
                if (style == null)
                {
                    style = EditorStyles.numberField;
                }
                
                field = (T)(object)EditorGUILayout.RectField(label, (Rect)(object)field, options);
                break;
            case Type _ when t == typeof(RectInt):
                if (style == null)
                {
                    style = EditorStyles.numberField;
                }
                
                field = (T)(object)EditorGUILayout.RectIntField((RectInt)(object)field, options);
                break;

            case Type _ when t == typeof(Bounds):
                if (style == null)
                {
                    style = EditorStyles.numberField;
                }
                
                field = (T)(object)EditorGUILayout.BoundsField(label, (Bounds)(object)field, options);
                break;
            case Type _ when t == typeof(BoundsInt):
                if (style == null)
                {
                    style = EditorStyles.numberField;
                }
                
                field = (T)(object)EditorGUILayout.BoundsIntField(label, (BoundsInt)(object)field, options);
                break;

            case Type _ when t == typeof(Enum):
                if (style == null)
                {
                    style = EditorStyles.textField;
                }
                
                field = (T)(object)EditorGUILayout.EnumFlagsField(label, (Enum)(object)field, style, options);
                break;

            case Type _ when t == typeof(AnimationCurve):
                if (style == null)
                {
                    style = EditorStyles.textField;
                }
                
                field = (T)(object)EditorGUILayout.CurveField(label, (AnimationCurve)(object)field, options);
                break;

            case Type _ when t == typeof(uint):
                if (style == null)
                {
                    style = EditorStyles.numberField;
                }
                
                field = (T)(object)EditorGUILayout.RenderingLayerMaskField(label, (uint)(object)field, style, options);
                break;

            case Type _ when typeof(UnityEngine.Object).IsAssignableFrom(t):
                if (style == null)
                {
                    style = EditorStyles.objectField;
                }
                
                field = (T)(object)EditorGUILayout.ObjectField(label, (UnityEngine.Object)(object)field, ((UnityEngine.Object)(object)field).GetType(), allowSceneObjects, options);
                break;

            case Type _ when t == typeof(SerializedProperty):
                if (style == null)
                {
                    style = EditorStyles.textField;
                }
                
                field = (T)(object)EditorGUILayout.PropertyField((SerializedProperty)(object)field, new GUIContent(label), options);
                break;

            default:
                throw new NotSupportedException($"Unsupported type: {t.Name} for DrawSelectionPopup<{typeof(T)}>");
        }
    }
    #endregion


    #region With Logic
    protected void DrawButton(string buttonName, Action logic, GUIStyle style = null, params GUILayoutOption[] options)
    {
        if (style == null)
        {
            style = EditorStyles.iconButton;
        }

        if (GUILayout.Button(buttonName, style, options))
        {
            logic?.Invoke();
        }
    }

    protected void DrawToggleGroup(string title, ref bool state, Action logic)
    {
        state = EditorGUILayout.BeginToggleGroup(title, state);
        if (state)
        {
            logic?.Invoke();
        }
        EditorGUILayout.EndToggleGroup();
    }

    protected void DrawFoldout(string title, ref bool state, Action logic, GUIStyle style = null)
    {
        if (style == null)
        {
            style = EditorStyles.foldout;
        }

        if (state = EditorGUILayout.Foldout(state, title, true, style))
        {
            logic?.Invoke();
        }
    }
    protected void DrawFoldoutHeaderGroup(string title, ref bool state, Action logic, GUIStyle style = null, Action<Rect> menuAction = null, GUIStyle menuIcon = null)
    {
        if (style == null)
        {
            style = EditorStyles.foldoutHeader;
        }

        state = EditorGUILayout.BeginFoldoutHeaderGroup(state, title, style, menuAction, menuIcon);
        if (state)
        {
            logic?.Invoke();
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
    }
    #endregion


    #region Complex
    private float GetListItemMatchScore(string item, string search)
    {
        if (string.IsNullOrEmpty(search))
            return 0; // no search, keep natural order


        item = item.ToLower();
        search = search.ToLower();


        float score = 0;
        bool canBePrefix = true;

        for (int start = 0; start < search.Length; start++)
        {
            for (int length = 1; length <= search.Length - start; length++)
            {
                string substring = search.Substring(start, length);

                if (item == substring && canBePrefix) score += 1; // exact match
                else if (item.StartsWith(substring) && canBePrefix) score += .75f; // prefix match
                else if (item.Contains(substring)) score += .5f; // substring match
            }

            canBePrefix = false;
        }

        
        return score;
    }
    
    protected void DrawList<T>(string label, object items, ref T foldoutOrScrollVar, List_GUIStyles styles = null, List_GUILayoutOptions options = null)
    {
        styles ??= new List_GUIStyles();
        options ??= new List_GUILayoutOptions();


        Type t = typeof(T);
        switch (t)
        {
            case Type _ when t == typeof(bool):
                bool foldoutBool = (bool)(object)foldoutOrScrollVar;

                DrawFoldout(label, ref foldoutBool, () =>
                {
                    DrawList(null, items, styles, options);
                }, styles.HeaderStyle);

                foldoutOrScrollVar = (T)(object)foldoutBool;
                break;
                
            case Type _ when t == typeof(Vector2):
                DrawLabel(label, styles.HeaderStyle, options.HeaderOptions);


                Vector2 scrollVector = (Vector2)(object)foldoutOrScrollVar;
                
                ScrollView(ref scrollVector, () =>
                {
                    DrawList(null, items, styles, options);
                }, options.ScrollOptions);

                foldoutOrScrollVar = (T)(object)scrollVector;
                break;
                
            default:
                throw new NotSupportedException($"Unsupported type: {t.Name} for DrawList<{typeof(T)}>");
        }
    }
    protected void DrawList(string label, object items, ref bool foldoutBool, ref Vector2 scrollVector, List_GUIStyles styles = null, List_GUILayoutOptions options = null)
    {
        styles ??= new List_GUIStyles();
        options ??= new List_GUILayoutOptions();

        
        Vector2 scroll = scrollVector;
        DrawFoldout(label, ref foldoutBool, () =>
        {
            ScrollView(ref scroll, () =>
            {
                DrawList(null, items, styles, options);
            }, options.ScrollOptions);
        }, styles.HeaderStyle);
        scrollVector = scroll;
    }
    protected void DrawList(string label, object items, List_GUIStyles styles = null, List_GUILayoutOptions options = null)
    {
        styles ??= new List_GUIStyles();
        options ??= new List_GUILayoutOptions();

        
        if (!string.IsNullOrEmpty(label))
        {
            DrawLabel(label, styles.HeaderStyle, options.HeaderOptions);
        }

        Vertical(() =>
        {
            switch (items)
            {
                case string[] strArray:
                    foreach (var item in strArray)
                        DrawLabel(item, styles.ItemStyle, options.ItemOptions);
                    break;

                case List<string> strList:
                    foreach (var item in strList)
                        DrawLabel(item, styles.ItemStyle, options.ItemOptions);
                    break;

                case Dictionary<string, Action> dict:
                    foreach (var kvp in dict)
                        DrawButton(kvp.Key, kvp.Value, styles.ItemStyle, options.ItemOptions);
                    break;

                default:
                    throw new NotSupportedException("Unsupported collection type for DrawList.");
            }
        });
    }

    private T DrawSearchableList_Logic<T>(T items, string searchStr)
    {
        object itemsObj = (object)items;

        string localSearchVal = searchStr;


        switch (itemsObj)
        {
            case string[] strArray:
                var rankedArray = strArray
                    .Select((i, index) => new { Item = i, Score = GetListItemMatchScore(i, localSearchVal), Index = index })
                    .ToList();

                if (rankedArray.Any(x => x.Score > 0))
                {
                    strArray = rankedArray
                        .OrderByDescending(x => x.Score)
                        .ThenBy(x => x.Index) // preserve original order for ties
                        .Select(x => x.Item)  // extract the string
                        .ToArray();
                }

                itemsObj = strArray;
                break;

            case List<string> strList:
                var rankedList = strList
                    .Select((i, index) => new { Item = i, Score = GetListItemMatchScore(i, localSearchVal), Index = index })
                    .ToList();

                if (rankedList.Any(x => x.Score > 0))
                {
                    strList = rankedList
                        .OrderByDescending(x => x.Score)
                        .ThenBy(x => x.Index) // preserve original order for ties
                        .Select(x => x.Item)  // extract the string
                        .ToList();
                }

                itemsObj = strList;
                break;

            case Dictionary<string, Action> dict:
                // Rank the keys by match score
                var ranked = dict
                    .Select((kvp, index) => new
                    {
                        Key = kvp.Key,
                        Value = kvp.Value,
                        Score = GetListItemMatchScore(kvp.Key, localSearchVal),
                        Index = index
                    })
                    .ToList();

                if (ranked.Any(x => x.Score > 0))
                {
                    dict = ranked
                        .OrderByDescending(x => x.Score)
                        .ThenBy(x => x.Index) // preserve original order for ties
                        .ToDictionary(x => x.Key, x => x.Value);
                }

                itemsObj = dict;
                break;

            default:
                throw new NotSupportedException("Unsupported collection type for DrawList.");
        }


        return (T)itemsObj;
    }
    protected void DrawSearchableList<T>(string label, string searchLabel, ref T items, ref string searchStr, bool delayedSearch = false, List_GUIStyles styles = null, List_GUILayoutOptions options = null)
    {
        styles ??= new List_GUIStyles();
        options ??= new List_GUILayoutOptions();

        
        DrawInputField(searchLabel, ref searchStr, styles.SearchStyle, delayedSearch, false, null, null, false, true, options.SearchOptions);


        items = DrawSearchableList_Logic(items, searchStr);


        DrawList(label, items, styles, options);
    }
    protected void DrawSearchableList<T>(string label, string searchLabel, ref T items, ref string searchStr, ref bool foldoutBool, bool delayedSearch = false, List_GUIStyles styles = null, List_GUILayoutOptions options = null)
    {
        styles ??= new List_GUIStyles();
        options ??= new List_GUILayoutOptions();

        
        DrawInputField(searchLabel, ref searchStr, styles.SearchStyle, delayedSearch, false, null, null, false, true, options.SearchOptions);


        items = DrawSearchableList_Logic(items, searchStr);


        DrawList(label, items, ref foldoutBool, styles, options);
    }
    protected void DrawSearchableList<T>(string label, string searchLabel, ref T items, ref string searchStr, ref Vector2 scrollVector, bool delayedSearch = false, List_GUIStyles styles = null, List_GUILayoutOptions options = null)
    {
        styles ??= new List_GUIStyles();
        options ??= new List_GUILayoutOptions();

        
        DrawInputField(searchLabel, ref searchStr, styles.SearchStyle, delayedSearch, false, null, null, false, true, options.SearchOptions);


        items = DrawSearchableList_Logic(items, searchStr);


        DrawList(label, items, ref scrollVector, styles, options);
    }
    protected void DrawSearchableList<T>(string label, string searchLabel, ref T items, ref string searchStr, ref bool foldoutBool, ref Vector2 scrollVector, bool delayedSearch = false, List_GUIStyles styles = null, List_GUILayoutOptions options = null)
    {
        styles ??= new List_GUIStyles();
        options ??= new List_GUILayoutOptions();

        
        DrawInputField(searchLabel, ref searchStr, styles.SearchStyle, delayedSearch, false, null, null, false, true, options.SearchOptions);


        items = DrawSearchableList_Logic(items, searchStr);


        DrawList(label, items, ref foldoutBool, ref scrollVector, styles, options);
    }

    protected void DrawLabeledSlider<T>(string label, ref T value, T min, T max, bool resetValuePresent = false, T resetValue = default, GUIStyle resetButtonStyle = null, params GUILayoutOption[] options)
    {
        EditorGUILayout.BeginHorizontal();

        Type t = typeof(T);
        if (t == typeof(short) || t == typeof(int) || t == typeof(long))
        {
            value = (T)(object)EditorGUILayout.IntSlider(new GUIContent(label), (int)(object)value, (int)(object)min, (int)(object)max, options);

            if (resetValuePresent)
            {
                T val = value;

                DrawButton("Reset", () =>
                {
                    val = resetValue;
                }, resetButtonStyle, options);

                value = val;
            }
        }
        else if (t == typeof(float) || t == typeof(double))
        {
            value = (T)(object)EditorGUILayout.Slider(label, (float)(object)value, (float)(object)min, (float)(object)max, options);

            if (!Mathf.Approximately((float)(object)resetValue, default))
            {
                T val = value;

                DrawButton("Reset", () =>
                {
                    val = resetValue;
                }, resetButtonStyle, options);

                value = val;
            }
        }
        else
        {
            throw new NotSupportedException($"Type {t} is not supported by DrawLabeledSlider.");
        }

        EditorGUILayout.EndHorizontal();
    }
    #endregion
    #endregion



    #region Misc
    protected class PopupOptions
    {
        public string Title { get; set; } = "PopUp";

        public string CloseButtonText { get; set; } = "OK";


        public AudioClip Sound { get; set; } = null;

        public bool Silent { get; set; } = false;


        public Texture2D Image { get; set; } = null;

        public float ImageWidth { get; set; } = 64;
        public float ImageHeight { get; set; } = 64;


        public bool Centered { get; set; } = true;
        public bool Locked { get; set; } = true;
    }

    protected static void PopupWindow(string message, float width, float height, PopupOptions options)
    {
        if (options == null)
        {
            options = new PopupOptions();
        }

        CreateInstance<BasicPopup>().CreatePopup(message, width, height, options.Title, options.CloseButtonText, options.Sound, options.Silent, options.Image, options.ImageWidth, options.ImageHeight, options.Centered, options.Locked);
    }


    private static Rect GetEditorMainWindowPos()
    {
        return EditorGUIUtility.GetMainWindowPosition();
    }

    private static Rect CenterWindow(float width, float height)
    {
        Rect main = GetEditorMainWindowPos();

        float x = main.x + (main.width - width) * 0.5f;
        float y = main.y + (main.height - height) * 0.5f;

        return new Rect(x, y, width, height);
    }


    protected static string LocalizeString(string key)
    {
        return ProjectKey + "_" + key;
    }
    #endregion




    #region Unity Methods
    protected virtual void OnEnable()
    {
        LoadPrefsOnEnable();
    }
    private void LoadPrefsOnEnable()
    {
        FieldInfo[] fields = GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        foreach (var field in fields)
        {
            var attr = field.GetCustomAttribute<EditorPrefAttribute>();
            if (attr == null) continue;

            Type fieldType = field.FieldType;
            object defaultValue = attr.DefaultValue;

            // Use reflection to call your generic GetEditorPref<T>
            MethodInfo method = typeof(SproutsEditorToolBase)
                .GetMethod(nameof(GetEditorPref), BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public)
                .MakeGenericMethod(fieldType);

            object value = method.Invoke(null, new object[] { attr.Key, true, defaultValue });
            field.SetValue(this, value);
        }
    }

    protected virtual void OnDisable()
    {
        SavePrefsOnDisable();
    }
    private void SavePrefsOnDisable()
    {
        FieldInfo[] fields = GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        foreach (var field in fields)
        {
            var attr = field.GetCustomAttribute<EditorPrefAttribute>();
            if (attr == null) continue;

            Type fieldType = field.FieldType;
            object value = field.GetValue(this);

            // Use reflection to call your generic SetEditorPref<T>
            MethodInfo method = typeof(SproutsEditorToolBase)
                .GetMethod(nameof(SetEditorPref), BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public)
                .MakeGenericMethod(fieldType);

            method.Invoke(null, new object[] { attr.Key, value, true });
        }
    }


    protected virtual void Update()
    {
        if (originalRect.HasValue)
        {
            position = new Rect(originalRect.Value.x, originalRect.Value.y, position.width, position.height);
        }
    }
    #endregion




    #region Popup Window
    private class BasicPopup : SproutsEditorToolBase
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


            CreateUtilityWindow<BasicPopup>(title, centered, locked, width, height, width, height);


            if (!silentWindow)
            {
                TryPlaySound(sound);
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


        public EditorPrefAttribute(string key, object defaultValue = default)
        {
            Key = key;

            DefaultValue = defaultValue;
        }
    }
    #endregion
}

//}