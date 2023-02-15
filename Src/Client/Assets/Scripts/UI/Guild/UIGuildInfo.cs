using SkillBridge.Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGuildInfo : MonoBehaviour 
{
    public Text guildName;
    public Text guildID;
    public Text guildLeaderName;
    public Text guildAim;
    public Text guildMemberCount;

    private NGuildInfo guildInfo;

    public NGuildInfo GuildInfo
    {
        get { return guildInfo; }
        set { guildInfo = value;UpdateGuildInfo(); }
    }


    public void UpdateGuildInfo()
    {
        if (GuildInfo != null)
        {
            guildName.text = GuildInfo.GuildName;
            guildID.text = string.Format("公会ID：{0}", GuildInfo.Id.ToString());
            guildLeaderName.text = string.Format("会长：{0}", GuildInfo.leaderName.ToString());
            guildAim.text = GuildInfo.Notice;
            guildMemberCount.text = string.Format("成员数量：{0}/{1}",GuildInfo.memberCount,100);
        }
        else
        {
            //清空信息
            guildName.text = "无";
            guildID.text = string.Format("公会ID：{0}", string.Empty);
            guildLeaderName.text = string.Format("会长：{0}", string.Empty);
            guildAim.text = string.Empty;
            guildMemberCount.text = string.Empty;
        }
    }
}
