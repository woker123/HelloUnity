using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEditor.SceneManagement;

public class DeleteAssetHandler : UnityEditor.AssetModificationProcessor
{
    static AssetDeleteResult OnWillDeleteAsset(string path, RemoveAssetOptions option)
    {
        var deleteResult = new AssetDeleteResult();
        if (path.EndsWith(".cs") || path.EndsWith(".CS"))
        {
            if (!HandleCsFileDeletion(path))
            {
                deleteResult = AssetDeleteResult.DidDelete;
            }
        }
        return deleteResult;
    }

    static bool HandleCsFileDeletion(string path)
    {
        if (EditorSceneManager.GetActiveScene().isDirty)
        {
            bool isSaveScene = EditorUtility.DisplayDialog("场景保存确认", "删除CS文件需要先保存场景，保存吗？", "保存", "不保存且不删除");
            if (isSaveScene)
            {
                EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
            }
            else
            {
                return false;
            }
        }

        var refGameObjs = GameObject.Find("ref:" + path);
        var checkResult = CheckCsRefLineInAllScene(path);
        if (checkResult != "")
        {
            return EditorUtility.DisplayDialog("删除脚本确认", checkResult, "删除", "保留");
        }
        return true;
    }

    static string CheckCsRefLineInAllScene(string csPath)
    {
        string guid = AssetDatabase.AssetPathToGUID(csPath);
        var scenePaths = GUIDFinder.GetAllScenePaths();
        string findResult = "";
        YAMLText yaml = null;
        foreach (var path in scenePaths)
        {
            yaml = new YAMLText(path);
            var guidLine = yaml.GetGUIDLine(guid);
            if (guidLine != -1)
            {
                findResult = "CS文件:guid=" + guid + "在" + path + "文件中第" + guidLine + "行有引用"
                    + "确认要删除吗";
                break;
            }
        }
        return findResult;
    }

    // [MenuItem("Tools/Test")]
    // static void Test()
    // {
    //     var path = @"D:\MyUnityProj\HelloUnity\Assets\Scenes\TestScene.unity";
    //     var yaml = new YAMLText(path);
    //     Debug.Log(yaml.GetGUIDLine("dc42784cf147c0c48a680349fa168899"));
    // }


}

public class GUIDFinder
{
    public static List<string> GetAllScenePaths()
    {
        List<string> scenePaths = new List<string>();
        var sceneGUIDs = AssetDatabase.FindAssets("t:scene");
        foreach (var sceneGUID in sceneGUIDs)
        {
            var path = AssetDatabase.GUIDToAssetPath(sceneGUID);
            if (path != "")
            {
                scenePaths.Add(ToAbsPath(path));
            }
        }
        return scenePaths;
    }

    public static string ToAbsPath(string path)
    {
        var folderPath = Application.dataPath;
        return folderPath.Substring(0, folderPath.LastIndexOf("/")) + "/" + path;
    }
}

public class YAMLText
{
    public YAMLText(string path)
    {
        StreamReader reader = new StreamReader(path);
        m_content = reader.ReadToEnd();
        reader.Close();
    }

    public int GetGUIDLine(string guid)
    {
        var fdIndex = m_content.IndexOf(guid);
        if (fdIndex != -1)
        {
            var rvFindIndex = fdIndex;
            int counter = 0;
            while ((rvFindIndex = m_content.LastIndexOfAny(new char[] { '\r', '\n' }, rvFindIndex - 1)) != -1)
            {
                ++counter;
            }
            return counter + 1;
        }
        else
        {
            return -1;
        }
    }

    public string m_content = "";
}
