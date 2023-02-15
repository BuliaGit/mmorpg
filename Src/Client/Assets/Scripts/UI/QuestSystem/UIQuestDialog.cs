using Models;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 任务对话框
/// </summary>
public class UIQuestDialog : UIWindowMe
{
    public UIQuestInfoMe questInfo;
    public Text dialogTxt;
    public Quest quest;

    /// <summary>
    /// 任务接受按钮
    /// </summary>
    public GameObject openButtons;
    /// <summary>
    /// 任务提交按钮
    /// </summary>
    public GameObject submiButtons;

    /// <summary>
    /// 设置任务信息
    /// </summary>
    /// <param name="quest"></param>
    public void SetQuest(Quest quest)
    {
        this.quest = quest;
        this.UpdateQuest();
        dialogTxt.text = quest.Define.Dialog;
        if (this.quest.Info == null)    //新任务，之前没做过
        {
            openButtons.SetActive(true);
            submiButtons.SetActive(false);
        }
        else
        {
            if (this.quest.Info.Status == SkillBridge.Message.QuestStatus.Complated)
            {
                openButtons.SetActive(false);
                submiButtons.SetActive(true);
            }
            else //正常流程不会执行到这里，因为在QuestManager.ShowQuestDialog已经校验过
            {
                openButtons.SetActive(false);
                submiButtons.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 更新任务信息
    /// </summary>
    private void UpdateQuest()
    {
        if (this.quest != null)
        {
            if (this.questInfo != null)
            {
                this.questInfo.SetQuestInfo(quest);
            }
        }
    }
}