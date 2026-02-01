using UnityEditor;
using UnityEngine;
using System;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;

namespace SETB
{

#region XML doc
/// <summary>
/// Class containing all the basics (and no so basics) for making editor tools.
/// </summary>
#endregion
public class EditorToolBase : EditorWindow
{
    #region Variables
    private Rect? originalRect = null;



    private string cacheSaveStr = "";

    private Dictionary<string, float> cacheScoreDictionary = new Dictionary<string, float>();
    #endregion




    #region Creation
    #region XML doc
    /// <summary>
    /// Instantiates a normal Unity Editor window.
    /// </summary>
    /// <param name="windowName">The name of the window.</param>
    /// <param name="centered">Whether the window is center when first appearing.</param>
    /// <param name="locked">Whether the window is locked in place (can't be moved).</param>
    /// <param name="minWidth">The minimmum width of the window.</param>
    /// <param name="minHeight">The minimmum height of the window.</param>
    /// <param name="maxWidth">The maximmum width of the window (if equal to minWidth, the user won't be able to chage the window width).</param>
    /// <param name="maxHeight">The maximmum height of the window (if equal to minHeight, the user won't be able to chage the window height).</param>
    #endregion
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
            /*if (maxWidth == minWidth)
            {
                maxWidth += 0.2f;
            }
            if (maxHeight == minHeight)
            {
                maxHeight += 0.2f;
            }*/

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
                    ((EditorToolBase)(object)window).originalRect = window.position; // now Unity has applied centering
                }
            };
        }
    }

    #region XML doc
    /// <summary>
    /// Instantiates a utility Unity Editor window.
    /// </summary>
    /// <param name="windowName">The name of the window.</param>
    /// <param name="centered">Whether the window is center when first appearing.</param>
    /// <param name="locked">Whether the window is locked in place (can't be moved).</param>
    /// <param name="minWidth">The minimmum width of the window.</param>
    /// <param name="minHeight">The minimmum height of the window.</param>
    /// <param name="maxWidth">The maximmum width of the window (if equal to minWidth, the user won't be able to chage the window width).</param>
    /// <param name="maxHeight">The maximmum height of the window (if equal to minHeight, the user won't be able to chage the window height).</param>
    #endregion
    protected static void CreateUtilityWindow<T>(string windowName, bool centered = false, bool locked = false, float minWidth = 0, float minHeight = 0, float maxWidth = 0, float maxHeight = 0) where T : EditorWindow
    {
        var window = GetWindow<T>();
        window.titleContent = new GUIContent(windowName);
        if (minWidth > 0 && minHeight > 0)
        {
            window.minSize = new Vector2(minWidth, minHeight);
        }
        if (maxWidth > 0 && maxHeight > 0)
        {
            /*if (maxWidth == minWidth)
            {
                maxWidth += 0.2f;
            }
            if (maxHeight == minHeight)
            {
                maxHeight += 0.2f;
            }*/

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
                    ((EditorToolBase)(object)window).originalRect = window.position; // now Unity has applied centering
                }
            };
        }
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
        #region XML doc
        /// <summary>
        /// Sets an indent level.
        /// </summary>
        /// <param name="indent">The indent level to set.</param>
        #endregion
        protected void Indent(int indent)
        {
            EditorGUI.indentLevel = indent;
        }

        #region XML doc
        /// <summary>
        /// Creates a space.
        /// </summary>
        /// <param name="size">The size of the space.</param>
        #endregion
        protected void Space(float size)
        {
            EditorGUILayout.Space(size);
        }

        #region XML doc
        /// <summary>
        /// Creates a separator.
        /// </summary>
        /// <param name="thickness">The thickness of the separator.</param>
        #endregion
        protected void DrawSeparator(float thickness = 1f)
        {
            Rect rect = EditorGUILayout.GetControlRect(false, thickness);
            EditorGUI.DrawRect(rect, new Color(0.3f, 0.3f, 0.3f));
        }

        #region XML doc
        /// <summary>
        /// Instantiates a flexible space.
        /// </summary>
        #endregion
        protected void FlexibleSpace()
        {
            GUILayout.FlexibleSpace();
        }

        #region XML doc
        /// <summary>
        /// Creates a horizontal area.
        /// </summary>
        /// <param name="content">The logic to run inside this area.</param>
        /// <param name="options">The horizontal area's GUILayoutOptions.</param>
        #endregion
        protected void Horizontal(Action content, params GUILayoutOption[] options)
        {
            EditorGUILayout.BeginHorizontal(options);

            content?.Invoke();

            EditorGUILayout.EndHorizontal();
        }
        #region XML doc
        /// <summary>
        /// Creates a vertical area.
        /// </summary>
        /// <param name="content">The logic to run inside this area.</param>
        /// <param name="options">The vertical area's GUILayoutOptions.</param>
        #endregion
        protected void Vertical(Action content, params GUILayoutOption[] options)
        {
            EditorGUILayout.BeginVertical(options);
            
            content?.Invoke();

            EditorGUILayout.EndVertical();
        }

        #region XML doc
        /// <summary>
        /// Gets a rect for an Editor control.
        /// </summary>
        /// <param name="options">The GUILayoutOptions of the rect.</param>
        /// <returns>Returns the rect.</returns>
        #endregion
        protected Rect GetControlRect(params GUILayoutOption[] options)
        {
            return EditorGUILayout.GetControlRect(options);
        }

        #region XML doc
        /// <summary>
        /// Creates a scroll view.
        /// </summary>
        /// <param name="scrollPos">The Vector2 that controlls the scroller.</param>
        /// <param name="content">The logic to run inside this view.</param>
        /// <param name="options">The scroll view's GUILayoutOptions.</param>
        #endregion
        protected void ScrollView(ref Vector2 scrollPos, Action content, params GUILayoutOption[] options)
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, options);

            content?.Invoke();

            EditorGUILayout.EndScrollView();
        }
        #endregion


        #region Text Display
        #region XML doc
        /// <summary>
        /// Creates a help box.
        /// </summary>
        /// <param name="message">The message to display on this help box.</param>
        /// <param name="type">The MessageType of this help box.</param>
        #endregion
        protected void DrawHelpBox(string message, MessageType type = MessageType.Info)
        {
            EditorGUILayout.HelpBox(message, type);
        }
        
        #region XML doc
        /// <summary>
        /// Creates a label.
        /// </summary>
        /// <param name="text">The text to display on this label.</param>
        /// <param name="style">The GUIStyle of the label.</param>
        /// <param name="options">The label's GUILayoutOptions.</param>
        #endregion
        protected void DrawLabel(string text, GUIStyle style = null, params GUILayoutOption[] options)
        {
            if (style == null)
            {
                style = EditorStyles.label;
            }

            EditorGUILayout.LabelField(text, style, options);
        }
        #region XML doc
        /// <summary>
        /// Creates a selectable label.
        /// </summary>
        /// <param name="text">The text to display on this label.</param>
        /// <param name="style">The GUIStyle of the label.</param>
        /// <param name="options">The label's GUILayoutOptions.</param>
        #endregion
        protected void DrawSelectableLabel(string text, GUIStyle style = null, params GUILayoutOption[] options)
        {
            if (style == null)
            {
                style = EditorStyles.label;
            }

            EditorGUILayout.SelectableLabel(text, style, options);
        }

        #region XML doc
        /// <summary>
        /// Creates a text area.
        /// </summary>
        /// <param name="text">The text to display on this area.</param>
        #endregion
        protected void DrawTextArea(ref string text)
        {
            text = EditorGUILayout.TextArea(text);
        }
        #endregion


        #region Value Changers
        #region XML doc
        /// <summary>
        /// Creates a toggle.
        /// </summary>
        /// <param name="label">The name of this toggle.</param>
        /// <param name="value">The boolean changed by the toggle.</param>
        /// <param name="style">The GUIStyle of the toggle.</param>
        /// <param name="options">The toggle's GUILayoutOptions.</param>
        #endregion
        protected void DrawToggle(string label, ref bool value, GUIStyle style = null, params GUILayoutOption[] options)
        {
            if (style == null)
            {
                style = EditorStyles.toggle;
            }

            value = EditorGUILayout.Toggle(label, value, style, options);
        }
        #region XML doc
        /// <summary>
        /// Creates a left-bound toggle.
        /// </summary>
        /// <param name="label">The name of this toggle.</param>
        /// <param name="value">The boolean changed by the toggle.</param>
        /// <param name="style">The GUIStyle of the toggle.</param>
        /// <param name="options">The toggle's GUILayoutOptions.</param>
        #endregion
        protected void DrawToggleLeft(string label, ref bool value, GUIStyle style = null, params GUILayoutOption[] options)
        {
            if (style == null)
            {
                style = EditorStyles.toggle;
            }

            value = EditorGUILayout.ToggleLeft(label, value, style, options);
        }

        #region XML doc
        /// <summary>
        /// Creates a selection popup
        /// </summary>
        /// <param name="value">The value to be changed by the selection.</param>
        /// <param name="intOrGeneric_options">The different options to select (only for int selections).</param>
        /// <param name="int_optionalOptions">The different optional options to select (only for int selections).</param>
        /// <param name="style">The GUIStyle of the popup.</param>
        /// <param name="options">The popup's GUILayoutOptions.</param>
        #endregion
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

        #region XML doc
        /// <summary>
        /// Creates an input field.
        /// </summary>
        /// <param name="label">The name of the input field.</param>
        /// <param name="field">The variable to be changed by the input.</param>
        /// <param name="style">The GUIStyle of the field.</param>
        /// <param name="delayed">Whether the field waits for the user to press enter/click away from the field to change the variable or not.</param>
        /// <param name="layerOrMask">Whether an int field is to be displayed as a layer/mask input or not.</param>
        /// <param name="displayedMaskOptions">The displayed mask options.</param>
        /// <param name="passwordOrTag">Whether a string field is to be displayed as a password/tag input or not.</param>
        /// <param name="labelField">Whether a string field is to be displayed as a label input or not.</param>
        /// <param name="allowSceneObjects">Whether to allow scene objects for Object fields or not.</param>
        /// <param name="options">The field's GUILayoutOptions.</param>
        #endregion
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
        #region XML doc
        /// <summary>
        /// Creates a button.
        /// </summary>
        /// <param name="buttonName">The name of the button.</param>
        /// <param name="logic">The logic to be run by the button.</param>
        /// <param name="style">The GUIStyle of the button.</param>
        /// <param name="options">The button's GUILayoutOptions.</param>
        #endregion
        protected void DrawButton(string buttonName, Action logic, GUIStyle style = null, params GUILayoutOption[] options)
        {
            if (style == null)
            {
                if (GUILayout.Button(buttonName, options))
                {
                    logic?.Invoke();
                }
            }
            else
            {
                if (GUILayout.Button(buttonName, style, options))
                {
                    logic?.Invoke();
                }
            }
        }

        #region XML doc
        /// <summary>
        /// Creates a toggle group.
        /// </summary>
        /// <param name="title">The title of the group.</param>
        /// <param name="state">The boolean that controls the toggle group.</param>
        /// <param name="logic">The logic to be run inside the toggle group.</param>
        #endregion
        protected void DrawToggleGroup(string title, ref bool state, Action logic)
        {
            state = EditorGUILayout.BeginToggleGroup(title, state);
            if (state)
            {
                logic?.Invoke();
            }
            EditorGUILayout.EndToggleGroup();
        }

        #region XML doc
        /// <summary>
        /// Creates a foldout.
        /// </summary>
        /// <param name="title">The title of the foldout.</param>
        /// <param name="state">The boolean that controls the foldout.</param>
        /// <param name="logic">The logic to be run inside the foldout.</param>
        /// <param name="style">The GUIStyle of the foldout.</param>
        #endregion
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
        #region XML doc
        /// <summary>
        /// Creates a foldout header group.
        /// </summary>
        /// <param name="title">The title of the group.</param>
        /// <param name="state">The boolean that controls the group.</param>
        /// <param name="logic">The logic to be run inside the group.</param>
        /// <param name="style">The GUIStyle of the group.</param>
        /// <param name="menuAction">The menu action.</param>
        /// <param name="menuIcon">The GUIStyle of the menuIcon.</param>
        #endregion
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
        #region XML doc
        /// <summary>
        /// Gets the match score of a list's item based on how close it is to the search string.
        /// </summary>
        /// <param name="item">The item string.</param>
        /// <param name="search">The search string</param>
        /// <returns>Returns the item's match score.</returns>
        #endregion
        private float GetSearchItemMatchScore(string item, string search)
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

            if (score > 0)
            {
                CacheItemScore(item, score);
            }

            
            return score;
        }
        #region XML doc
        /// <summary>
        /// Caches an item's score to be used later.
        /// </summary>
        /// <param name="item">The item string.</param>
        /// <param name="score">The item's match score.</param>
        #endregion
        private void CacheItemScore(string item, float score)
        {
            foreach (string key in cacheScoreDictionary.Keys)
            {
                if (item.Equals(key))
                {
                    return;
                }
            }

            cacheScoreDictionary.Add(item, score);
        }
        #region XML doc
        /// <summary>
        /// Gets the match score of a list's item from it's cached match score.
        /// </summary>
        /// <param name="item">The item string.</param>
        /// <returns>Returns the match score.</returns>
        #endregion
        private float GetListItemMatchScore(string item)
        {
            foreach (KeyValuePair<string, float> cachedItem in cacheScoreDictionary)
            {
                if (item.Equals(cachedItem.Key))
                {
                    return cachedItem.Value;
                }
            }

            return 0f;
        }
        #region XML doc
        /// <summary>
        /// Gets the match score of all the items on a list.
        /// </summary>
        /// <param name="itemList">The list of items.</param>
        /// <param name="search">The search string.</param>
        #endregion
        private void GetListItemMatchScore_Master(ref object itemList, string search)
        {
            if (search != cacheSaveStr)
            {
                cacheSaveStr = search;
                cacheScoreDictionary = new Dictionary<string, float>();

                
                string localSearchVal = search;


                switch (itemList)
                {
                    case string[] strArray:
                        var rankedArray = strArray
                            .Select((i, index) => new { Item = i, Score = GetSearchItemMatchScore(i, localSearchVal), Index = index })
                            .ToList();

                        if (rankedArray.Any(x => x.Score > 0))
                        {
                            strArray = rankedArray
                                .OrderByDescending(x => x.Score)
                                .ThenBy(x => x.Index) // preserve original order for ties
                                .Select(x => x.Item)  // extract the string
                                .ToArray();
                        }

                        itemList = strArray;
                        break;

                    case List<string> strList:
                        var rankedList = strList
                            .Select((i, index) => new { Item = i, Score = GetSearchItemMatchScore(i, localSearchVal), Index = index })
                            .ToList();

                        if (rankedList.Any(x => x.Score > 0))
                        {
                            strList = rankedList
                                .OrderByDescending(x => x.Score)
                                .ThenBy(x => x.Index) // preserve original order for ties
                                .Select(x => x.Item)  // extract the string
                                .ToList();
                        }

                        itemList = strList;
                        break;

                    case Dictionary<string, Action> dict:
                        // Rank the keys by match score
                        var ranked = dict
                            .Select((kvp, index) => new
                            {
                                Key = kvp.Key,
                                Value = kvp.Value,
                                Score = GetSearchItemMatchScore(kvp.Key, localSearchVal),
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

                        itemList = dict;
                        break;

                    default:
                        throw new NotSupportedException("Unsupported collection type for DrawList.");
                }
            }
            else
            {
                switch (itemList)
                {
                    case string[] strArray:
                        var rankedArray = strArray
                            .Select((i, index) => new { Item = i, Score = GetListItemMatchScore(i), Index = index })
                            .ToList();

                        if (rankedArray.Any(x => x.Score > 0))
                        {
                            strArray = rankedArray
                                .OrderByDescending(x => x.Score)
                                .ThenBy(x => x.Index) // preserve original order for ties
                                .Select(x => x.Item)  // extract the string
                                .ToArray();
                        }

                        itemList = strArray;
                        break;

                    case List<string> strList:
                        var rankedList = strList
                            .Select((i, index) => new { Item = i, Score = GetListItemMatchScore(i), Index = index })
                            .ToList();

                        if (rankedList.Any(x => x.Score > 0))
                        {
                            strList = rankedList
                                .OrderByDescending(x => x.Score)
                                .ThenBy(x => x.Index) // preserve original order for ties
                                .Select(x => x.Item)  // extract the string
                                .ToList();
                        }

                        itemList = strList;
                        break;

                    case Dictionary<string, Action> dict:
                        // Rank the keys by match score
                        var ranked = dict
                            .Select((kvp, index) => new
                            {
                                Key = kvp.Key,
                                Value = kvp.Value,
                                Score = GetListItemMatchScore(kvp.Key),
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

                        itemList = dict;
                        break;

                    default:
                        throw new NotSupportedException("Unsupported collection type for DrawList.");
                }
            }
        }
        
        #region XML doc
        /// <summary>
        /// Creates a simple list.
        /// </summary>
        /// <param name="label">The name of the list.</param>
        /// <param name="items">The list of items to be displayed.</param>
        /// <param name="foldoutOrScrollVar">Determines if the list can be folded, scrolled or noone. Null for noone; Boolean for folded; Vector2 for scrolled.</param>
        /// <param name="styles">The list of GUIStyles for the list.</param>
        /// <param name="options">The list's GUILayoutOptions list.</param>
        #endregion
        protected void DrawList<T>(string label, object items, ref T foldoutOrScrollVar, List_GUIStyles styles = null, List_GUILayoutOptions options = null)
        {
            styles ??= new List_GUIStyles();
            options ??= new List_GUILayoutOptions();


            if (foldoutOrScrollVar == null)
            {
                DrawList(label, items, styles, options);


                return;
            }


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
                    if (!string.IsNullOrEmpty(label))
                    {
                        DrawLabel(label, styles.HeaderStyle, options.HeaderOptions);
                    }


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
        #region XML doc
        /// <summary>
        /// Creates a foldable, scrollable list.
        /// </summary>
        /// <param name="label">The name of the list.</param>
        /// <param name="items">The list of items to be displayed.</param>
        /// <param name="foldoutBool">The boolean that controls the fold.</param>
        /// <param name="scrollVector">The vector2 that controls the scroller.</param>
        /// <param name="styles">The list of GUIStyles for the list.</param>
        /// <param name="options">The list's GUILayoutOptions list.</param>
        #endregion
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
        #region XML doc
        /// <summary>
        /// Internal simple list creation logic.
        /// </summary>
        /// <param name="label">The name of the list.</param>
        /// <param name="items">The list of items to be displayed.</param>
        /// <param name="styles">The list of GUIStyles for the list.</param>
        /// <param name="options">The list's GUILayoutOptions list.</param>
        #endregion
        private void DrawList(string label, object items, List_GUIStyles styles = null, List_GUILayoutOptions options = null)
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

        #region XML doc
        /// <summary>
        /// Internal searchable list sort logic.
        /// </summary>
        /// <param name="items">The list of items to sort.</param>
        /// <param name="searchStr">The search string.</param>
        /// <returns>Returns the sorted list.</returns>
        #endregion
        private T DrawSearchableList_Logic<T>(T items, string searchStr)
        {
            object itemsObj = (object)items;


            GetListItemMatchScore_Master(ref itemsObj, searchStr);

        
            return (T)itemsObj;
        }
        #region XML doc
        /// <summary>
        /// Creates a searchable list.
        /// </summary>
        /// <param name="label">The name of the list.</param>
        /// <param name="searchLabel">The name of the search field.</param>
        /// <param name="items">The list of items to be displayed.</param>
        /// <param name="searchStr">The search string.</param>
        /// <param name="delayedSearch">Whether the input field waits for the user to press enter/click away from the field to change the variable or not.</param>
        /// <param name="styles">The list of GUIStyles for the list.</param>
        /// <param name="options">The list's GUILayoutOptions list.</param>
        #endregion
        protected void DrawSearchableList<T>(string label, string searchLabel, ref T items, ref string searchStr, bool delayedSearch = false, List_GUIStyles styles = null, List_GUILayoutOptions options = null)
        {
            styles ??= new List_GUIStyles();
            options ??= new List_GUILayoutOptions();

            
            DrawInputField(searchLabel, ref searchStr, styles.SearchStyle, delayedSearch, false, null, null, false, true, options.SearchOptions);


            items = DrawSearchableList_Logic(items, searchStr);


            DrawList(label, items, styles, options);
        }
        #region XML doc
        /// <summary>
        /// Creates a foldable searchable list.
        /// </summary>
        /// <param name="label">The name of the list.</param>
        /// <param name="searchLabel">The name of the search field.</param>
        /// <param name="items">The list of items to be displayed.</param>
        /// <param name="searchStr">The search string.</param>
        /// <param name="foldoutBool">The boolean that controls the fold.</param>
        /// <param name="delayedSearch">Whether the input field waits for the user to press enter/click away from the field to change the variable or not.</param>
        /// <param name="styles">The list of GUIStyles for the list.</param>
        /// <param name="options">The list's GUILayoutOptions list.</param>
        #endregion
        protected void DrawSearchableList<T>(string label, string searchLabel, ref T items, ref string searchStr, ref bool foldoutBool, bool delayedSearch = false, List_GUIStyles styles = null, List_GUILayoutOptions options = null)
        {
            styles ??= new List_GUIStyles();
            options ??= new List_GUILayoutOptions();

            
            DrawInputField(searchLabel, ref searchStr, styles.SearchStyle, delayedSearch, false, null, null, false, true, options.SearchOptions);


            items = DrawSearchableList_Logic(items, searchStr);


            DrawList(label, items, ref foldoutBool, styles, options);
        }
        #region XML doc
        /// <summary>
        /// Creates a scrollable searchable list.
        /// </summary>
        /// <param name="label">The name of the list.</param>
        /// <param name="searchLabel">The name of the search field.</param>
        /// <param name="items">The list of items to be displayed.</param>
        /// <param name="searchStr">The search string.</param>
        /// <param name="scrollVector">The vector2 that controls the scroller.</param>
        /// <param name="delayedSearch">Whether the input field waits for the user to press enter/click away from the field to change the variable or not.</param>
        /// <param name="styles">The list of GUIStyles for the list.</param>
        /// <param name="options">The list's GUILayoutOptions list.</param>
        #endregion
        protected void DrawSearchableList<T>(string label, string searchLabel, ref T items, ref string searchStr, ref Vector2 scrollVector, bool delayedSearch = false, List_GUIStyles styles = null, List_GUILayoutOptions options = null)
        {
            styles ??= new List_GUIStyles();
            options ??= new List_GUILayoutOptions();

            
            DrawInputField(searchLabel, ref searchStr, styles.SearchStyle, delayedSearch, false, null, null, false, true, options.SearchOptions);


            items = DrawSearchableList_Logic(items, searchStr);


            DrawList(label, items, ref scrollVector, styles, options);
        }
        #region XML doc
        /// <summary>
        /// Creates a foldable, scrollable searchable list.
        /// </summary>
        /// <param name="label">The name of the list.</param>
        /// <param name="searchLabel">The name of the search field.</param>
        /// <param name="items">The list of items to be displayed.</param>
        /// <param name="searchStr">The search string.</param>
        /// <param name="foldoutBool">The boolean that controls the fold.</param>
        /// <param name="scrollVector">The vector2 that controls the scroller.</param>
        /// <param name="delayedSearch">Whether the input field waits for the user to press enter/click away from the field to change the variable or not.</param>
        /// <param name="styles">The list of GUIStyles for the list.</param>
        /// <param name="options">The list's GUILayoutOptions list.</param>
        #endregion
        protected void DrawSearchableList<T>(string label, string searchLabel, ref T items, ref string searchStr, ref bool foldoutBool, ref Vector2 scrollVector, bool delayedSearch = false, List_GUIStyles styles = null, List_GUILayoutOptions options = null)
        {
            styles ??= new List_GUIStyles();
            options ??= new List_GUILayoutOptions();

            
            DrawInputField(searchLabel, ref searchStr, styles.SearchStyle, delayedSearch, false, null, null, false, true, options.SearchOptions);


            items = DrawSearchableList_Logic(items, searchStr);


            DrawList(label, items, ref foldoutBool, ref scrollVector, styles, options);
        }

        #region XML doc
        /// <summary>
        /// Creates a labeled slider.
        /// </summary>
        /// <param name="label">The name of the slider.</param>
        /// <param name="value">The variable to change with the slider.</param>
        /// <param name="min">The minimmum value of the slider.</param>
        /// <param name="max">The maximmum value of the slider.</param>
        /// <param name="resetValuePresent">Whether the slider has a reset value.</param>
        /// <param name="resetValue">The reset value of the slider.</param>
        /// <param name="resetButtonStyle">The GUIStyle of the slider.</param>
        /// <param name="options">The slider's GUILayoutOptions.</param>
        #endregion
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

    #region XML doc
    /// <summary>
    /// Instantiates a popup window.
    /// </summary>
    /// <param name="message">The message to display in this popup.</param>
    /// <param name="width">The width of the popup.</param>
    /// <param name="height">The height of the popup.</param>
    /// <param name="options">The PopupOptions of this popup.</param>
    #endregion
    protected static void PopupWindow(string message, float width, float height, PopupOptions options)
    {
        if (options == null)
        {
            options = new PopupOptions();
        }

        CreateInstance<BasicPopup>().CreatePopup(message, width, height, options.Title, options.CloseButtonText, options.Sound, options.Silent, options.Image, options.ImageWidth, options.ImageHeight, options.Centered, options.Locked);
    }


    #region XML doc
    /// <summary>
    /// Retrieves the rect position of the main editor window.
    /// </summary>
    /// <returns>Returns the Rect of the window.</returns>
    #endregion
    private static Rect GetEditorMainWindowPos()
    {
        return EditorGUIUtility.GetMainWindowPosition();
    }

    #region XML doc
    /// <summary>
    /// Centers a window on screen.
    /// </summary>
    /// <param name="width">The current width of the window.</param>
    /// <param name="height">The current height of the window.</param>
    /// <returns>Returns the new Rect position of the window.</returns>
    #endregion
    private static Rect CenterWindow(float width, float height)
    {
        Rect main = GetEditorMainWindowPos();

        float x = main.x + (main.width - width) * 0.5f;
        float y = main.y + (main.height - height) * 0.5f;

        return new Rect(x, y, width, height);
    }
    #endregion




    #region Unity Methods
    protected virtual void OnEnable()
    {
        LoadPrefsOnEnable();
    }
    #region XML doc
    /// <summary>
    /// Loads all the tracked EditorPrefs on enable.
    /// </summary>
    #endregion
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
            MethodInfo method = typeof(EditorToolBase)
                .GetMethod(nameof(HandyEditorFunctions.GetEditorPref), BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public)
                .MakeGenericMethod(fieldType);

            object value = method.Invoke(null, new object[] { attr.Key, true, defaultValue });
            field.SetValue(this, value);
        }
    }

    protected virtual void OnDisable()
    {
        SavePrefsOnDisable();
    }
    #region XML doc
    /// <summary>
    /// Saves all the tracked EditorPrefs on enable.
    /// </summary>
    #endregion
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
            MethodInfo method = typeof(EditorToolBase)
                .GetMethod(nameof(HandyEditorFunctions.SetEditorPref), BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public)
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
    private class BasicPopup : EditorToolBase
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


        public EditorPrefAttribute(string key, object defaultValue = default)
        {
            Key = key;

            DefaultValue = defaultValue;
        }
    }
    #endregion
}

}
