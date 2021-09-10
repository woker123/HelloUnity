using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class UILayoutTest : EditorWindow 
{   
    [MenuItem("Tools/TestUI")]
    static void ShowUI()
    {
        var window = EditorWindow.GetWindowWithRect<UILayoutTest>(new Rect(0, 0, 400, 300), false);
        window.minSize = new Vector2(200, 200);
        window.maxSize = new Vector2(4000, 4000);
        window.Show();
    }

    private void OnGUI() 
    {
        GUILayout.Button("click me");
    }
}