using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using CustomGUI;

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
        string[] items = new string[100];
        for(int i = 0; i < items.Length; ++i)
        {
            string temp = i + ": ";
            for(int j = 0; j < 100; ++j)
                items[i] = temp += "-" + j;
        }
        GUILayout.BeginVertical(); 
        TextItemScrollArea.Draw(new Rect(100, 0, 400, 300), items);
        GUILayout.EndVertical();
    }

}