using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Script_03_6
{
    [InitializeOnLoadMethod]
    static void InitializeOnLoadMethd()
    {
        EditorApplication.hierarchyWindowItemOnGUI = delegate(int instanceID, Rect selectionRect)
        {
            if(Selection.activeObject && instanceID == Selection.activeObject.GetInstanceID())
            {
                Rect buttonRect = selectionRect;
                buttonRect.width = 50;
                buttonRect.x += selectionRect.width - buttonRect.width;
                if(GUI.Button(buttonRect, "Down"))
                {
                    Debug.Log("Down Called");
                } 
            }
        };
    }


}
