using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class SceneFinder
{
    static List<string> m_totalScenePaths = new List<string>();
    static List<Tuple<string, List<string>>> m_taskResult = new List<Tuple<string, List<string>>>();
    static int m_totalSceneNum = 0;
    static int m_currentTaskIndex = 0;
    static bool m_isInTask = false;
    static string m_tempScenePath;

    public static string GetGameObjectInheritPath(GameObject gameObject)
    {
        var gameObj = gameObject;
        string path = "/" + gameObj.name;
        while (gameObj.transform.parent != null)
        {
            gameObj = gameObj.transform.parent.gameObject;
            path = path.Insert(0, "/" + gameObj.name);
        }

        return path;
    }

    static void ResetTask()
    {
        var sceneGUIDs = AssetDatabase.FindAssets("t:scene");
        m_totalScenePaths.Clear();
        foreach (var sceneGUID in sceneGUIDs)
        {
            m_totalScenePaths.Add(AssetDatabase.GUIDToAssetPath(sceneGUID));
        }

        m_taskResult.Clear();
        m_totalSceneNum = m_totalScenePaths.Count;
        m_currentTaskIndex = 0;
        m_isInTask = true;
        m_tempScenePath = EditorSceneManager.GetActiveScene().path;
    }

    public static int GetCurrentTaskIndex()
    {
        return m_currentTaskIndex;
    }

    public static int GetTotalTaskNum()
    {
        return m_totalSceneNum;
    }

    public static List<Tuple<string, List<string>>> GetTaskResult()
    {
        return m_taskResult;
    }

    public static void UpdateTask()
    {
        if (!m_isInTask)
            return;

        var curScenePath = m_totalScenePaths[m_currentTaskIndex];
        var curScene = EditorSceneManager.OpenScene(curScenePath, OpenSceneMode.Single);
        var rootGameObjs = curScene.GetRootGameObjects();
        List<string> findResultPaths = new List<string>();
        foreach (var rootGameObj in rootGameObjs)
        {
            var comps = rootGameObj.GetComponentsInChildren<Transform>(true);
            foreach (var comp in comps)
            {
                if (GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(comp.gameObject) > 0)
                {
                    findResultPaths.Add(GetGameObjectInheritPath(comp.gameObject));
                }
            }
        }

        m_taskResult.Add(new Tuple<string, List<string>>(curScenePath, findResultPaths));
        ++m_currentTaskIndex;
        if (m_currentTaskIndex >= m_totalSceneNum)
        {
            m_isInTask = false;
            EditorSceneManager.OpenScene(m_tempScenePath, OpenSceneMode.Single);
        }

    }

    public static void FindMissingScriptInAllScene()
    {
        ResetTask();
    }

}

public class ClearMissingScriptUI : EditorWindow
{
    public static void ShowUI()
    {
        var window = (ClearMissingScriptUI)EditorWindow.GetWindow(typeof(ClearMissingScriptUI), false, "SceneTool");
        window.minSize = new Vector2(500, 600);
        //window.maxSize = new Vector2(800, 500);
        window.Show();
    }

