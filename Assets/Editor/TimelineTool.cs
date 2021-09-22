using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Timeline;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;

public class ToolUI : EditorWindow
{
    [MenuItem("Tools/TimelineTool")]
    private static void CreateWindow()
    {
        var window = EditorWindow.GetWindowWithRect<ToolUI>(new Rect(0, 0, 500, 300), true, "timelinetool");
        window.Show();
    }

    private void OnGUI()
    {
        TestButton();
    }

    private void TestButton()
    {
        GUILayout.BeginHorizontal();
        if(GUILayout.Button("TestTimeline"))
        {
            TimelineTool.Test();
        }
        GUILayout.EndHorizontal();
    }


}


public class TimelineTool
{
    public static void Test()
    {
        var rootObjs = EditorSceneManager.GetActiveScene().GetRootGameObjects();
        foreach(var rootObj in rootObjs)
        {
            var playableDirectors = rootObj.GetComponentsInChildren<PlayableDirector>();
            foreach (var playDirector in playableDirectors)
            {
                HandlePlayableComponent(playDirector);
            }
        }
    }

    private static void HandlePlayableComponent(PlayableDirector pDirector)
    {
        var playAsset = pDirector.playableAsset as TimelineAsset;
        var tracks = playAsset.GetRootTracks();
        foreach (var track in tracks)
        {
            var comp = pDirector.GetGenericBinding(track);
            if(comp && comp.GetType() == typeof(Animator))
                Debug.Log(comp.name);
            
        }

    }


}
