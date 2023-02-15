using Assets.Scripts.Managers.Me;
using Assets.Scripts.Services;
using Managers;
using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGuild : UIWindowMe 
{
    public GameObject guildItem;
    public Transform itemRoot;
    public ListViewMe listView;
    public UIGuildInfo uiGuildInfo;
    public UIGuildItem selectItem;

    public GameObject panelAdmin;
    public GameObject panelLeader;

    public void Start()
    {
        GuildService.Instance.OnGuildUpdate += UpdateGuildUI;
        listView.OnItemSelect += OnGuildItemSelect;
        UpdateGuildUI();
    }

    public void OnDestroy()
    {
        GuildService.Instance.OnGuildUpdate -= UpdateGuildUI;
    }


    private void UpdateGuildUI()
    {
        uiGuildInfo.GuildInfo = GuildManager.Instance.guildInfo;
        ClearGuildUI();
        InitGuildUI();

        panelAdmin.SetActive(GuildManager.Instance.myMemberInfo.Title > GuildTitle.GuildTitleNone);
        panelLeader.SetActive(GuildManager.Instance.myMemberInfo.Title == GuildTitle.GuildTitlePresident);
    }

    private void InitGuildUI()
    {
        foreach (var member in GuildManager.Instance.guildInfo.Members)
        {
            GameObject itemGO = Instantiate(guildItem, itemRoot);
            UIGuildItem itemUI = itemGO.GetComponent<UIGuildItem>();
            itemUI.SetGuildItemInfo(member);
            listView.AddItem(itemUI);
        }
    }

    private void ClearGuildUI()
    {
        listView.RemoveAll();
    }

    private void OnGuildItemSelect(ListViewMe.ListViewItemMe item)
    {
        selectItem = item as UIGuildItem;
    }

    public void OnClickAppliesList()
    {
        UIManagerMe.Instance.Show<UIGuildApplyList>();
    }
    public void OnClickLeave()
    {
        //自己写
    }
    public void OnClickChat()
    {

    }

    /// <summary>
    /// 踢出成员
    /// </summary>
    public void OnClickKickout()
    {
        if (selectItem == null)
        {
            MessageBox.Show("请选择提出的成员");
            return;
        }
        MessageBox.Show(string.Format("是否把【{0}】踢出公会？", selectItem.Info.Info.Name), "踢出公会", MessageBoxType.Confirm, "是", "否").OnYes = () =>
        {
            GuildService.Instance.SendAdminCommand(GuildAdminCommand.GuildAdminCommandKickout, selectItem.Info.Info.Id);
        };
    }

    /// <summary>
    /// 升职
    /// </summary>
    public void OnClickPromote()
    {
        if (selectItem == null)
        {
            MessageBox.Show("请选择晋升的成员");
            return;
        }
        if (selectItem.Info.Title != GuildTitle.GuildTitleNone)
        {
            MessageBox.Show("对方已经有了管理权限，无法晋升");
            return;
        }
        MessageBox.Show(string.Format("是否晋升【{0}】为公会副会长？", selectItem.Info.Info.Name), "晋升", MessageBoxType.Confirm, "是", "否").OnYes = () =>
        {
            GuildService.Instance.SendAdminCommand(GuildAdminCommand.GuildAdminCommandPromote, selectItem.Info.Info.Id);
        };
    }

    /// <summary>
    /// 罢免
    /// </summary>
    public void OnClickDepose()
    {
        if(selectItem == null)
        {
            MessageBox.Show("请选择罢免的成员");
            return;
        }
        if(selectItem.Info.Title != GuildTitle.GuildTitleVicePresident)
        {
            MessageBox.Show("无法罢免");
            return;
        }
        MessageBox.Show(string.Format("是否罢免【{0}】的权限？", selectItem.Info.Info.Name), "罢免", MessageBoxType.Confirm, "是", "否").OnYes = () =>
        {
            GuildService.Instance.SendAdminCommand(GuildAdminCommand.GuildAdminCommandDepost, selectItem.Info.Info.Id);
        };
    }

    /// <summary>
    /// 转让
    /// </summary>
    public void OnClickTransfer()
    {
        if(selectItem == null)
        {
            MessageBox.Show("请选择要转让为会长的成员");
            return;
        }
        if(selectItem.Info.Title == GuildTitle.GuildTitlePresident)
        {
            MessageBox.Show("此成员为会长，不能转让为自己");
            return;
        }
        MessageBox.Show(string.Format("是否转让会长给【{0}】？", selectItem.Info.Info.Name), "转让会长", MessageBoxType.Confirm, "是", "否").OnYes = () =>
        {
            GuildService.Instance.SendAdminCommand(GuildAdminCommand.GuildAdminCommandTransfer, selectItem.Info.Info.Id);
        };
    }


    /// <summary>
    /// 编辑公会宣言
    /// </summary>
    public void OnClickSetNotice()
    {

    }

}
