using Assets.Scripts.Services;
using SkillBridge.Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGuildApplyItem : ListViewMe.ListViewItemMe
{
    public Text applyName;
    public Text level;
    public Text applyClass;

    public NGuildApplyInfo nGuildApplyInfo;

    public void SetApplyItemUIInfo(NGuildApplyInfo applyInfo)
    {
        nGuildApplyInfo = applyInfo;
        if (nGuildApplyInfo == null) return;
        applyName.text = nGuildApplyInfo.Name;
        level.text = nGuildApplyInfo.Level.ToString();
        switch (nGuildApplyInfo.Class)
        {
            case 1:
                applyClass.text = "战士";
                break;
            case 2:
                applyClass.text = "法师";
                break;
            case 3:
                applyClass.text = "弓箭手";
                break;
        }
    }

    /// <summary>
    /// 同意入会
    /// </summary>
    public void OnAccept()
    {
        MessageBox.Show(string.Format("要同意【{0}】的入会申请吗？", nGuildApplyInfo.Name, MessageBoxType.Confirm, "同意加入", "取消")).OnYes = () =>
        {
            GuildService.Instance.SendGuildJoinApply(true, nGuildApplyInfo);
        };
    }

    /// <summary>
    /// 拒绝入会
    /// </summary>
    public void OnReject()
    {
        MessageBox.Show(string.Format("要拒绝【{0}】的入会申请吗？", nGuildApplyInfo.Name, MessageBoxType.Confirm, "拒绝加入", "取消")).OnNo = () =>
        {
            GuildService.Instance.SendGuildJoinApply(false, nGuildApplyInfo);
        };
    }
}
