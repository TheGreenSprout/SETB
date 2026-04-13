using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

using static SETB.HandyEditorFunctions;

namespace SETB
{
    public static class EditorGUI_Base
    {
        #region Layout Helpers
        #region XML doc
        /// <summary>
        /// Sets an indent level.
        /// </summary>
        /// <param name="indent">The indent level to set.</param>
        #endregion
        public static void SetIndent(int indent) => EditorGUI.indentLevel = indent;
        public static void IterateIndent(int iteration) => EditorGUI.indentLevel += iteration;
        public static int GetIndent() => EditorGUI.indentLevel;
        public static void Indent(Action action)
        {
            IterateIndent(1);

            action?.Invoke();

            IterateIndent(-1);
        }

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
        public static void Horizontal(Action content, string style = null, params GUILayoutOption[] options)
        {
            BeginHorizontal(style, options);

            content?.Invoke();

            EndHorizontal();
        }
        public static void BeginHorizontal(string style = null, params GUILayoutOption[] options){
            if (style == null) EditorGUILayout.BeginHorizontal(options);
            else EditorGUILayout.BeginHorizontal(style, options);
        }
        public static void EndHorizontal() => EditorGUILayout.EndHorizontal();
        #region XML doc
        /// <summary>
        /// Creates a vertical area.
        /// </summary>
        /// <param name="content">The logic to run inside this area.</param>
        /// <param name="options">The vertical area's GUILayoutOptions.</param>
        #endregion
        public static void Vertical(Action content, string style = null, params GUILayoutOption[] options)
        {
            BeginVertical(style, options);

            content?.Invoke();

            EndVertical();
        }
        public static void BeginVertical(string style = null, params GUILayoutOption[] options){
            if (style == null) EditorGUILayout.BeginVertical(options);
            else EditorGUILayout.BeginVertical(style, options);
        }
        public static void EndVertical() => EditorGUILayout.EndVertical();


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
        public static void DrawLabel(string text, string text2, GUIStyle style = null, params GUILayoutOption[] options)
        {
            if (style == null) style = EditorStyles.label;

            EditorGUILayout.LabelField(text, text2, style, options);
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
        public static bool DrawToggle(string label, ref bool value, GUIStyle style = null, params GUILayoutOption[] options)
        {
            if (style == null) style = EditorStyles.toggle;


            value = EditorGUILayout.Toggle(label, value, style, options);


            return value;
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
        public static bool DrawToggleLeft(string label, ref bool value, GUIStyle style = null, params GUILayoutOption[] options)
        {
            if (style == null) value = EditorGUILayout.ToggleLeft(label, value, options);
            else value = EditorGUILayout.ToggleLeft(label, value, style, options);


            return value;
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
        public static T DrawSelectionPopup<T>(ref T value, GUIStyle style = null, UnityEngine.Object record = null, params GUILayoutOption[] options) where T : Enum
        {
            if (style == null) style = EditorStyles.popup;

            
            var newValue = (T)EditorGUILayout.EnumPopup(value, style, options);
            ChangeValue(ref value, newValue, record);


            return value;
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
        public static int DrawSelectionPopup(ref int value, string[] intOrGeneric_options = null, int[] int_optionalOptions = null, GUIStyle style = null, UnityEngine.Object record = null, params GUILayoutOption[] options)
        {
            if (style == null) style = EditorStyles.popup;


            int newValue;
            if (int_optionalOptions != null) newValue = EditorGUILayout.IntPopup(value, intOrGeneric_options, int_optionalOptions, style, options);
            else newValue = EditorGUILayout.Popup(value, intOrGeneric_options, style, options);

            ChangeValue(ref value, newValue, record);


            return value;
        }


        public static E DrawInputField<E>(string label, ref E field, GUIStyle style = null, UnityEngine.Object record = null, bool delayed = false, bool layerOrMask = false, string[] displayedMaskOptions = null, bool? passwordOrTag = null, bool allowSceneObjects = true, params GUILayoutOption[] options)
        {
            object newValue = field;


            if (field is string s) newValue = DrawInputString(label, s, style, delayed, passwordOrTag, options);

            else if (field is int i) newValue = DrawInputInt(label, i, style, delayed, layerOrMask, displayedMaskOptions, options);

            else if (field is long l) newValue = DrawInputLong(label, l, style, options);

            else if (field is float f) newValue = DrawInputFloat(label, f, style, delayed, options);

            else if (field is double d) newValue = DrawInputDouble(label, d, style, delayed, options);

            else if (field is Vector2 v2) newValue = DrawInputVector2(label, v2, options);

            else if (field is Vector2Int v2i) newValue = DrawInputVector2Int(label, v2i, options);

            else if (field is Vector3 v3) newValue = DrawInputVector3(label, v3, options);

            else if (field is Vector3Int v3i) newValue = DrawInputVector3Int(label, v3i, options);

            else if (field is Vector4 v4) newValue = DrawInputVector4(label, v4, options);

            else if (field is Color c) newValue = DrawInputColor(label, c, options);

            else if (field is Gradient g) newValue = DrawInputGradient(label, g, options);

            else if (field is Rect r) newValue = DrawInputRect(label, r, options);

            else if (field is RectInt rint) newValue = DrawInputRectInt(label, rint, options);

            else if (field is Bounds b) newValue = DrawInputBounds(label, b, options);
            else if (field is BoundsInt bint) newValue = DrawInputBoundsInt(label, bint, options);

            else if (typeof(E).IsEnum) newValue = DrawInputEnum(label, newValue, style, options);

            else if (field is SerializedProperty sp)
            {
                DrawInputSerializedProperty(label, sp, options);
                return (E)newValue;
            }

            else if (field is AnimationCurve curve) newValue = DrawInputCurve(label, curve, options);

            else if (field is uint ui) newValue = DrawInputUInt(label, ui, style, options);

            else if (field  is UnityEngine.Object obj) newValue = DrawInputObject(label, obj, allowSceneObjects, options);

            else throw new NotSupportedException($"Unsupported type: {typeof(E).Name}");


            ChangeValue(ref field, (E)newValue, record);


            return (E)newValue;
        }

        #region Individual Input Drawers
        public static void DrawInputString(string label, ref string value, GUIStyle style = null, bool delayed = false, bool? passwordOrTag = null, params GUILayoutOption[] options)
        {
            style ??= EditorStyles.textField;

            if (passwordOrTag != null) value = passwordOrTag.Value ? EditorGUILayout.PasswordField(label, value, style, options)
                                                                    : EditorGUILayout.TagField(label, value, style, options);
            else value = delayed ? EditorGUILayout.DelayedTextField(label, value, style, options) : EditorGUILayout.TextField(label, value, style, options);
        }
        public static string DrawInputString(string label, string value, GUIStyle style = null, bool delayed = false, bool? passwordOrTag = null, params GUILayoutOption[] options)
        {
            style ??= EditorStyles.textField;

            if (passwordOrTag != null) return passwordOrTag.Value ? EditorGUILayout.PasswordField(label, value, style, options)
                                                                    : EditorGUILayout.TagField(label, value, style, options);
            else return delayed ? EditorGUILayout.DelayedTextField(label, value, style, options) : EditorGUILayout.TextField(label, value, style, options);
        }

        public static void DrawInputInt(string label, ref int value, GUIStyle style = null, bool delayed = false, bool layerOrMask = false, string[] displayedMaskOptions = null, params GUILayoutOption[] options)
        {
            style ??= EditorStyles.numberField;

            if (layerOrMask) value = displayedMaskOptions == null ? EditorGUILayout.LayerField(label, value, style, options)
                                                                : EditorGUILayout.MaskField(new GUIContent(label), value, displayedMaskOptions, style, options);
            else value = delayed ? EditorGUILayout.DelayedIntField(label, value, style, options) : EditorGUILayout.IntField(label, value, style, options);
        }
        public static int DrawInputInt(string label, int value, GUIStyle style = null, bool delayed = false, bool layerOrMask = false, string[] displayedMaskOptions = null, params GUILayoutOption[] options)
        {
            style ??= EditorStyles.numberField;

            if (layerOrMask) return displayedMaskOptions == null ? EditorGUILayout.LayerField(label, value, style, options)
                                                                : EditorGUILayout.MaskField(new GUIContent(label), value, displayedMaskOptions, style, options);
            else return delayed ? EditorGUILayout.DelayedIntField(label, value, style, options) : EditorGUILayout.IntField(label, value, style, options);
        }

        public static void DrawInputLong(string label, ref long value, GUIStyle style = null, params GUILayoutOption[] options)
        {
            value = EditorGUILayout.LongField(label, value, style ?? EditorStyles.numberField, options);
        }
        public static long DrawInputLong(string label, long value, GUIStyle style = null, params GUILayoutOption[] options)
            => EditorGUILayout.LongField(label, value, style ?? EditorStyles.numberField, options);

        public static void DrawInputFloat(string label, ref float value, GUIStyle style = null, bool delayed = false, params GUILayoutOption[] options)
        {
            value = delayed ? EditorGUILayout.DelayedFloatField(label, value, style ?? EditorStyles.numberField, options)
                            : EditorGUILayout.FloatField(label, value, style ?? EditorStyles.numberField, options);
        }
        public static float DrawInputFloat(string label, float value, GUIStyle style = null, bool delayed = false, params GUILayoutOption[] options)
            => delayed ? EditorGUILayout.DelayedFloatField(label, value, style ?? EditorStyles.numberField, options)
                        : EditorGUILayout.FloatField(label, value, style ?? EditorStyles.numberField, options);

        public static void DrawInputDouble(string label, ref double value, GUIStyle style = null, bool delayed = false, params GUILayoutOption[] options)
        {
            value = delayed ? EditorGUILayout.DelayedDoubleField(label, value, style ?? EditorStyles.numberField, options)
                            : EditorGUILayout.DoubleField(label, value, style ?? EditorStyles.numberField, options);
        }
        public static double DrawInputDouble(string label, double value, GUIStyle style = null, bool delayed = false, params GUILayoutOption[] options)
            => delayed ? EditorGUILayout.DelayedDoubleField(label, value, style ?? EditorStyles.numberField, options)
                        : EditorGUILayout.DoubleField(label, value, style ?? EditorStyles.numberField, options);

        public static void DrawInputEnum<T>(string label, ref T value, GUIStyle style = null, params GUILayoutOption[] options) where T : Enum
        {
            value = (T)(object)EditorGUILayout.EnumFlagsField(label, (Enum)(object)value, style ?? EditorStyles.textField, options);
        }
        public static T DrawInputEnum<T>(string label, T value, GUIStyle style = null, params GUILayoutOption[] options) where T : Enum
            => (T)(object)EditorGUILayout.EnumFlagsField(label, (Enum)(object)value, style ?? EditorStyles.textField, options);
        public static void DrawInputEnum(string label, ref object value, GUIStyle style = null, params GUILayoutOption[] options)
        {
            value = EditorGUILayout.EnumFlagsField(label, (Enum)value, style ?? EditorStyles.textField, options);
        }
        public static object DrawInputEnum(string label, object value, GUIStyle style = null, params GUILayoutOption[] options)
            => EditorGUILayout.EnumFlagsField(label, (Enum)value, style ?? EditorStyles.textField, options);

        public static bool DrawInputSerializedProperty(string label, SerializedProperty property, params GUILayoutOption[] options)
            => EditorGUILayout.PropertyField(property, label == null ? null : new GUIContent(label), options);

        public static void DrawInputObject<T>(string label, ref T value, bool allowSceneObjects = true, params GUILayoutOption[] options) where T : UnityEngine.Object
        {
            value = (T)EditorGUILayout.ObjectField(label, value, typeof(T), allowSceneObjects, options);
        }
        public static T DrawInputObject<T>(string label, T value, bool allowSceneObjects = true, params GUILayoutOption[] options) where T : UnityEngine.Object
            => (T)EditorGUILayout.ObjectField(label, value, typeof(T), allowSceneObjects, options);

        public static void DrawInputVector2(string label, ref Vector2 value, params GUILayoutOption[] options)
        {
            value = EditorGUILayout.Vector2Field(label, value, options);
        }
        public static Vector2 DrawInputVector2(string label, Vector2 value, params GUILayoutOption[] options)
            => EditorGUILayout.Vector2Field(label, value, options);
        public static void DrawInputVector2Int(string label, ref Vector2Int value, params GUILayoutOption[] options)
        {
            value = EditorGUILayout.Vector2IntField(label, value, options);
        }
        public static Vector2Int DrawInputVector2Int(string label, Vector2Int value, params GUILayoutOption[] options)
            => EditorGUILayout.Vector2IntField(label, value, options);
        public static void DrawInputVector3(string label, ref Vector3 value, params GUILayoutOption[] options)
        {
            value = EditorGUILayout.Vector3Field(label, value, options);
        }
        public static Vector3 DrawInputVector3(string label, Vector3 value, params GUILayoutOption[] options)
            => EditorGUILayout.Vector3Field(label, value, options);
        public static void DrawInputVector3Int(string label, ref Vector3Int value, params GUILayoutOption[] options)
        {
            value = EditorGUILayout.Vector3IntField(label, value, options);
        }
        public static Vector3Int DrawInputVector3Int(string label, Vector3Int value, params GUILayoutOption[] options)
            => EditorGUILayout.Vector3IntField(label, value, options);
        public static void DrawInputVector4(string label, ref Vector4 value, params GUILayoutOption[] options)
        {
            value = EditorGUILayout.Vector4Field(label, value, options);
        }
        public static Vector4 DrawInputVector4(string label, Vector4 value, params GUILayoutOption[] options)
            => EditorGUILayout.Vector4Field(label, value, options);

        public static void DrawInputColor(string label, ref Color value, params GUILayoutOption[] options)
        {
            value = EditorGUILayout.ColorField(label, value, options);
        }
        public static Color DrawInputColor(string label, Color value, params GUILayoutOption[] options)
            => EditorGUILayout.ColorField(label, value, options);
        public static void DrawInputGradient(string label, ref Gradient value, params GUILayoutOption[] options)
        {
            value = EditorGUILayout.GradientField(label, value, options);
        }
        public static Gradient DrawInputGradient(string label, Gradient value, params GUILayoutOption[] options)
            => EditorGUILayout.GradientField(label, value, options);

        public static void DrawInputRect(string label, ref Rect value, params GUILayoutOption[] options)
        {
            value = EditorGUILayout.RectField(label, value, options);
        }
        public static Rect DrawInputRect(string label, Rect value, params GUILayoutOption[] options)
            => EditorGUILayout.RectField(label, value, options);
        public static void DrawInputRectInt(string label, ref RectInt value, params GUILayoutOption[] options)
        {
            value = EditorGUILayout.RectIntField(label, value, options);
        }
        public static RectInt DrawInputRectInt(string label, RectInt value, params GUILayoutOption[] options)
            => EditorGUILayout.RectIntField(label, value, options);
        public static void DrawInputBounds(string label, ref Bounds value, params GUILayoutOption[] options)
        {
            value = EditorGUILayout.BoundsField(label, value, options);
        }
        public static Bounds DrawInputBounds(string label, Bounds value, params GUILayoutOption[] options)
            => EditorGUILayout.BoundsField(label, value, options);
        public static void DrawInputBoundsInt(string label, ref BoundsInt value, params GUILayoutOption[] options)
        {
            value = EditorGUILayout.BoundsIntField(label, value, options);
        }
        public static BoundsInt DrawInputBoundsInt(string label, BoundsInt value, params GUILayoutOption[] options)
            => EditorGUILayout.BoundsIntField(label, value, options);

        public static void DrawInputCurve(string label, ref AnimationCurve value, params GUILayoutOption[] options)
        {
            value = EditorGUILayout.CurveField(label, value, options);
        }
        public static AnimationCurve DrawInputCurve(string label, AnimationCurve value, params GUILayoutOption[] options)
            => EditorGUILayout.CurveField(label, value, options);

        public static void DrawInputUInt(string label, ref uint value, GUIStyle style = null, params GUILayoutOption[] options)
        {
            value = EditorGUILayout.RenderingLayerMaskField(label, value, style ?? EditorStyles.numberField, options);
        }
        public static uint DrawInputUInt(string label, uint value, GUIStyle style = null, params GUILayoutOption[] options)
            => EditorGUILayout.RenderingLayerMaskField(label, value, style ?? EditorStyles.numberField, options);
        #endregion
        #endregion



        #region With Logic
        public static bool ChangeCheck(Action draw)
        {
            EditorGUI.BeginChangeCheck();

            draw?.Invoke();

            return EditorGUI.EndChangeCheck();
        }


        #region XML doc
        /// <summary>
        /// Creates a button.
        /// </summary>
        /// <param name="buttonName">The name of the button.</param>
        /// <param name="logic">The logic to be run by the button.</param>
        /// <param name="style">The GUIStyle of the button.</param>
        /// <param name="options">The button's GUILayoutOptions.</param>
        #endregion
        public static bool DrawButton(string buttonName, Action logic = null, GUIStyle style = null, params GUILayoutOption[] options)
        {
            bool button = style == null ? GUILayout.Button(buttonName, options) : GUILayout.Button(buttonName, style, options);
            
            if (button) logic?.Invoke();

            return button;
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
                cacheScoreDictionary.Clear();

                
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
            object itemsObj = items;


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
        public static void _DrawSearchableList<E>(ref string cacheSaveStr, Dictionary<string, float> cacheScoreDictionary, string label, string searchLabel, ref E items, ref string searchStr, UnityEngine.Object record = null, bool delayedSearch = false, List_GUIStyles styles = null, List_GUILayoutOptions options = null)
        {
            styles ??= new List_GUIStyles();
            options ??= new List_GUILayoutOptions();

            
            DrawInputString(searchLabel, ref searchStr, styles.SearchStyle, delayedSearch, false, options.SearchOptions);


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
        public static void _DrawSearchableList<E>(ref string cacheSaveStr, Dictionary<string, float> cacheScoreDictionary, string label, string searchLabel, ref E items, ref string searchStr, ref bool foldoutBool, UnityEngine.Object record = null, bool delayedSearch = false, List_GUIStyles styles = null, List_GUILayoutOptions options = null)
        {
            styles ??= new List_GUIStyles();
            options ??= new List_GUILayoutOptions();

            
            DrawInputString(searchLabel, ref searchStr, styles.SearchStyle, delayedSearch, false, options.SearchOptions);


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
        public static void _DrawSearchableList<E>(ref string cacheSaveStr, Dictionary<string, float> cacheScoreDictionary, string label, string searchLabel, ref E items, ref string searchStr, ref Vector2 scrollVector, UnityEngine.Object record = null, bool delayedSearch = false, List_GUIStyles styles = null, List_GUILayoutOptions options = null)
        {
            styles ??= new List_GUIStyles();
            options ??= new List_GUILayoutOptions();

            
            DrawInputString(searchLabel, ref searchStr, styles.SearchStyle, delayedSearch, false, options.SearchOptions);


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
        public static void _DrawSearchableList<E>(ref string cacheSaveStr, Dictionary<string, float> cacheScoreDictionary, string label, string searchLabel, ref E items, ref string searchStr, ref bool foldoutBool, ref Vector2 scrollVector, UnityEngine.Object record = null, bool delayedSearch = false, List_GUIStyles styles = null, List_GUILayoutOptions options = null)
        {
            styles ??= new List_GUIStyles();
            options ??= new List_GUILayoutOptions();

            
            DrawInputString(searchLabel, ref searchStr, styles.SearchStyle, delayedSearch, false, options.SearchOptions);


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
    
    

        #region Misc
        public static bool ChangeValue<T>(ref T value, T newValue, UnityEngine.Object target = null, string undoActionName = "Value Change", bool includeTypeName = true)
        {
            if (target == null)
            {
                value = newValue;

                return false;
            }
            

            var _value = value;

            if (ChangeCheck(() => _value = newValue))
            {
                Record(target, undoActionName + (includeTypeName ? $" ({typeof(T).Name})" : ""));

                value = _value;


                return true;
            }
            return false;
        }
        #endregion
    }



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
}
