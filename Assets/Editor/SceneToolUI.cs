using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class Init
{
    static Init()
    {
        SceneToolUI.ShowWindow();
    }
}

public class SceneToolUI : EditorWindow
{
    public static void ShowWindow()
    {
        var window = EditorWindow.GetWindowWithRect(typeof(SceneToolUI), 
            SceneToolUI.m_winPos, false);
        window.Show();
    }

    [MenuItem("SceneTool/UI")]
    static void ShowUI()
    {
        ShowWindow();
    }

    void OnGUI()
    {
        if(GUI.Button(new Rect(0, 0, this.position.width, 20), "清除Missing Script组件"))
        {
            
        }

    }

    void Update()
    {
        this.Repaint();
    }


    public static Rect m_winPos = new Rect(0, 0, 200, 150);
}