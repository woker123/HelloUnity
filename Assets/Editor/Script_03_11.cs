using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;

[CustomEditor(typeof(Transform))]
public class Script_03_11 : Editor
{
    private Editor m_Editor;
    void OnEnable()
    {
        m_Editor = Editor.CreateEditor(target, Assembly.GetAssembly(typeof(Editor)).GetType("UnityEditor.TransformInspector", true));
    }

    public override void OnInspectorGUI()
    {
        m_Editor.OnInspectorGUI();
    }

}
