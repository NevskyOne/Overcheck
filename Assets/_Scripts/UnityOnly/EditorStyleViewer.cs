using UnityEditor;
using UnityEngine;

public class EditorStyleViewer : EditorWindow
{
    private Vector2 scrollPosition;

    [MenuItem("Window/Editor Style Viewer")]
    public static void ShowWindow()
    {
        GetWindow<EditorStyleViewer>("Editor Style Viewer");
    }

    private void OnGUI()
    {
        scrollPosition = GUILayout.BeginScrollView(scrollPosition);
        foreach (GUIStyle style in GUI.skin.customStyles)
        {
            GUILayout.Label(style.name, style);
        }
        GUILayout.EndScrollView();
    }
}