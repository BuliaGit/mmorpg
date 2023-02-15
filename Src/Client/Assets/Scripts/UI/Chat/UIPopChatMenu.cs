using Services;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIPopChatMenu : UIWindowMe, IDeselectHandler
{
	[HideInInspector]
	public int targetId;
	[HideInInspector]
	public string targetName;
	public void OnDeselect(BaseEventData eventData)
	{
		var ed = eventData as PointerEventData;
		if (ed.hovered.Contains(gameObject)) 
			return;
		Close(WindowResult.None);
	}

	public void OnEnable()
	{
		GetComponent<Selectable>().Select();
		Root.transform.position = Input.mousePosition + new Vector3(80, 0, 0);
	}
	public void OnChat()
	{
		ChatManager.Instance.StartPrivateChat(targetId, targetName);
        Close(WindowResult.No);
    }

	public void OnAddFriend()
	{
		MessageBox.Show(string.Format("确定要加【{0}】为好友吗", targetName), "添加好友", MessageBoxType.Confirm, "确定", "取消").OnYes = () =>
		{
			FriendService.Instance.SendFriendAddRequest(targetId, targetName);
		};
		Close(WindowResult.No);
	}

	public void OnInviteTeam()
	{
        MessageBox.Show(string.Format("确定要邀请【{0}】加入队伍吗", targetName), "邀请组队", MessageBoxType.Confirm, "确定", "取消").OnYes = () =>
        {
            TeamService.Instance.SendTeamInviteRequest(targetId, targetName);
        };
        Close(WindowResult.No);
	}
}
