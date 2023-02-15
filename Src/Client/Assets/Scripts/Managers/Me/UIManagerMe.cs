using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public class UIManagerMe : Singleton<UIManagerMe>
    {
        public bool isOpen;
        public class UIElement
        {
            public string resources;
            public bool cache;
            public GameObject instance;
        }

        public Dictionary<Type, UIElement> UIEleDic;

        public UIManagerMe()
        {
            UIEleDic = new Dictionary<Type, UIElement>();

            UIEleDic.Add(typeof(UISetting), new UIElement() { resources = "UI/UISetting", cache = true });

            UIEleDic.Add(typeof(UIBagMe), new UIElement() { resources = "UI/UIBagme", cache = false });

            UIEleDic.Add(typeof(UIShop), new UIElement() { resources = "UI/UIShop", cache = false });

            UIEleDic.Add(typeof(UIEquip), new UIElement() { resources = "UI/UIEquip", cache = false });

            UIEleDic.Add(typeof(UIQuestSystemMe), new UIElement() { resources = "UI/UIQuestSystemMe", cache = false });
            UIEleDic.Add(typeof(UIQuestDialog), new UIElement() { resources = "UI/UIQuestDialogMe", cache = false });

            UIEleDic.Add(typeof(UIFriends), new UIElement() { resources = "UI/UIFriends", cache = false });

            UIEleDic.Add(typeof(UIGuild), new UIElement() { resources = "UI/Guild/UIGuild", cache = false });
            UIEleDic.Add(typeof(UIGuildList), new UIElement() { resources = "UI/Guild/UIGuildList", cache = false });
            UIEleDic.Add(typeof(UIGuildPopNoGuild), new UIElement() { resources = "UI/Guild/UIGuildPopNoGuild", cache = false });
            UIEleDic.Add(typeof(UIGuildPopCreate), new UIElement() { resources = "UI/Guild/UIGuildPopCreate", cache = false });
            UIEleDic.Add(typeof(UIGuildApplyList), new UIElement() { resources = "UI/Guild/UIGuildApplyList", cache = false });

            UIEleDic.Add(typeof(UIPopChatMenu), new UIElement() { resources = "UI/UIPopChatMenu", cache = false });
            UIEleDic.Add(typeof(UIRide), new UIElement() { resources = "UI/Ride/UIRide", cache = false });

            UIEleDic.Add(typeof(UISystemConfig), new UIElement() { resources = "UI/Sound/UISystemConfig", cache = false });
            UIEleDic.Add(typeof(UISkill), new UIElement() { resources = "UI/Skill/UISkill", cache = false });


        }

        public T Show<T>()
        {
            SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Win_Open);
            isOpen = true;
            Type type = typeof(T);
            if (UIEleDic.ContainsKey(type))
            {
                UIElement uiElement = UIEleDic[type];
                if (uiElement.instance != null)
                {
                    uiElement.instance.SetActive(true);
                }
                else
                {
                    UnityEngine.Object prefab = Resources.Load(uiElement.resources);
                    if (prefab == null)
                    {
                        return default(T);
                    }
                    uiElement.instance = (GameObject)GameObject.Instantiate(prefab);
                }
                return uiElement.instance.GetComponent<T>();
            }
            return default(T);
        }

        public void Close(Type type)
        {
            SoundManager.Instance.PlaySound(SoundDefine.SFX_UI_Win_Close);
            isOpen = false;
            if (UIEleDic.ContainsKey(type))
            {
                UIElement uiElement = UIEleDic[type];
                if (uiElement.cache)
                {
                    uiElement.instance.SetActive(false);
                }
                else
                {
                    GameObject.Destroy(uiElement.instance);
                    uiElement.instance = null;
                }
            }
        }
    }
}