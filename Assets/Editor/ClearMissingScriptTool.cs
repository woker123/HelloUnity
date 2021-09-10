using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class SceneFinder
{
    static List<GameObject> m_totalTask = new List<GameObject>();
    static List<string> m_compleledTask = new List<string>();
    static List<GameObject> m_completedGameOjbs = new List<GameObject>();
    static int m_currentTaskIndex;
    static bool m_isInTask;
    static string m_scenePathInTask = "";

    public static void ClossingSceneCb(UnityEngine.SceneManagement.Scene scene, bool removingScene)
    {
        if (scene.path == m_scenePathInTask)
        {
            ClearTask();
        }
    }

    public static void BeginTask()
    {
        var curScene = EditorSceneManager.GetActiveScene();
        if (curScene == null) return;

        m_totalTask = new List<GameObject>();
        m_compleledTask = new List<string>();
        m_completedGameOjbs = new List<GameObject>();
        m_currentTaskIndex = 0;
        m_isInTask = true;
        m_scenePathInTask = curScene.path;
        var rootGameObjs = curScene.GetRootGameObjects();
        foreach (var rootGameObj in rootGameObjs)
        {
            var comps = rootGameObj.GetComponentsInChildren<Transform>(false);
            foreach (var comp in comps)
            {
                m_totalTask.Add(comp.gameObject);
            }
        }
    }

    public static void UpdateTask()
    {
        if (!m_isInTask)
            return;

        if (m_currentTaskIndex < m_totalTask.Count)
        {
            var gameObj = m_totalTask[m_currentTaskIndex];
            if (gameObj != null)
            {
                if (GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(gameObj) > 0)
                {
                    m_compleledTask.Add(GetGameObjInheritPath(gameObj));
                    m_completedGameOjbs.Add(gameObj);
                }     
                ++m_currentTaskIndex;
            }
        }
        else
        {
            EndTask();
        }
    }

    public static void ClearMissingScripts()
    {
        if(!m_isInTask)
        {
            foreach(var gameObj in m_completedGameOjbs)
            {
                if(gameObj != null)
                    GameObjectUtility.RemoveMonoBehavioursWithMissingScript(gameObj);
            }
            m_compleledTask.Clear();
            m_completedGameOjbs.Clear();
            //EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }
    }

    public static int GetTaskNum()
    {
        return m_totalTask.Count;
    }

    public static int GetCurTaskIndex()
    {
        return m_currentTaskIndex;
    }

    public static List<string> GetCompletedTask()
    {
        return m_compleledTask;
    }

    public static List<GameObject> GetCompletedGameObjs()
    {
        return m_completedGameOjbs;
    }

    public static string GetTaskScenePath()
    {
        return m_scenePathInTask;
    }

    public static bool IsInTask()
    {
        return m_isInTask;
    }

    static string GetGameObjInheritPath(GameObject gameObject)
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

    static void ClearTask()
    {
        m_totalTask = new List<GameObject>();
        m_compleledTask = new List<string>();
        m_completedGameOjbs = new List<GameObject>();
        m_currentTaskIndex = 0;
        m_isInTask = false;
        m_scenePathInTask = "";
    }

    static void EndTask()
    {
        m_isInTask = false;
    }
}

public class FindMissingScriptUI : EditorWindow
{
    public static void ShowUI()
    {
        var window = (FindMissingScriptUI)EditorWindow.GetWindow(typeof(FindMissingScriptUI), false, "MissingScriptTool");
        window.minSize = new Vector2(200, 200);
        window.position = new Rect(0, 0, 800, 600);
        window.Show();
        EditorSceneManager.sceneClosing  += SceneFinder.ClossingSceneCb;
    }

    [MenuItem("Tools/资源检查工具/查找Missing script组件")]
    static void ShowTool()
    {
        ShowUI();
    }

    void OnGUI()
    {
        if(GUI.Button(new Rect(0, 0 * 20, this.position.width - 100, 20), string.Format("查找当前场景中的Missing script组件(仅启用的游戏对象)--{0}/{1}",
                SceneFinder.GetCurTaskIndex(), SceneFinder.GetTaskNum())))
        {
            SceneFinder.BeginTask();
        }
        
        GUI.enabled = (!SceneFinder.IsInTask()) && (SceneFinder.GetCompletedGameObjs().Count > 0);
        if(GUI.Button(new Rect(this.position.width - 100, 0 * 20, 100, 20), "清除全部"))
        {
            SceneFinder.ClearMissingScripts();
        }
        GUI.enabled = true;

        SceneFinder.UpdateTask();
        FindInSceneScrollBar(new Rect(0, 1 * 20, this.position.width, this.position.height / 2 - 1 * 20), SceneFinder.GetCompletedTask());


        /**********************************************************************************************/
        int denum = currentIndexFlag < pathNumNeedCheck ? currentIndexFlag + 1 : pathNumNeedCheck;
        if (GUI.Button(new Rect(0, this.position.height / 2, this.position.width, 20),
                "在所有Prefab中查找具有Missing Script的组件--" + string.Format("{0}/{1}", denum, pathNumNeedCheck)))
        {
            FindPrefabMissingScript();
        }
        UpdateTaskMissingScriptFinding();
        FindInProjectViewScrollBar(new Rect(0, this.position.height / 2 + 20, this.position.width, this.position.height / 2 - 1 * 20), targetPaths.ToArray());

        this.Repaint();
    }

    Vector2 scrollPos2 = new Vector2(0, 0);
    void FindInSceneScrollBar(Rect outterPos, List<string> paths)
    {
        int perItemHeight = 20;
        float textMaxWidth = 2000;
        int buttonWidth = 120;

        var svWidth = textMaxWidth + buttonWidth;

        var svInnerPos = new Rect(outterPos.x, outterPos.y, svWidth, perItemHeight * paths.Count);
        scrollPos2 = GUI.BeginScrollView(outterPos, scrollPos2, svInnerPos, true, true);
        for (int i = 0; i < paths.Count; ++i)
        {
            if (GUI.Button(new Rect(svInnerPos.x, svInnerPos.y + i * perItemHeight, buttonWidth, perItemHeight), "定位GameOjbect"))
            {
                var targetGameObj = GameObject.Find(paths[i]);
                EditorGUIUtility.PingObject(targetGameObj);
                Selection.activeObject = targetGameObj;
                EditorApplication.ExecuteMenuItem("Edit/Lock View to Selected");
            }
            GUI.TextField(new Rect(svInnerPos.x + buttonWidth, svInnerPos.y + i * perItemHeight, textMaxWidth, perItemHeight),
                SceneFinder.GetTaskScenePath() + ": " + paths[i]);
        }

        GUI.EndScrollView();
    }

    Vector2 scrollPos = new Vector2(0, 0);
    void FindInProjectViewScrollBar(Rect outterPos, string[] paths)
    {
        int perItemHeight = 20;
        float textMaxWidth = 2000;
        int buttonWidth = 120;


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
