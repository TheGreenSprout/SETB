using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using static SETB.EditorGUI_Base;
using static SETB.HandyEditorFunctions;

namespace SETB
{

#region XML doc
/// <summary>
/// Class containing all the basics (and no so basics) for making editor tools.
/// </summary>
#endregion
public abstract class EditorWindow_Base<T> : EditorWindow where T : EditorWindow
{
    #region Variables
    private Rect? originalRect = null;


    private string cacheSaveStr = "";

    private Dictionary<string, float> cacheScoreDictionary = new Dictionary<string, float>();
    #endregion



    #region Unity Methods
    protected virtual void OnEnable() => this.Load_AttributeEditorPrefs();
    

    protected virtual void OnDisable() => this.Save_AttributeEditorPrefs();
    


    protected virtual void Update()
    {
        if (originalRect.HasValue) position = new Rect(originalRect.Value.x, originalRect.Value.y, position.width, position.height);
    }
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
    protected static void CreateWindow(string windowName, bool centered = false, bool locked = false, float minWidth = 0, float minHeight = 0, float maxWidth = 0, float maxHeight = 0)
    {
        var window = GetWindow<T>();
        window.titleContent = new GUIContent(windowName);

        if (minWidth > 0 && minHeight > 0) window.minSize = new Vector2(minWidth, minHeight);
        if (maxWidth > 0 && maxHeight > 0 && maxWidth >= minWidth && maxHeight >= minHeight) window.maxSize = new Vector2(maxWidth, maxHeight);

        window.Show();


        if (centered) window.position = CenterWindow(window.position.width, window.position.height);

        if (locked)
        {
            EditorApplication.delayCall += () =>
            {
                if (window != null && window is EditorWindow_Base<T> baseWindow) baseWindow.originalRect = window.position;
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
    protected static void CreateUtilityWindow(string windowName, bool centered = false, bool locked = false, float minWidth = 0, float minHeight = 0, float maxWidth = 0, float maxHeight = 0)
    {
        var window = GetWindow<T>();
        window.titleContent = new GUIContent(windowName);

        if (minWidth > 0 && minHeight > 0) window.minSize = new Vector2(minWidth, minHeight);
        if (maxWidth > 0 && maxHeight > 0 && maxWidth >= minWidth && maxHeight >= minHeight) window.maxSize = new Vector2(maxWidth, maxHeight);

        window.ShowUtility();


        if (centered) window.position = CenterWindow(window.position.width, window.position.height);

        if (locked)
        {
            EditorApplication.delayCall += () =>
            {
                if (window != null) ((EditorWindow_Base<T>)(object)window).originalRect = window.position;
            };
        }
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



    #region Misc
    #region XML doc
    /// <summary>
    /// Retrieves the rect position of the main editor window.
    /// </summary>
    /// <returns>Returns the Rect of the window.</returns>
    #endregion
    private static Rect GetEditorMainWindowPos() => EditorGUIUtility.GetMainWindowPosition();

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


    protected void Validate(bool condition, string message, MessageType type = MessageType.Warning)
    {
        if (!condition) EditorGUILayout.HelpBox(message, type);
    }
    #endregion
}

}
