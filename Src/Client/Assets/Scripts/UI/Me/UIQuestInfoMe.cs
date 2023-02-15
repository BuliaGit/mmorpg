using Common.Data;
using Managers;
using Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIQuestInfoMe : MonoBehaviour
{
    public Text title;
    public Text questDes;
    public Text[] targets;
    public Image[] sprites;
    public Text gold;
    public Text exp;
    public Image originImage;

    //public Text overview;
    public Button navButton;
    private int npc = 0;

    public void SetQuestInfo(Quest quest)
    {
        string typeName = quest.Define.Type == Common.Data.QuestType.Main ? "主线" : "支线";
        title.text = string.Format("【{0}】{1}", typeName, quest.Define.Name);
        questDes.text = quest.Define.Overview;

        //if (overview != null)
        //{
        //    overview.text = quest.Define.Overview;
        //}
        if (questDes != null)
        {
            if (quest.Info == null)
            {
                questDes.text = quest.Define.Dialog;
            }
            else
            {
                if (quest.Info.Status == SkillBridge.Message.QuestStatus.Complated)
                {
                    questDes.text = quest.Define.DialogFinish;
                }
            }
        }

        if (quest.Define.RewardItem1 > 0)
        {
            ItemDefine itemDefine1 = DataManager.Instance.Items[quest.Define.RewardItem1];

            sprites[0].overrideSprite = Resloader.Load<Sprite>(itemDefine1.Icon);
            if (quest.Define.RewardItem2 > 0)
            {
                ItemDefine itemDefine2 = DataManager.Instance.Items[quest.Define.RewardItem2];
                sprites[1].overrideSprite = Resloader.Load<Sprite>("UI/Items/" + itemDefine2.Icon);
                if (quest.Define.RewardItem3 > 0)
                {
                    ItemDefine itemDefine3 = DataManager.Instance.Items[quest.Define.RewardItem3];
                    sprites[2].overrideSprite = Resloader.Load<Sprite>("UI/Items/" + itemDefine3.Icon);
                }
            }
        }


        gold.text = quest.Define.RewardGold.ToString();
        exp.text = quest.Define.RewardExp.ToString();

        if (quest.Info == null)
        {
            npc = quest.Define.AcceptNpc;
        }
        else
        {
            if(quest.Info.Status == SkillBridge.Message.QuestStatus.Complated)
            {
                npc = quest.Define.SubmitNpc;
            }
        }
        if(navButton != null)
        {
            navButton.gameObject.SetActive(npc > 0);
        }

        foreach (var fitter in GetComponentsInChildren<ContentSizeFitter>())
        {
            fitter.SetLayoutVertical();
        }
    }

    public void Clear()
    {
        title.text = string.Empty;
        questDes.text = string.Empty;
        foreach (var item in sprites)
        {
            item.overrideSprite = originImage.sprite;
        }
        gold.text = string.Empty;
        exp.text = string.Empty;
        foreach (var fitter in GetComponentsInChildren<ContentSizeFitter>())
        {
            fitter.SetLayoutVertical();
        }
    }

    public void OnClickNav()
    {
        Vector3 pos = NpcManagerMe.Instance.GetNpcPosition(npc);
        User.Instance.CurrentCharacterObject.StartNav(pos);
        UIManagerMe.Instance.Close(typeof(UIQuestSystemMe));
    }
}
