using Common.Data;
using Managers;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 任务窗口
/// </summary>
public class UIQuestSystem : UIWindow
{
    public Text title;
    public GameObject itemPrefab;   //任务项
    public TabView Tabs; 
    public ListView listMain;
    public ListView listBranch;

    public UIQuestInfo questInfo;

    /// <summary>
    /// 是否显示可接任务    true:可接  false:进行中
    /// </summary>
    private bool ShowAvailableList = false;

    UIQuestItem questItem;

    void Start()
    {
        this.listMain.onItemSelected += this.OnQuestSelected;
        this.listBranch.onItemSelected += this.OnQuestSelected;
        this.Tabs.OnTabSelect += this.OnSelectTab;

        RefreshUI();
        //QuestManager.Instance.OnQuestChange +=RefreshUI;
    }

    #region OnQuestSelected
    bool first = true;
    private void OnQuestSelected(ListView.ListViewItem item)
    {
        if (first)
        {
            if (item.owner == listBranch)
            {
                questItem.Selected = false;
                listBranch.SelectedItem.Selected = true;
            }
            else if (item.owner == listMain)
            {
                questItem.Selected = false;
                listMain.SelectedItem.Selected = true;
            }
            first = false;
        }
        else
        {
            if (item.owner == listBranch)
            {
                listMain.SelectedItem.Selected = false;
                listBranch.SelectedItem.Selected = true;
            }
            else if (item.owner == listMain)
            {
                listMain.SelectedItem.Selected = true;
                listBranch.SelectedItem.Selected = false;
            }
        }

        UIQuestItem qustItem = item as UIQuestItem;
        this.questInfo.SetQuestInfo(qustItem.quest);
    }
    #endregion

    #region OnSelectTab
    private void OnSelectTab(int idx)
    {
        ShowAvailableList = idx == 1;
        RefreshUI();
    }
    #endregion

    #region RefreshUI
    private void RefreshUI()
    {
        ClearAllQuestList();
        InitAllQustItems();
    }

    /// <summary>
    /// 初始化所有可接任务
    /// </summary>
    private void InitAllQustItems()
    {
        int i = 1;
        foreach (var quest in QuestManager.Instance.allQuests)
        {
            if (ShowAvailableList)//可接
            {
                if (quest.Value.Info != null)  //已接
                    continue;
            }
            else//进行中
            {
                if (quest.Value.Info == null)  //完成了
                    continue;
            }

            GameObject go = Instantiate(itemPrefab, quest.Value.Define.Type == QuestType.Main ? listMain.transform : listBranch.transform);
            UIQuestItem ui = go.GetComponent<UIQuestItem>();
            ui.SetQUestInfo(quest.Value);

            if (quest.Value.Define.Type == QuestType.Main)
            {
                this.listMain.AddItem(ui);
                if (i == 1)
                {
                    questItem = ui;
                    ui.Selected = true;
                    questInfo.SetQuestInfo(ui.quest);
                }
            }
            else
                this.listBranch.AddItem(ui);

            i++;
        }
    }

    private void ClearAllQuestList()
    {
        this.listMain.RemoveAll();
        this.listBranch.RemoveAll();
    }
    #endregion
}