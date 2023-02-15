using Common.Utils;
using SkillBridge.Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGuildItem : ListViewMe.ListViewItemMe
{
    public Text name;
    public Text level;
    public Text characterClass;
    public Text job;
    public Text joinTime;
    public Text status;

    public Image background;
    public Sprite originImg;
    public Sprite selectImg;
    public NGuildMemberInfo Info;

    public override void OnSelect(bool val)
    {
        background.overrideSprite = val ? selectImg : originImg;

    }

    public void SetGuildItemInfo(NGuildMemberInfo info)
    {
        Info = info;
        if (Info == null) return;
        name.text = Info.Info.Name;
        level.text = Info.Info.Level.ToString();
        characterClass.text = Info.Info.Class.ToString();
        switch (Info.Title)
        {
            case GuildTitle.GuildTitleNone:
                job.text = "成员";
                break;
            case GuildTitle.GuildTitleVicePresident:
                job.text = "副会长";
                break;
            case GuildTitle.GuildTitlePresident:
                job.text = "会长";
                break;
        }
        joinTime.text = TimeUtil.GetTime(Info.joinTime).ToShortDateString();
        status.text = Info.Status == 1 ? "在线" : "离线";
    }
}