    [MenuItem("Tools/清除Missing script组件")]
    static void ShowTool()
    {
        ShowUI();
    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(0, 0 * 20, this.position.width, 20), string.Format("扫描所有场景中的Missing script组件(注意保存当前场景)--{0}/{1}",
                SceneFinder.GetCurrentTaskIndex(), SceneFinder.GetTotalTaskNum())))
        {
            SceneFinder.FindMissingScriptInAllScene();
        }
        SceneFinder.UpdateTask();
        FindInSceneScrollBar(new Rect(0, 1 * 20, this.position.width, this.position.height / 2 - 1 * 20), SceneFinder.GetTaskResult());


        /**********************************************************************************************/
        int denum = currentIndexFlag < pathNumNeedCheck ? currentIndexFlag + 1 : pathNumNeedCheck;
        if (GUI.Button(new Rect(0, this.position.height / 2, this.position.width, 20),
                "扫描所有具有Missing Script的Prefab--" + string.Format("{0}/{1}", denum, pathNumNeedCheck)))
        {
            FindPrefabMissingScript();
        }
        UpdateTaskMissingScriptFinding();
        PinToProjectViewScrollBar(new Rect(0, this.position.height / 2 + 20, this.position.width, this.position.height / 2 - 1 * 20), targetPaths.ToArray());

        this.Repaint();
    }

    int clearNumInScene = 0;
    void ClearMissingScriptInSceneViewUI()
    {
        GUI.Label(new Rect(0, 0 * 20, this.position.width, 20), string.Format("上次清除组件数目:{0}", clearNumInScene));
        if (GUI.Button(new Rect(0, 1 * 20, this.position.width, 20), "清除场景中的Missing script组件"))
        {
            int clearNum = ClearMissingScriptInCurrentScene();
            clearNumInScene = clearNum > 0 ? clearNum : clearNumInScene;
        }
    }

    Vector2 scrollPos2 = new Vector2(0, 0);
    void FindInSceneScrollBar(Rect outterPos, List<Tuple<string, List<string>>> items)
    {
        int perItemHeight = 20;
        float textMaxWidth = 0;
        int buttonWidth = 180;

        if (items == null) items = new List<Tuple<string, List<string>>>();
        foreach (var item in items)
        {
            foreach (var obj in item.Item2)
            {
                var w = GUI.skin.textArea.CalcSize(new GUIContent(item.Item1 + obj)).x;
                if (w > textMaxWidth)
                    textMaxWidth = w;
            }
        }

        var svWidth = textMaxWidth + buttonWidth;
        int totalCount = 0;
        foreach (var item in items) totalCount += item.Item2.Count;
        var svInnerPos = new Rect(outterPos.x, outterPos.y, svWidth, perItemHeight * totalCount);
        scrollPos2 = GUI.BeginScrollView(outterPos, scrollPos2, svInnerPos, true, true);
        int counter = 0;
        for (int i = 0; i < items.Count; ++i)
        {
            for (int j = 0; j < items[i].Item2.Count; ++j)
            {
                if (GUI.Button(new Rect(svInnerPos.x, svInnerPos.y + counter * perItemHeight, buttonWidth, perItemHeight), "打开Scene并定位GameOjbect"))
                {
                    if (!EditorSceneManager.GetActiveScene().path.Equals(items[i].Item1))
                        EditorSceneManager.OpenScene(items[i].Item1, OpenSceneMode.Single);
                    var targetGameObj = GameObject.Find(items[i].Item2[j]);
                    EditorGUIUtility.PingObject(targetGameObj);
                    Selection.activeObject = targetGameObj;
                }
                GUI.TextField(new Rect(svInnerPos.x + buttonWidth, svInnerPos.y + counter * perItemHeight, textMaxWidth, perItemHeight),
                    items[i].Item1 + items[i].Item2[j]);
                ++counter;
            }

        }

        GUI.EndScrollView();
    }

    Vector2 scrollPos = new Vector2(0, 0);
    void PinToProjectViewScrollBar(Rect outterPos, string[] paths)
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
                Selection.activeObject = gameObj;
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

    /***
    * 为防止Editor卡死，模拟Coroutine
    * 每帧只完成一部分任务
    ***/
    string[] pathsNeedCheck = new string[0];
    List<string> targetPaths = new List<string>();
    int pathNumNeedCheck = 0;
    int currentIndexFlag = 0;
    bool isFirstIntoOnThisTask = true;
    bool isThisTaskRunning = false;
    void FindPrefabMissingScript()
    {
        pathsNeedCheck = new string[0];
        targetPaths = new List<string>();
        pathNumNeedCheck = 0;
        currentIndexFlag = 0;
        var prefabGUIDs = AssetDatabase.FindAssets("t:prefab");
        pathsNeedCheck = new string[prefabGUIDs.Length];
        for (int i = 0; i < pathsNeedCheck.Length; ++i)
            pathsNeedCheck[i] = AssetDatabase.GUIDToAssetPath(prefabGUIDs[i]);
        pathNumNeedCheck = pathsNeedCheck.Length;
        isFirstIntoOnThisTask = true;
        isThisTaskRunning = true;
    }

    void UpdateTaskMissingScriptFinding()
    {
        if (!isThisTaskRunning)
            return;

        //初次进入
        if (isFirstIntoOnThisTask)
        {
            isFirstIntoOnThisTask = false;
        }

        int handleNum = 1;
        //每帧只检查handleNum个，防止Editor卡死
        for (int i = 0; i < handleNum; ++i)
        {
            int index = i + currentIndexFlag;
            if (index < pathNumNeedCheck)
            {
                var prefabObj = AssetDatabase.LoadAssetAtPath<GameObject>(pathsNeedCheck[index]);
                var comps = prefabObj.transform.GetComponentsInChildren<Transform>();
                foreach (var comp in comps)
                {
                    //阻止嵌套检查
                    if (!PrefabUtility.IsPartOfPrefabInstance(comp.gameObject))
                    {
                        int num = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(comp.gameObject);
                        if (num > 0)
                        {
                            targetPaths.Add(pathsNeedCheck[index]);
                            break;
                        }
                    }
                }
            }
        }
        currentIndexFlag += handleNum;

        //任务是否完成？
        if (currentIndexFlag >= pathNumNeedCheck)
        {
            isThisTaskRunning = false;
            isFirstIntoOnThisTask = true;
        }
    }
}
