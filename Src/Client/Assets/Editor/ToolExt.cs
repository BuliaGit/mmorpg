using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class ToolExt {

    [MenuItem("自定义工具/返回启动场景")]
    public static void GoToSetup()
    {
        EditorSceneManager.OpenScene(Application.dataPath + "/Levels/Loading.unity");
    }

    [MenuItem("自定义工具/返回主城场景")]
    public static void GoToMainCity()
    {
        EditorSceneManager.OpenScene(Application.dataPath + "/Levels/MainCity.unity");
    }
}
