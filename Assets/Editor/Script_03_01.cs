﻿using UnityEngine;
using UnityEditor;

public class Script_03_01
{
    [MenuItem("Assets/My Tools/Tools 1", false, 2)]
    static void MyTools1()
    {
        Debug.Log("this is tools 1");
    }

    [MenuItem("Assets/My Tools/Tools 2", false, 1)]
    static void MyTools2()
    {
        Debug.Log("this is tools 2");
    }
    
}