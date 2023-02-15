using Models;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 任务项
/// </summary>
public class UIQuestItem : ListView.ListViewItem
{
    public Quest quest;
    public Text title;

    public Image background;
    public Sprite nomalBg;
    public Sprite SelectedBg;

    
    public override void onSelected(bool selected)
    {
        this.background.overrideSprite = selected ? SelectedBg : nomalBg;
    }


    bool isEquiped = false;
    /// <summary>
    /// 设置任务项信息
    /// </summary>
    /// <param name="item"></param>
    public void SetQUestInfo(Quest item)
    {
        this.quest = item;
        if (this.title != null)
        {
            this.title.text = this.quest.Define.Name; 
        }
    }
}