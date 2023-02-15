using Models;
using Network;
using Services;
using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Experimental.UIElements;

public class TeamService : Singleton<TeamService>, IDisposable
{
    public TeamService()
    {
        MessageDistributer.Instance.Subscribe<TeamInviteRequest>(OnTeamInviteRequest); //别人的组队请求
        MessageDistributer.Instance.Subscribe<TeamInviteResponse>(OnTeamInviteResponse);
        MessageDistributer.Instance.Subscribe<TeamInfoResponse>(OnTeamInfo);
        MessageDistributer.Instance.Subscribe<TeamLeaveResponse>(OnTeamLeave);
    }
    public void Dispose()
    {
        MessageDistributer.Instance.Unsubscribe<TeamInviteRequest>(OnTeamInviteRequest);
        MessageDistributer.Instance.Unsubscribe<TeamInviteResponse>(OnTeamInviteResponse);
        MessageDistributer.Instance.Unsubscribe<TeamInfoResponse>(OnTeamInfo);
        MessageDistributer.Instance.Unsubscribe<TeamLeaveResponse>(OnTeamLeave);
    }
    public void Init()
    {
    }

    /// <summary>
    /// 发送组队请求
    /// </summary>
    /// <param name="friendId"></param>
    /// <param name="friendName"></param>
    public void SendTeamInviteRequest(int friendId, string friendName)
    {
        Debug.Log("SendTeamInviteRequest");
        NetMessage msg = new NetMessage();
        msg.Request = new NetMessageRequest();
        msg.Request.teamInviteReq = new TeamInviteRequest()
        {
            //真的服了这里
            //TeamId = User.Instance.TeamInfo.Id,
            FromId = User.Instance.CurrentCharacterInfo.Id,
            FromName = User.Instance.CurrentCharacterInfo.Name,
            ToId = friendId,
            ToName = friendName
        };
        NetClient.Instance.SendMessage(msg);
    }

    /// <summary>
    /// 发送组队离开请求
    /// </summary>
    public void SendTeamLeaveRequest()
    {
        Debug.Log("SendTeamInviteRequest");
        NetMessage msg = new NetMessage();
        msg.Request = new NetMessageRequest();
        msg.Request.teamLeave = new TeamLeaveRequest()
        {
            TeamId = User.Instance.TeamInfo.Id,
            characterId = User.Instance.CurrentCharacterInfo.Id
        };
        NetClient.Instance.SendMessage(msg);
    }

    /// <summary>
    /// 发送别人组队请求回应
    /// </summary>
    /// <param name="isAgree"></param>
    /// <param name="quest"></param>
    public void SendTeamInviteResponse(bool isAgree,TeamInviteRequest quest)
    {
        Debug.Log("SendTeamInviteResponse");
        NetMessage msg = new NetMessage();
        msg.Response = new NetMessageResponse();
        msg.Response.teamInviteRes = new TeamInviteResponse()
        {
            Result = isAgree ? Result.Success : Result.Failed,
            Errormsg = isAgree ? "组队成功" : "对方拒绝了好友请求",
            Request = quest
        };
        NetClient.Instance.SendMessage(msg);
    }



    /// <summary>
    /// 收到别人组队请求时
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="request"></param>
    private void OnTeamInviteRequest(object sender, TeamInviteRequest request)
    {
        var uiMsgBox =  MessageBox.Show(String.Format("{0}-邀请您加入组队", request.FromName), "邀请组队", MessageBoxType.Confirm, "接受", "拒绝");
        uiMsgBox.OnYes = () => { SendTeamInviteResponse(true, request); };
        uiMsgBox.OnNo = () => { SendTeamInviteResponse(false, request); };
    }

    /// <summary>
    /// 收到组队请求回应时
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="response"></param>
    private void OnTeamInviteResponse(object sender, TeamInviteResponse response)
    {
        Debug.Log("OnTeamInviteResponse");
        if(response.Result == Result.Success)
        {
            MessageBox.Show(String.Format("邀请-{0}-组队成功",response.Request.ToName), "邀请成功");
        }
        else
        {
            MessageBox.Show(response.Errormsg, "邀请失败");
        }
    }

    /// <summary>
    /// 收到队伍信息时
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="response"></param>
    private void OnTeamInfo(object sender, TeamInfoResponse response)
    {
        Debug.Log("OnTeamInfo");
        TeamManagerMe.Instance.UpdateTeamInfo(response.Team);
    }

    /// <summary>
    /// 收到离开请求回应时
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="response"></param>
    private void OnTeamLeave(object sender, TeamLeaveResponse response)
    {
        Debug.Log("OnTeamLeave");
        if(response.Result == Result.Success)
        {
            TeamManagerMe.Instance.UpdateTeamInfo(null);
            MessageBox.Show("退出成功", "退出队伍");
        }
        else
        {
            MessageBox.Show(response.Errormsg, "退出队伍");
        }
    }

    
}
