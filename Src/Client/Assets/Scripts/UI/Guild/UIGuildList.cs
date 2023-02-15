using Assets.Scripts.Services;
using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGuildList : UIWindowMe
{
    public GameObject guildListItem;
    public ListViewMe listView;
    public Transform itemRoot;
    public UIGuildInfo guildInfo;
    public UIGuildListItem selectGuildListItem;

    void Start()
    {
        listView.OnItemSelect += OnSelectItem;
        GuildService.Instance.OnGuildListResult += UpdateGuildList;
        GuildService.Instance.SendGuildListRequest();
    }

    void OnDestroy()
    {
        GuildService.Instance.OnGuildListResult -= UpdateGuildList;
    }

    /// <summary>
    /// 选中列表项
    /// </summary>
    /// <param name="selectItem"></param>
    private void OnSelectItem(ListViewMe.ListViewItemMe selectItem)
    {
        //赋值
        selectGuildListItem = selectItem as UIGuildListItem;
        //赋值左边信息并刷新
        guildInfo.GuildInfo = selectGuildListItem.guildInfo;
    }

    public void UpdateGuildList(List<NGuildInfo> guilds)
    {
        ClearGuildList();
        InitGuildList(guilds);
    }

    private void InitGuildList(List<NGuildInfo> guilds)
    {
        foreach (var guildItemInfo in guilds)
        {
            GameObject guildItemGO = Instantiate(guildListItem, itemRoot.transform);
            UIGuildListItem guildItemUI = guildItemGO.GetComponent<UIGuildListItem>();
            guildItemUI.SetGuildListItemInfo(guildItemInfo);
            listView.AddItem(guildItemUI);
        }
    }

    private void ClearGuildList()
    {
        listView.RemoveAll();
    }

    public void OnClickJoin()
    {
        if (selectGuildListItem == null)
        {
            MessageBox.Show("请选择您要加入的公会");
            return;
        }
        else
        {
            MessageBox.Show(String.Format("您确定要加入{0}公会吗？", selectGuildListItem.guildInfo.GuildName), "申请加入公会", MessageBoxType.Confirm, "申请加入", "取消加入").OnYes = () =>
            {
                GuildService.Instance.SendGuildJoinRequest(selectGuildListItem.guildInfo.Id);
            };
        }
    }
}
