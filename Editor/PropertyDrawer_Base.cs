using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static SETB.EditorGUI_Base;

namespace SETB
{
    public abstract class PropertyDrawer_Base<T> : PropertyDrawer where T : UnityEngine.Object
    {
        #region Variables
        private string cacheSaveStr = "";

        private Dictionary<string, float> cacheScoreDictionary = new Dictionary<string, float>();
        #endregion




        #region Unity Methods
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            DrawProperty(position, property, label);

            EditorGUI.EndProperty();
        }

        protected virtual void DrawProperty(Rect position, SerializedProperty property, GUIContent label) => EditorGUI.PropertyField(position, property, label, true);


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => EditorGUI.GetPropertyHeight(property, label, true);
        #endregion



        #region GUI Helpers
        protected Rect Line(ref Rect position, float? height = null)
        {
            if (height == null) height = EditorGUIUtility.singleLineHeight;

            
            Rect r = new Rect(position.x, position.y, position.width, height.Value);
            position.y += height.Value + EditorGUIUtility.standardVerticalSpacing;
            return r;
        }


        protected void DrawLabel(Rect rect, string text) => EditorGUI.LabelField(rect, text);


        protected void DrawButton(Rect rect, string label, Action action)
        {
            if (GUI.Button(rect, label)) action?.Invoke();
        }
        
        protected void DrawPropertyField(Rect rect, SerializedProperty prop, string label = null)
        {
            if (label == null) EditorGUI.PropertyField(rect, prop, GUIContent.none);
            else EditorGUI.PropertyField(rect, prop, new GUIContent(label));
        }


        protected void DrawFoldout(Rect rect, string title, ref bool state, Action logic, GUIStyle style = null, bool toggleOnClick = true)
        {
            if (state = EditorGUI.Foldout(rect, state, title, toggleOnClick, style)) logic?.Invoke();
        }
        #endregion
        


        #region Proxy
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
        public void DrawSearchableList<E>(string label, string searchLabel, ref E items, ref string searchStr, bool delayedSearch = false, List_GUIStyles styles = null, List_GUILayoutOptions options = null)
            => _DrawSearchableList(ref cacheSaveStr, cacheScoreDictionary, label, searchLabel, ref items, ref searchStr, delayedSearch, styles, options);
        
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
        public void DrawSearchableList<E>(string label, string searchLabel, ref E items, ref string searchStr, ref bool foldoutBool, bool delayedSearch = false, List_GUIStyles styles = null, List_GUILayoutOptions options = null)
            => _DrawSearchableList(ref cacheSaveStr, cacheScoreDictionary, label, searchLabel, ref items, ref searchStr, ref foldoutBool, delayedSearch, styles, options);
        
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
        public void DrawSearchableList<E>(string label, string searchLabel, ref E items, ref string searchStr, ref Vector2 scrollVector, bool delayedSearch = false, List_GUIStyles styles = null, List_GUILayoutOptions options = null)
            => _DrawSearchableList(ref cacheSaveStr, cacheScoreDictionary, label, searchLabel, ref items, ref searchStr, ref scrollVector, delayedSearch, styles, options);
        
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
        public void DrawSearchableList<E>(string label, string searchLabel, ref E items, ref string searchStr, ref bool foldoutBool, ref Vector2 scrollVector, bool delayedSearch = false, List_GUIStyles styles = null, List_GUILayoutOptions options = null)
            => _DrawSearchableList(ref cacheSaveStr, cacheScoreDictionary, label, searchLabel, ref items, ref searchStr, ref foldoutBool, ref scrollVector, delayedSearch, styles, options);
        #endregion
    }
}
