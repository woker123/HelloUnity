using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;


public class SceneTool
{
    [MenuItem("SceneTool/ClearMissingScript")]
    static void ClearMissingScript()
    {
        var gameObjs = SceneManager.GetActiveScene().GetRootGameObjects();

        //for per gameobject
        foreach (var gameObj in gameObjs)
        {
            var comps = gameObj.GetComponentsInChildren<Transform>();
            foreach (var comp in comps)
            {
                int rmNum = GameObjectUtility.RemoveMonoBehavioursWithMissingScript(comp.gameObject);
                if (rmNum > 0)
                {
                    Debug.Log("GameObject: " + comp.name + " has been removed " + rmNum + " missing script component");
                }
            }
        }
    }

    [MenuItem("SceneTool/Test")]
    static void Test()
    {
        var prefabGuids = AssetDatabase.FindAssets(string.Format("t:{0}", "Prefab"));

        AssetDatabase.Refresh();
        var PATH = AssetDatabase.GUIDToAssetPath("c65c9a10fbd35dd4796f0242cdc626a1");
        var obj = AssetDatabase.LoadAssetAtPath<GameObject>(PATH);
        

    }

}
