using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class ClearMissingScriptUI : EditorWindow
{
    public static void ShowUI()
    {
        var window = (ClearMissingScriptUI)EditorWindow.GetWindow(typeof(ClearMissingScriptUI), false, "SceneTool");
        window.minSize = new Vector2(500, 300);
        //window.maxSize = new Vector2(800, 500);
        window.Show();
    }

    [MenuItem("Tools/清除Missing script组件")]
    static void ShowTool()
    {
        ShowUI();
    }

    string[] paths;
    void OnGUI()
    {
        ClearMissingScriptInSceneView();

        if (GUI.Button(new Rect(0, 2 * 20, this.position.width, 20), "扫描所有具有Missing Script的Prefab"))
        {
            paths = FindPrefabWithMissingScript();
        }
        ScrollView(new Rect(0, 3 * 20, this.position.width, this.position.height - 3 * 20), paths);
    }

    int clearNumInScene = 0;
    void ClearMissingScriptInSceneView()
    {
        GUI.Label(new Rect(0, 0 * 20, this.position.width, 20), string.Format("上次清除组件数目:{0}", clearNumInScene));
        if (GUI.Button(new Rect(0, 1 * 20, this.position.width, 20), "清除场景中的Missing script组件"))
        {
            int clearNum = ClearMissingScriptInCurrentScene();
            clearNumInScene = clearNum > 0 ? clearNum : clearNumInScene;
        }
    }

    Vector2 scrollPos = new Vector2(0, 0);
    void ScrollView(Rect outterPos, string[] paths)
    {
        int perItemHeight = 20;
        float textMaxWidth = 0;
        int buttonWidth = 120;

        if (paths == null) paths = new string[0];
        foreach (var str in paths)
        {
            var w = GUI.skin.textArea.CalcSize(new GUIContent(str)).x;
            if (w > textMaxWidth)
                textMaxWidth = w;
        }

        var svWidth = textMaxWidth + buttonWidth;
        var svInnerPos = new Rect(outterPos.x, outterPos.y, svWidth, perItemHeight * paths.Length);
        scrollPos = GUI.BeginScrollView(outterPos, scrollPos, svInnerPos, true, true);
        for (int i = 0; i < paths.Length; ++i)
        {
            if (GUI.Button(new Rect(svInnerPos.x, svInnerPos.y + i * perItemHeight, buttonWidth, perItemHeight), "定位到Project视图"))
            {
                var gameObj = AssetDatabase.LoadAssetAtPath<GameObject>(paths[i]);
                EditorGUIUtility.PingObject(gameObj);
            }
            GUI.TextField(new Rect(svInnerPos.x + buttonWidth, svInnerPos.y + i * perItemHeight, textMaxWidth, perItemHeight), paths[i]);
        }

        GUI.EndScrollView();
    }

    //only in current scene
    static int ClearMissingScriptInCurrentScene()
    {
        var curScene = EditorSceneManager.GetActiveScene();
        var scenePath = AssetDatabase.GetActiveScenePath();
        var rootGameObjs = curScene.GetRootGameObjects();
        List<GameObject> totalObj = new List<GameObject>();

        foreach (var gameObj in rootGameObjs)
        {
            var comps = gameObj.transform.GetComponentsInChildren<Transform>(true);
            foreach (var comp in comps)
            {
                totalObj.Add(comp.gameObject);
            }
        }

        int clearNum = 0;
        foreach (var gameObj in totalObj)
        {
            int num = GameObjectUtility.RemoveMonoBehavioursWithMissingScript(gameObj);
            clearNum += num;
        }
        UnityEditor.SceneManagement.EditorSceneManager.SaveScene(curScene, scenePath);
        return clearNum;
    }

    string[] FindPrefabWithMissingScript()
    {
        var prefabGUIDs = AssetDatabase.FindAssets("t:prefab");
        List<string> resultPaths = new List<string>();
        foreach (var prefabGUID in prefabGUIDs)
        {
            var prefabPath = AssetDatabase.GUIDToAssetPath(prefabGUID);
            var prefabObj = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            var comps = prefabObj.transform.GetComponentsInChildren<Transform>();
            foreach (var comp in comps)
            {
                //阻止嵌套检查
                if (!PrefabUtility.IsPartOfPrefabInstance(comp.gameObject))
                {
                    int num = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(comp.gameObject);
                    if (num > 0)
                    {
                        resultPaths.Add(prefabPath);
                        break;
                    }
                }
            }
        }
        return resultPaths.ToArray();
    }
}
