using Managers;
using Models;
using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIQuestSystemMe : UIWindowMe
{
    //标题
    public Text title;
    //主线列表
    public ListViewMe mainList;
    //支线列表
    public ListViewMe branchList;
    //当前任务开关
    public Toggle currentQuestToggle;
    //可接任务开关
    public Toggle acceptableQuestToggle;
    //列表开关组
    public ToggleGroup listToggleGroup;
    //任务项预制体
    public GameObject questItemPrefab;
    //任务信息脚本
    public UIQuestInfoMe uiQuestInfo;



    //显示可接任务（默认显示进行中任务）
    private bool showAvailableList = false;

    private void Start()
    {
        currentQuestToggle.onValueChanged.AddListener((bool ison) => { QuestToggleOnClick(currentQuestToggle, ison); });
        acceptableQuestToggle.onValueChanged.AddListener((bool ison) => { QuestToggleOnClick(acceptableQuestToggle, ison); });
        RefreshUI();
    }

    private void QuestToggleOnClick(Toggle toggle, bool ison)
    {
        showAvailableList = toggle.name == "AcceptableQuest" ? true : false;
        title.text = toggle.name == "AcceptableQuest" ? "可接任务" : "当前任务";
        RefreshUI();
    }
    private void RefreshUI()
    {
        //清除任务列表
        ClearQuestList();
        //清除任务信息
        ClearQuestInfo();
        InitQuestList();
    }

    private void InitQuestList()
    {
        int i = 1;
        foreach (var quest in QuestManager.Instance.allQuests)
        {
            if (showAvailableList)//可接
            {
                if (quest.Value.Info != null)//已接
                {
                    continue;
                }
            }
            else//进行中
            {
                if (quest.Value.Info == null||quest.Value.Info.Status == QuestStatus.Finished)//已完成
                {
                    continue;
                }
            }
            GameObject questItemUIGO = Instantiate(questItemPrefab, quest.Value.Define.Type == Common.Data.QuestType.Main ? mainList.transform : branchList.transform);
            UIQuestItemMe questItemUI = questItemUIGO.GetComponent<UIQuestItemMe>();
            questItemUI.SetQuestItemTxt(quest.Value);
            //设置任务项开关组
            questItemUIGO.GetComponent<Toggle>().group = listToggleGroup;
            //任务项开光注册事件
            questItemUIGO.GetComponent<Toggle>().onValueChanged.AddListener((bool ison) => { QuestItemOnclick(quest, ison); });


            if (quest.Value.Define.Type == Common.Data.QuestType.Main)
            {
                if (i == 1)
                {
                    questItemUIGO.GetComponent<Toggle>().isOn = true;
                    //设置任务详细信息
                    uiQuestInfo.SetQuestInfo(quest.Value);
                }
                mainList.AddItem(questItemUI);

            }
            else
            {
                branchList.AddItem(questItemUI);
            }
            i++;
        }
    }

    /// <summary>
    /// 任务项点击事件（设置任务项详细信息）
    /// </summary>
    /// <param name="quest"></param>
    /// <param name="ison"></param>
    private void QuestItemOnclick(KeyValuePair<int, Quest> quest, bool ison)
    {
        uiQuestInfo.SetQuestInfo(quest.Value);
    }

    private void ClearQuestList()
    {
        mainList.RemoveAll();
        branchList.RemoveAll();
    }
    private void ClearQuestInfo()
    {
        uiQuestInfo.Clear();
    }
}
