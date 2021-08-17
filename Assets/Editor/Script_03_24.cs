using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Script_03_24 : EditorWindow
{
    [MenuItem("Window/Open My Window")]
    public static void Init()
    {
        Script_03_24 window = (Script_03_24)EditorWindow.GetWindow(typeof(Script_03_24));
        window.Show();
    }    

    void Awake()
    {
        Debug.Log("Window initialized");

    }
    
    public float m_MyFloat;

    void OnGUI()
    {
        GUILayout.Label("hello", EditorStyles.boldLabel);
        m_MyFloat = EditorGUILayout.Slider("Slider", m_MyFloat, -5, 5);

    }

}
