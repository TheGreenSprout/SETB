#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

using Alchemy.Editor;

using static SETB.EditorGUI_Base;
using static SETB.HandyEditorFunctions;

namespace SETB.SuperClasses
{
    #region XML doc
    /// <summary>
    /// Base class for custom editors that render through Alchemy's attribute-driven
    /// UI Toolkit inspector while still exposing the SETB helpers.
    /// </summary>
    #endregion
    public abstract class AlchemyEditor_Base<T> : AlchemyEditor where T : AlchemyEditor_Base<T>
    {
        #region Variables
        private string cacheSaveStr = "";
        private Dictionary<string, float> cacheScoreDictionary = new();
        #endregion




        #region Main
        // NOTE: We intentionally do NOT override OnEnable/OnDisable/OnDestroy.
        // AlchemyEditor implements them (privately) to invoke the [OnInspectorEnable],
        // [OnInspectorDisable] and [OnInspectorDestroy] callbacks. Declaring methods with
        // the same name here would shadow them and silently break those attributes.
        // Use those Alchemy attributes for lifecycle hooks; prefs saving and the scene GUI
        // are wired through the UI Toolkit panel lifecycle below.

        #region XML doc
        /// <summary>
        /// Whether Alchemy's attribute-driven inspector is drawn.
        /// Return false to take full control of the inspector through <see cref="DrawInspector"/>.
        /// </summary>
        #endregion
        protected virtual bool ShowAlchemyInspector => true;


        public override VisualElement CreateInspectorGUI()
        {
            this.Load_AttributeEditorPrefs();

            var root = new VisualElement();

            if (ShowAlchemyInspector) root.Add(base.CreateInspectorGUI());

            if (IsDrawInspectorOverridden())
            {
                root.Add(new IMGUIContainer(() =>
                {
                    if (serializedObject == null || serializedObject.targetObject == null) return;

                    serializedObject.Update();

                    DrawInspector();

                    serializedObject.ApplyModifiedProperties();
                }));
            }

            root.RegisterCallback<AttachToPanelEvent>(_ => SceneView.duringSceneGui += OnSceneGUIInternal);
            root.RegisterCallback<DetachFromPanelEvent>(_ =>
            {
                SceneView.duringSceneGui -= OnSceneGUIInternal;
                this.Save_AttributeEditorPrefs();
            });

            return root;
        }

        #region XML doc
        /// <summary>
        /// Override this to draw extra IMGUI content beneath Alchemy's inspector, or the
        /// whole inspector when <see cref="ShowAlchemyInspector"/> returns false.
        /// It runs inside an IMGUIContainer that is already wrapped with
        /// serializedObject.Update() and ApplyModifiedProperties(), so do not call those here.
        /// </summary>
        #endregion
        protected virtual void DrawInspector() { }

        private bool IsDrawInspectorOverridden()
        {
            var method = GetType().GetMethod(nameof(DrawInspector),
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public);

            return method != null && method.DeclaringType != typeof(AlchemyEditor_Base<T>);
        }


        private void OnSceneGUIInternal(SceneView sv) => OnSceneGUI();
        protected virtual void OnSceneGUI() { }
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


        protected SerializedProperty Prop(string name) => FindProperty(serializedObject, name);
        protected SerializedProperty Prop(SerializedObject obj, string name) => obj == null ? Prop(name) : FindProperty(obj, name);

        protected SerializedProperty PropRelative(SerializedProperty prop, string relativePath) => FindPropertyRelative(prop, relativePath);


        protected void RecordTarget(string name = "Inspector Change") => Record(target, name);
        #endregion
    }
}
#endif
