using UnityEngine;

/// <summary>
/// 资源加载类
/// </summary>
class Resloader
{
    public static T Load<T>(string path) where T : UnityEngine.Object
    {
        return Resources.Load<T>(path);
    }

    //todo:以后在这里做热更新，动态加载
}