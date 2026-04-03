using UnityEditor;
using UnityEngine;

using SETB;
using static SETB.EditorGUI_Base;
using static SETB.HandyEditorFunctions;

public class EditorWindow_Test : EditorWindow_Base<EditorWindow_Test>
{
    int slider = 1;


    [EditorPref("CenteredBool", true)]
    private static bool centeredWindow = true;


    [EditorPref("FoldoutBool", true)]
    private bool foldout = true;




    [MenuItem("Tools/SETB/Test Tool")]
    public static void ShowWindow() => CreateWindow("Test Tool", centeredWindow);


    string listSearch = "";
    Vector2 scrollVector = new Vector2();
    bool listBool = false;
    string[] list = new string[] { "item1", "item2", "item3", "item4", "item5", "test", "placeholder", "example", "item2", "item3", "item4", "item5", "test", "placeholder", "example", "item2", "item3", "item4", "item5", "test", "placeholder", "example", "item2", "item3", "item4", "item5", "test", "placeholder", "example", "item2", "item3", "item4", "item5", "test", "placeholder", "example", "item2", "item3", "item4", "item5", "test", "placeholder", "example" };
    protected void OnGUI()
    {
        GUILayout.Label("This is a test tool", EditorStyles.boldLabel);

        DrawToggle("Centered Toggle", ref centeredWindow);

        DrawToggle("Foldout Toggle", ref foldout);

        DrawFoldout("Test Stuff", ref foldout, () =>
        {
            DrawLabeledSlider("TestSlider", ref slider, 1, 10);

            DrawButton("Show Popup... " + slider, () =>
            {
                for (int i = 0; i < slider; i++)
                {
                    PopupWindow("This is a popup message!", 250, 100, new PopupOptions { Title = "Test Popup", CloseButtonText = "Close" });
                }
            });
        });

        DrawSeparator(1f);



        //DrawList("Test", list, ref listBool);
        DrawSearchableList("TestList", "Search:", ref list, ref listSearch, ref listBool, ref scrollVector, false);
    }
}
