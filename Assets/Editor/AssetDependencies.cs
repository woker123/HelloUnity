using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Test11
{
    [MenuItem("Tools/Test")]
    static void Test()
    {
        var dependencies = AssetDatabase.GetDependencies("Assets/Scenes/TestScene.unity");
        foreach(var depend in dependencies)
        {
            Debug.Log(depend);
        }


    }

}