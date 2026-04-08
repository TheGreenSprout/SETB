using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

using static SETB.EditorGUI_Base;
using static SETB.HandyEditorFunctions;

namespace SETB
{
    public abstract class Editor_Base<T> : Editor where T : Editor_Base<T>
    {
        #region Variables
        private string cacheSaveStr = "";
        private Dictionary<string, float> cacheScoreDictionary = new();


        private Dictionary<string, SerializedProperty> propCache = new();
        #endregion




        #region Unity Methods
        protected virtual void OnEnable()
        {
            this.Load_AttributeEditorPrefs();

            propCache.Clear();
        }
        
        protected virtual void OnDisable() => this.Save_AttributeEditorPrefs();


        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawInspector();

            serializedObject.ApplyModifiedProperties();
        }

        protected virtual void DrawInspector() => DrawDefaultInspector();
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
            => _DrawSearchableList(ref cacheSaveStr, cacheScoreDictionary, label, searchLabel, ref items, ref searchStr, target, delayedSearch, styles, options);
        
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
            => _DrawSearchableList(ref cacheSaveStr, cacheScoreDictionary, label, searchLabel, ref items, ref searchStr, ref foldoutBool, target, delayedSearch, styles, options);
        
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
            => _DrawSearchableList(ref cacheSaveStr, cacheScoreDictionary, label, searchLabel, ref items, ref searchStr, ref scrollVector, target, delayedSearch, styles, options);
        
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
            => _DrawSearchableList(ref cacheSaveStr, cacheScoreDictionary, label, searchLabel, ref items, ref searchStr, ref foldoutBool, ref scrollVector, target, delayedSearch, styles, options);
        
        
        protected void Record(string name = "Inspector Change") => HandyEditorFunctions.Record(target, name);
        #endregion



        #region Misc
        protected SerializedProperty Prop(string name)
        {
            if (!propCache.TryGetValue(name, out var prop))
            {
                prop = serializedObject.FindProperty(name);
                propCache[name] = prop;
            }
            return prop;
        }


        protected void Validate(bool condition, string message, MessageType type = MessageType.Warning)
        {
            if (!condition) EditorGUILayout.HelpBox(message, type);
        }
        #endregion
    }
}
