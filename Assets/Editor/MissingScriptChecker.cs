using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEditor;

public class Test
{
    [MenuItem("Tools/MissingScript测试")]
    static void CheckAllScene()
    {
        List<string> errorList = new List<string>();
        int fixedCount = 0;
        var scene = EditorSceneManager.GetActiveScene();
        MissingScriptChecker.CheckMissingScript(scene, true, ref errorList, ref fixedCount);

        foreach (var estr in errorList)
            Debug.Log(estr);
    }
}


public class MissingScriptChecker
{
    public static void CheckMissingScript(Scene scene, bool autoFixed, ref List<string> erroList, ref int fixedCount)
    {
        List<GameObject> allGameOjbs = GetAllGameObjectInScene(scene);
        int count = 0;
        foreach (var gameObj in allGameOjbs)
        {
            if (GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(gameObj) > 0)
            {
                if (autoFixed)
                {
                    count += GameObjectUtility.RemoveMonoBehavioursWithMissingScript(gameObj);
                    erroList.Add(FormatLog(scene, gameObj, true));
                }
                else
                {
                    erroList.Add(FormatLog(scene, gameObj, false));
                }
            }
        }
        fixedCount = count;
    }

    static List<GameObject> GetAllGameObjectInScene(Scene scene)
    {
        List<GameObject> result = new List<GameObject>();
        var rootGameOjbs = scene.GetRootGameObjects();
        foreach (var rootGameObj in rootGameOjbs)
        {
            var comps = rootGameObj.GetComponentsInChildren<Transform>(true);
            foreach (var comp in comps)
            {
                if (comp.gameObject != null)
                {
                    result.Add(comp.gameObject);
                }
            }
        }

        return result;
    }

    static string GameObjectToInheritPath(GameObject gameObject)
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

    static string FormatLog(Scene scene, GameObject gameObj, bool isFixed)
    {
        string fixedStr = isFixed ? ",【已自动修复】" : "";
        string mess = gameObj.name + "上有带有MissingScript的MonoBehaviour组件" + fixedStr + ":"
            + scene.path + GameObjectToInheritPath(gameObj);

        return mess;
    }

}