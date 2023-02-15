using SkillBridge.Message;
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UI管理器
/// </summary>
public class UIManager : Singleton<UIManager>
{
    class UIElement
    {
        public string Resources;
        public bool Cashe;
        public GameObject Instance;
    }

    private Dictionary<Type, UIElement> UIResources = new Dictionary<Type, UIElement>();

    public bool isOpen;

    public UIManager()
    {
        //Start()执行初始化，所以把缓存关掉，后续优化时再考虑打开
        this.UIResources.Add(typeof(UIBag), new UIElement() { Resources = "UI/UIBag", Cashe = false });
        this.UIResources.Add(typeof(UIShop), new UIElement() { Resources = "UI/UIShop", Cashe = true });   
        this.UIResources.Add(typeof(UIEquip), new UIElement() { Resources = "UI/UIEquip", Cashe = false });
        this.UIResources.Add(typeof(UIQuestSystem), new UIElement() { Resources = "UI/UIQuestSystem", Cashe = true });
        this.UIResources.Add(typeof(UIQuestDialog), new UIElement() { Resources = "UI/UIQuestDialog", Cashe = true });
        this.UIResources.Add(typeof(UIFriends), new UIElement() { Resources = "UI/UIFriends", Cashe = true });
        //this.UIResources.Add(typeof(UIRide), new UIElement() { Resources = "UI/UIRide", Cashe = true });
    }

    #region 打开
    public T Show<T>()
    {
        GameObjectManager.Instance.pc.rb.velocity = Vector3.zero;
        GameObjectManager.Instance.pc.character.Stop();
        GameObjectManager.Instance.pc.SendEntityEvent(EntityEvent.Idle);
        isOpen = true;

        //SoundManager.Instance.PlaySound("ui_open");
        Type type = typeof(T);
        if (this.UIResources.ContainsKey(type))
        {
            UIElement info = this.UIResources[type];
            if (info.Instance != null)
            {
                info.Instance.SetActive(true);
            }
            else
            {
                UnityEngine.Object prefab = Resources.Load(info.Resources);
                if (prefab == null)
                {
                    return default(T);
                }
                info.Instance = (GameObject)GameObject.Instantiate(prefab);
            }
            return info.Instance.GetComponent<T>();
        }
        return default(T);
    }
    #endregion

    #region 关闭
    public void Close(Type type)
    {
        isOpen = false;

        //SoundManager.Instance.PlaySound("ui_close");
        if (this.UIResources.ContainsKey(type))
        {
            UIElement info = this.UIResources[type];
            if (info.Cashe)
            {
                info.Instance.SetActive(false);
            }
            else
            {
                GameObject.Destroy(info.Instance);
                info.Instance = null;
            }
        }
    }
    public void Close<T>()
    {
        this.Close(typeof(T));
    }
    #endregion
}


