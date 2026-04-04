using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SETB
{
    public static class EditorGUI_Base
    {
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

            public List_GUILayoutOptions(List_GUILayoutOptions other)
            {
                SearchOptions = other.SearchOptions;
                HeaderOptions = other.HeaderOptions;
                ScrollOptions = other.ScrollOptions;
                ItemOptions = other.ItemOptions;
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

            public List_GUIStyles(List_GUIStyles other)
            {
                SearchStyle = other.SearchStyle;
                HeaderStyle = other.HeaderStyle;
                ItemStyle = other.ItemStyle;
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
        public static void Indent(int indent) => EditorGUI.indentLevel = indent;

        #region XML doc
        /// <summary>
        /// Creates a space.
        /// </summary>
        /// <param name="size">The size of the space.</param>
        #endregion
        public static void Space(float size = 6f, bool expand = true) => EditorGUILayout.Space(size, expand);

        #region XML doc
        /// <summary>
        /// Creates a separator.
        /// </summary>
        /// <param name="thickness">The thickness of the separator.</param>
        #endregion
        public static void DrawSeparator(float thickness = 1f)
        {
            Rect rect = EditorGUILayout.GetControlRect(false, thickness);
            EditorGUI.DrawRect(rect, new Color(0.3f, 0.3f, 0.3f));
        }


        #region XML doc
        /// <summary>
        /// Instantiates a flexible space.
        /// </summary>
        #endregion
        public static void FlexibleSpace() => GUILayout.FlexibleSpace();

        #region XML doc
        /// <summary>
        /// Creates a horizontal area.
        /// </summary>
        /// <param name="content">The logic to run inside this area.</param>
        /// <param name="options">The horizontal area's GUILayoutOptions.</param>
        #endregion
        public static void Horizontal(Action content, params GUILayoutOption[] options)
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
        public static void Vertical(Action content, params GUILayoutOption[] options)
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
        public static Rect GetControlRect(params GUILayoutOption[] options) => EditorGUILayout.GetControlRect(options);


        #region XML doc
        /// <summary>
        /// Creates a scroll view.
        /// </summary>
        /// <param name="scrollPos">The Vector2 that controlls the scroller.</param>
        /// <param name="content">The logic to run inside this view.</param>
        /// <param name="options">The scroll view's GUILayoutOptions.</param>
        #endregion
        public static void ScrollView(ref Vector2 scrollPos, Action content, params GUILayoutOption[] options)
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
        public static void DrawHelpBox(string message, MessageType type = MessageType.Info) => EditorGUILayout.HelpBox(message, type);
        

        #region XML doc
        /// <summary>
        /// Creates a label.
        /// </summary>
        /// <param name="text">The text to display on this label.</param>
        /// <param name="style">The GUIStyle of the label.</param>
        /// <param name="options">The label's GUILayoutOptions.</param>
        #endregion
        public static void DrawLabel(string text, GUIStyle style = null, params GUILayoutOption[] options)
        {
            if (style == null) style = EditorStyles.label;

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
        public static void DrawSelectableLabel(string text, GUIStyle style = null, params GUILayoutOption[] options)
        {
            if (style == null) style = EditorStyles.label;

            EditorGUILayout.SelectableLabel(text, style, options);
        }


        #region XML doc
        /// <summary>
        /// Creates a text area.
        /// </summary>
        /// <param name="text">The text to display on this area.</param>
        #endregion
        public static void DrawTextArea(ref string text) => text = EditorGUILayout.TextArea(text);
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
        public static void DrawToggle(string label, ref bool value, GUIStyle style = null, params GUILayoutOption[] options)
        {
            if (style == null) style = EditorStyles.toggle;

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
        public static void DrawToggleLeft(string label, ref bool value, GUIStyle style = null, params GUILayoutOption[] options)
        {
            if (style == null) style = EditorStyles.toggle;

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
        public static void DrawSelectionPopup(ref Enum value, GUIStyle style = null, params GUILayoutOption[] options)
        {
            if (style == null) style = EditorStyles.popup;

            value = EditorGUILayout.EnumPopup(value, style, options);
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
        public static void DrawSelectionPopup(ref int value, string[] intOrGeneric_options = null, int[] int_optionalOptions = null, GUIStyle style = null, params GUILayoutOption[] options)
        {
            if (style == null) style = EditorStyles.popup;

            if (int_optionalOptions != null) value = EditorGUILayout.IntPopup(value, intOrGeneric_options, int_optionalOptions, style, options);
            else value = EditorGUILayout.Popup(value, intOrGeneric_options, style, options);
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
        public static void DrawInputField<E>(string label, ref E field, GUIStyle style = null, bool delayed = false, bool layerOrMask = false, string[] displayedMaskOptions = null, bool? passwordOrTag = null, bool labelField = false, bool allowSceneObjects = true, params GUILayoutOption[] options)
        {
            Type e = typeof(E);
            switch (e)
            {
                case Type _ when e == typeof(string):
                    if (style == null) style = EditorStyles.textField;
                    
                    if (passwordOrTag != null)
                    {
                        if (passwordOrTag == true) field = (E)(object)EditorGUILayout.PasswordField(label, (string)(object)field, style, options);
                        else field = (E)(object)EditorGUILayout.TagField(label, (string)(object)field, style, options);
                    }
                    else
                    {
                        if (labelField) EditorGUILayout.LabelField(label, (string)(object)field, style, options);
                        else
                        {
                            if (delayed) field = (E)(object)EditorGUILayout.DelayedTextField(label, (string)(object)field, style, options);
                            else field = (E)(object)EditorGUILayout.TextField(label, (string)(object)field, style, options);
                        }
                    }
                    break;

                case Type _ when e == typeof(int):
                    if (style == null) style = EditorStyles.numberField;
                    
                    if (layerOrMask)
                    {
                        if (displayedMaskOptions == null) field = (E)(object)EditorGUILayout.LayerField(label, (int)(object)field, style, options);
                        else field = (E)(object)EditorGUILayout.MaskField(new GUIContent(label), (int)(object)field, displayedMaskOptions, style, options);
                    }
                    else
                    {
                        if (delayed) field = (E)(object)EditorGUILayout.DelayedIntField(label, (int)(object)field, style, options);
                        else field = (E)(object)EditorGUILayout.IntField(label, (int)(object)field, style, options);
                    }
                    break;

                case Type _ when e == typeof(long):
                    if (style == null) style = EditorStyles.numberField;
                    
                    field = (E)(object)EditorGUILayout.LongField(label, (long)(object)field, style, options);
                    break;

                case Type _ when e == typeof(float):
                    if (style == null) style = EditorStyles.numberField;
                    
                    if (delayed) field = (E)(object)EditorGUILayout.DelayedFloatField(label, (float)(object)field, style, options);
                    else field = (E)(object)EditorGUILayout.FloatField(label, (float)(object)field, style, options);
                    break;

                case Type _ when e == typeof(double):
                    if (style == null) style = EditorStyles.numberField;
                    
                    if (delayed) field = (E)(object)EditorGUILayout.DelayedDoubleField(label, (double)(object)field, style, options);
                    else field = (E)(object)EditorGUILayout.DoubleField(label, (double)(object)field, style, options);
                    break;

                case Type _ when e == typeof(Vector2):
                    field = (E)(object)EditorGUILayout.Vector2Field(label, (Vector2)(object)field, options);
                    break;

                case Type _ when e == typeof(Vector2Int):
                    field = (E)(object)EditorGUILayout.Vector2IntField(label, (Vector2Int)(object)field, options);
                    break;

                case Type _ when e == typeof(Vector3):
                    field = (E)(object)EditorGUILayout.Vector3Field(label, (Vector3)(object)field, options);
                    break;

                case Type _ when e == typeof(Vector3Int):
                    field = (E)(object)EditorGUILayout.Vector3IntField(label, (Vector3Int)(object)field, options);
                    break;

                case Type _ when e == typeof(Vector4):
                    field = (E)(object)EditorGUILayout.Vector4Field(label, (Vector4)(object)field, options);
                    break;

                case Type _ when e == typeof(Color):
                    field = (E)(object)EditorGUILayout.ColorField(label, (Color)(object)field, options);
                    break;

                case Type _ when e == typeof(Gradient):
                    field = (E)(object)EditorGUILayout.GradientField(label, (Gradient)(object)field, options);
                    break;

                case Type _ when e == typeof(Rect):
                    field = (E)(object)EditorGUILayout.RectField(label, (Rect)(object)field, options);
                    break;

                case Type _ when e == typeof(RectInt):
                    field = (E)(object)EditorGUILayout.RectIntField((RectInt)(object)field, options);
                    break;

                case Type _ when e == typeof(Bounds):
                    field = (E)(object)EditorGUILayout.BoundsField(label, (Bounds)(object)field, options);
                    break;
                    
                case Type _ when e == typeof(BoundsInt):
                    field = (E)(object)EditorGUILayout.BoundsIntField(label, (BoundsInt)(object)field, options);
                    break;

                case Type _ when typeof(E).IsEnum:
                    if (style == null) style = EditorStyles.textField;
                    
                    field = (E)(object)EditorGUILayout.EnumFlagsField(label, (Enum)(object)field, style, options);
                    break;

                case Type _ when e == typeof(AnimationCurve):
                    field = (E)(object)EditorGUILayout.CurveField(label, (AnimationCurve)(object)field, options);
                    break;

                case Type _ when e == typeof(uint):
                    if (style == null) style = EditorStyles.numberField;
                    
                    field = (E)(object)EditorGUILayout.RenderingLayerMaskField(label, (uint)(object)field, style, options);
                    break;

                case Type _ when typeof(UnityEngine.Object).IsAssignableFrom(typeof(E)):
                    var obj = (UnityEngine.Object)(object)field;
                    var objType = obj != null ? obj.GetType() : typeof(E);

                    field = (E)(object)EditorGUILayout.ObjectField(label, obj, objType, allowSceneObjects, options);
                    break;

                case Type _ when e == typeof(SerializedProperty):
                    EditorGUILayout.PropertyField((SerializedProperty)(object)field, new GUIContent(label), options);
                    break;

                default:
                    throw new NotSupportedException($"Unsupported type: {e.Name} for DrawInputField<{typeof(E)}>");
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
        public static void DrawButton(string buttonName, Action logic, GUIStyle style = null, params GUILayoutOption[] options)
        {
            if (style == null)
            {
                if (GUILayout.Button(buttonName, options)) logic?.Invoke();
            }
            else if (GUILayout.Button(buttonName, style, options)) logic?.Invoke();
        }


        #region XML doc
        /// <summary>
        /// Creates a toggle group.
        /// </summary>
        /// <param name="title">The title of the group.</param>
        /// <param name="state">The boolean that controls the toggle group.</param>
        /// <param name="logic">The logic to be run inside the toggle group.</param>
        #endregion
        public static void DrawToggleGroup(string title, ref bool state, Action logic)
        {
            state = EditorGUILayout.BeginToggleGroup(title, state);

            if (state) logic?.Invoke();

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
        public static void DrawFoldout(string title, ref bool state, Action logic, GUIStyle style = null, bool toggleOnClick = true)
        {
            if (style == null) style = EditorStyles.foldout;


            if (state = EditorGUILayout.Foldout(state, title, toggleOnClick, style)) logic?.Invoke();
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
        public static void DrawFoldoutHeaderGroup(string title, ref bool state, Action logic, GUIStyle style = null, Action<Rect> menuAction = null, GUIStyle menuIcon = null)
        {
            if (style == null) style = EditorStyles.foldoutHeader;


            state = EditorGUILayout.BeginFoldoutHeaderGroup(state, title, style, menuAction, menuIcon);

            if (state) logic?.Invoke();

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
        private static float GetSearchItemMatchScore(Dictionary<string, float> cacheScoreDictionary, string item, string search)
        {
            if (string.IsNullOrEmpty(search)) return 0; // no search, keep natural order


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

            if (score > 0) CacheItemScore(cacheScoreDictionary, item, score);

            
            return score;
        }
        #region XML doc
        /// <summary>
        /// Caches an item's score to be used later.
        /// </summary>
        /// <param name="item">The item string.</param>
        /// <param name="score">The item's match score.</param>
        #endregion
        private static void CacheItemScore(Dictionary<string, float> cacheScoreDictionary, string item, float score)
        {
            if (cacheScoreDictionary.ContainsKey(item)) return;

            cacheScoreDictionary.Add(item, score);
        }
        #region XML doc
        /// <summary>
        /// Gets the match score of a list's item from it's cached match score.
        /// </summary>
        /// <param name="item">The item string.</param>
        /// <returns>Returns the match score.</returns>
        #endregion
        private static float GetListItemMatchScore(Dictionary<string, float> cacheScoreDictionary, string item)
        {
            if (cacheScoreDictionary.TryGetValue(item, out var score)) return score;

            return 0f;
        }
        #region XML doc
        /// <summary>
        /// Gets the match score of all the items on a list.
        /// </summary>
        /// <param name="itemList">The list of items.</param>
        /// <param name="search">The search string.</param>
        #endregion
        private static void GetListItemMatchScore_Master(ref object itemList, ref string cacheSaveStr, Dictionary<string, float> _cacheScoreDictionary, string search)
        {
            Dictionary<string, float> cacheScoreDictionary = _cacheScoreDictionary;


            if (search != cacheSaveStr)
            {
                cacheSaveStr = search;
                cacheScoreDictionary = new Dictionary<string, float>();

                
                string localSearchVal = search;


                switch (itemList)
                {
                    case string[] strArray:
                        var rankedArray = strArray
                            .Select((i, index) => new { Item = i, Score = GetSearchItemMatchScore(cacheScoreDictionary, i, localSearchVal), Index = index })
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
                            .Select((i, index) => new { Item = i, Score = GetSearchItemMatchScore(cacheScoreDictionary, i, localSearchVal), Index = index })
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
                                Score = GetSearchItemMatchScore(cacheScoreDictionary, kvp.Key, localSearchVal),
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
                            .Select((i, index) => new { Item = i, Score = GetListItemMatchScore(cacheScoreDictionary, i), Index = index })
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
                            .Select((i, index) => new { Item = i, Score = GetListItemMatchScore(cacheScoreDictionary, i), Index = index })
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
                                Score = GetListItemMatchScore(cacheScoreDictionary, kvp.Key),
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
        public static void DrawList<E>(string label, object items, ref E foldoutOrScrollVar, List_GUIStyles styles = null, List_GUILayoutOptions options = null)
        {
            styles ??= new List_GUIStyles();
            options ??= new List_GUILayoutOptions();


            if (foldoutOrScrollVar == null)
            {
                DrawList(label, items, styles, options);


                return;
            }


            Type e = typeof(E);
            switch (e)
            {
                case Type _ when e == typeof(bool):
                    bool foldoutBool = (bool)(object)foldoutOrScrollVar;

                    DrawFoldout(label, ref foldoutBool, () =>
                    {
                        DrawList(null, items, styles, options);
                    }, styles.HeaderStyle);

                    foldoutOrScrollVar = (E)(object)foldoutBool;
                    break;

                case Type _ when e == typeof(Vector2):
                    if (!string.IsNullOrEmpty(label))
                    {
                        DrawLabel(label, styles.HeaderStyle, options.HeaderOptions);
                    }


                    Vector2 scrollVector = (Vector2)(object)foldoutOrScrollVar;

                    ScrollView(ref scrollVector, () =>
                    {
                        DrawList(null, items, styles, options);
                    }, options.ScrollOptions);

                    foldoutOrScrollVar = (E)(object)scrollVector;
                    break;

                default:
                    throw new NotSupportedException($"Unsupported type: {e.Name} for DrawList<{typeof(E)}>");
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
        public static void DrawList(string label, object items, ref bool foldoutBool, ref Vector2 scrollVector, List_GUIStyles styles = null, List_GUILayoutOptions options = null)
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
        private static void DrawList(string label, object items, List_GUIStyles styles = null, List_GUILayoutOptions options = null)
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
        private static E DrawSearchableList_Logic<E>(ref string cacheSaveStr, Dictionary<string, float> cacheScoreDictionary, E items, string searchStr)
        {
            object itemsObj = (object)items;


            GetListItemMatchScore_Master(ref itemsObj, ref cacheSaveStr, cacheScoreDictionary, searchStr);

        
            return (E)itemsObj;
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
        public static void _DrawSearchableList<E>(ref string cacheSaveStr, Dictionary<string, float> cacheScoreDictionary, string label, string searchLabel, ref E items, ref string searchStr, bool delayedSearch = false, List_GUIStyles styles = null, List_GUILayoutOptions options = null)
        {
            styles ??= new List_GUIStyles();
            options ??= new List_GUILayoutOptions();

            
            DrawInputField(searchLabel, ref searchStr, styles.SearchStyle, delayedSearch, false, null, null, false, true, options.SearchOptions);


            items = DrawSearchableList_Logic(ref cacheSaveStr, cacheScoreDictionary, items, searchStr);


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
        public static void _DrawSearchableList<E>(ref string cacheSaveStr, Dictionary<string, float> cacheScoreDictionary, string label, string searchLabel, ref E items, ref string searchStr, ref bool foldoutBool, bool delayedSearch = false, List_GUIStyles styles = null, List_GUILayoutOptions options = null)
        {
            styles ??= new List_GUIStyles();
            options ??= new List_GUILayoutOptions();

            
            DrawInputField(searchLabel, ref searchStr, styles.SearchStyle, delayedSearch, false, null, null, false, true, options.SearchOptions);


            items = DrawSearchableList_Logic(ref cacheSaveStr, cacheScoreDictionary, items, searchStr);


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
        public static void _DrawSearchableList<E>(ref string cacheSaveStr, Dictionary<string, float> cacheScoreDictionary, string label, string searchLabel, ref E items, ref string searchStr, ref Vector2 scrollVector, bool delayedSearch = false, List_GUIStyles styles = null, List_GUILayoutOptions options = null)
        {
            styles ??= new List_GUIStyles();
            options ??= new List_GUILayoutOptions();

            
            DrawInputField(searchLabel, ref searchStr, styles.SearchStyle, delayedSearch, false, null, null, false, true, options.SearchOptions);


            items = DrawSearchableList_Logic(ref cacheSaveStr, cacheScoreDictionary, items, searchStr);


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
        public static void _DrawSearchableList<E>(ref string cacheSaveStr, Dictionary<string, float> cacheScoreDictionary, string label, string searchLabel, ref E items, ref string searchStr, ref bool foldoutBool, ref Vector2 scrollVector, bool delayedSearch = false, List_GUIStyles styles = null, List_GUILayoutOptions options = null)
        {
            styles ??= new List_GUIStyles();
            options ??= new List_GUILayoutOptions();

            
            DrawInputField(searchLabel, ref searchStr, styles.SearchStyle, delayedSearch, false, null, null, false, true, options.SearchOptions);


            items = DrawSearchableList_Logic(ref cacheSaveStr, cacheScoreDictionary, items, searchStr);


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
        public static void DrawLabeledSlider<E>(string label, ref E value, E min, E max, bool resetValuePresent = false, E resetValue = default, GUIStyle resetButtonStyle = null, params GUILayoutOption[] options)
        {
            EditorGUILayout.BeginHorizontal();

            Type e = typeof(E);
            if (e == typeof(short) || e == typeof(int) || e == typeof(long))
            {
                value = (E)(object)EditorGUILayout.IntSlider(new GUIContent(label), (int)(object)value, (int)(object)min, (int)(object)max, options);

                if (resetValuePresent)
                {
                    E val = value;

                    DrawButton("Reset", () =>
                    {
                        val = resetValue;
                    }, resetButtonStyle, options);

                    value = val;
                }
            }
            else if (e == typeof(float) || e == typeof(double))
            {
                value = (E)(object)EditorGUILayout.Slider(label, (float)(object)value, (float)(object)min, (float)(object)max, options);

                if (!Mathf.Approximately((float)(object)resetValue, default))
                {
                    E val = value;

                    DrawButton("Reset", () =>
                    {
                        val = resetValue;
                    }, resetButtonStyle, options);

                    value = val;
                }
            }
            else
            {
                throw new NotSupportedException($"Type {e} is not supported by DrawLabeledSlider.");
            }

            EditorGUILayout.EndHorizontal();
        }
        #endregion
    }
}
