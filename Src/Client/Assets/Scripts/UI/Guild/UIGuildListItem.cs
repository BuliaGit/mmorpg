using SkillBridge.Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGuildListItem : ListViewMe.ListViewItemMe 
{
	public Text guildID;
	public Text guildName;
	public Text guildMemberCount;
	public Text guildLeaderName;

	public Image background;
	public Sprite originImg;
	public Sprite selectImg;

	public NGuildInfo guildInfo;


	public override void OnSelect(bool val)
	{
		background.overrideSprite = val ? selectImg : originImg;

    }

	public void SetGuildListItemInfo(NGuildInfo info)
	{
		if (info == null) return;

		guildInfo = info;
		guildID.text = guildInfo.Id.ToString();
		guildName.text = guildInfo.GuildName;
		guildMemberCount.text = guildInfo.memberCount.ToString();
		guildLeaderName.text = guildInfo.leaderName;

    }
}
