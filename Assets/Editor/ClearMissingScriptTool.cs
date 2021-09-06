using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public class ClearMissingScriptUI : EditorWindow
{
    public static void ShowUI()
    {
        var window = (ClearMissingScriptUI)EditorWindow.GetWindowWithRect(typeof(ClearMissingScriptUI), new Rect(0, 0, 200, 150), false, "SceneTool");
        window.Show();
    }

    [MenuItem("Tools/清除Missing script组件")]
    static void ShowTool()
    {
        ShowUI();
    }

    int clearNumInScene = 0;
    int clearNumInPrefab = 0;
    void OnGUI()
    {
        if(GUI.Button(new Rect(0, 0, this.position.width, 20), "清除场景Missing script组件"))
        {
            int clearNum =  ClearMissingScript();
            clearNumInScene = clearNum > 0 ?  clearNum  : clearNumInScene;    
        }
        GUI.Label(new Rect(0, 20, this.position.width, 20), string.Format("上次清除组件数目:{0}", clearNumInScene));

        if(GUI.Button(new Rect(0, 40, this.position.width, 20), "清除Prefab Missing script组件"))
        {
            int clearNum = ClearPrefabMissingScript();
            clearNumInPrefab = clearNum > 0 ? clearNum : clearNumInPrefab;
        }
        GUI.Label(new Rect(0, 60, this.position.width, 20), string.Format("上次清除组件数目:{0}", clearNumInPrefab));
    }

    //In current scene
    static int ClearMissingScript()
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
        foreach(var gameObj in totalObj)
        {
            int num = GameObjectUtility.RemoveMonoBehavioursWithMissingScript(gameObj);
            clearNum += num;
        }
        UnityEditor.SceneManagement.EditorSceneManager.SaveScene(curScene, scenePath);
        return clearNum;
    }

    //In all prefab
    static int ClearPrefabMissingScript()
    {
        int clearNum = 0;
        var prefabGUIDs = AssetDatabase.FindAssets("t:prefab");
        foreach(var prefabGUID in prefabGUIDs)
        {
            var prefabPath = AssetDatabase.GUIDToAssetPath(prefabGUID);
            var prefabObj = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            var gameObj = (GameObject)PrefabUtility.InstantiatePrefab(prefabObj);
            var comps = gameObj.transform.GetComponentsInChildren<Transform>(true);
            bool dirty = false;
            foreach(var comp in comps)
            {
                int cNum = GameObjectUtility.RemoveMonoBehavioursWithMissingScript(comp.gameObject);
                if(cNum > 0)
                {
                    clearNum += cNum;
                    dirty = true;
                }
            }
            if(dirty)
            {
                PrefabUtility.SaveAsPrefabAsset(gameObj, prefabPath);
            }
            
            GameObject.DestroyImmediate(gameObj);
        }

        return clearNum;
    }

}
