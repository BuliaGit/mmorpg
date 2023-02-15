using Assets.Scripts.Managers.Me;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using UnityEngine.Events;
using Debug = UnityEngine.Debug;

namespace Assets.Scripts.Services
{
    internal class GuildService : Singleton<GuildService>, IDisposable
    {
        //公会更新事件
        public UnityAction OnGuildUpdate;
        //公会创建结果事件
        public UnityAction<bool> OnGuildCreateResult;
        //公会列表更新事件
        public UnityAction<List<NGuildInfo>> OnGuildListResult;


        public GuildService()
        {
            MessageDistributer.Instance.Subscribe<GuildCreateResponse>(OnGuildCreate);
            MessageDistributer.Instance.Subscribe<GuildListResponse>(OnGuildList);
            MessageDistributer.Instance.Subscribe<GuildJoinRequest>(OnGuildJoinRequest);
            MessageDistributer.Instance.Subscribe<GuildJoinResponse>(OnGuildJoinResponse);
            MessageDistributer.Instance.Subscribe<GuildResponse>(OnGuild);
            MessageDistributer.Instance.Subscribe<GuildLeaveResponse>(OnGuildLeave);
            MessageDistributer.Instance.Subscribe<GuildAdminResponse>(OnGuildAdmin);
        }

        

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<GuildCreateResponse>(OnGuildCreate);
            MessageDistributer.Instance.Unsubscribe<GuildListResponse>(OnGuildList);
            MessageDistributer.Instance.Unsubscribe<GuildJoinRequest>(OnGuildJoinRequest);
            MessageDistributer.Instance.Unsubscribe<GuildJoinResponse>(OnGuildJoinResponse);
            MessageDistributer.Instance.Unsubscribe<GuildResponse>(OnGuild);
            MessageDistributer.Instance.Unsubscribe<GuildLeaveResponse>(OnGuildLeave);
            MessageDistributer.Instance.Unsubscribe<GuildAdminResponse>(OnGuildAdmin);
        }
        internal void Init()
        {

        }

        /// <summary>
        /// 公会创建响应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="response"></param>
        private void OnGuildCreate(object sender, GuildCreateResponse response)
        {
            Debug.Log(string.Format("OnGuildCreateResponse:{0}", response.Result));
            if (OnGuildCreateResult != null)
            {
                OnGuildCreateResult(response.Result==Result.Success);
            }
            if (response.Result == Result.Success)
            {
                GuildManager.Instance.Init(response.guildInfo);
                MessageBox.Show(String.Format("{0}公会创建成功！", response.guildInfo.GuildName), "创建公会");
            }
            else
            {
                MessageBox.Show(String.Format("{0}公会创建失败！", response.guildInfo.GuildName), "创建公会");
            }
        }

        /// <summary>
        /// 公会列表响应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void OnGuildList(object sender, GuildListResponse response)
        {
            if(OnGuildListResult != null)
            {
                OnGuildListResult(response.Guilds);
            }
        }

        /// <summary>
        /// 收到加入公会请求
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void OnGuildJoinRequest(object sender, GuildJoinRequest request)
        {
            var confirm = MessageBox.Show(string.Format("{0}申请加入公会", request.Apply.Name), "公会申请", MessageBoxType.Confirm);
            confirm.OnYes = () =>
            {
                SendGuildJoinResponse(true, request);
            };
            confirm.OnNo = () =>
            {
                SendGuildJoinResponse(false, request);
            };
        }

        /// <summary>
        /// 收到加入公会响应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void OnGuildJoinResponse(object sender, GuildJoinResponse response)
        {
            Debug.LogFormat("OnGuildJoinResponse：{0}", response.Result);
            if(response.Result == Result.Success)
            {
                MessageBox.Show("加入公会成功", "公会");
            }
            else
            {
                MessageBox.Show("加入公会失败", "公会");
            }
        }


        /// <summary>
        /// 请求公会信息响应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void OnGuild(object sender, GuildResponse message)
        {
            Debug.LogFormat("OnGuild:{0}{1}{2}", message.Result, message.guildInfo.Id, message.guildInfo.GuildName);
            GuildManager.Instance.Init(message.guildInfo);
            if (OnGuildUpdate != null)
            {
                OnGuildUpdate();
            }
        }

        /// <summary>
        /// 请求离开公会响应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void OnGuildLeave(object sender, GuildLeaveResponse response)
        {
            if(response.Result == Result.Success)
            {
                GuildManager.Instance.Init(null);
                MessageBox.Show("离开公会成功", "公会");
            }
            else
            {
                MessageBox.Show("离开公会失败", "公会",MessageBoxType.Error);
            }
        }

        /// <summary>
        /// 公会权限请求的响应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void OnGuildAdmin(object sender, GuildAdminResponse message)
        {
            MessageBox.Show(string.Format("执行操作：{0}，结果：{1}，信息：{2}", message.Command, message.Result, message.Errormsg));
        }







        /// <summary>
        /// 发送公会创建请求
        /// </summary>
        /// <param name="guildName"></param>
        /// <param name="guildAim"></param>
        internal void SendGuildCreate(string guildName, string guildAim)
        {
            Debug.Log("SendGuildCreate");
            NetMessage msg = new NetMessage();
            msg.Request = new NetMessageRequest();
            msg.Request.guildCreate = new GuildCreateRequest
            {
                GuildName = guildName,
                GuildNotice = guildAim
            };
            NetClient.Instance.SendMessage(msg);
        }

        /// <summary>
        /// 发送加入公会请求
        /// </summary>
        /// <param name="guildJoinID"></param>
        internal void SendGuildJoinRequest(int guildJoinID)
        {
            Debug.Log("SendGuildJoinRequest");
            NetMessage msg = new NetMessage();
            msg.Request = new NetMessageRequest();
            msg.Request.guildJoinReq = new GuildJoinRequest
            {
                Apply = new NGuildApplyInfo
                {
                    GuildId = guildJoinID
                }
            };
            NetClient.Instance.SendMessage(msg);
        }

        /// <summary>
        /// 发送别人加入公会请求的回应
        /// </summary>
        /// <param name="accept"></param>
        /// <param name="request"></param>
        public void SendGuildJoinResponse(bool accept,GuildJoinRequest request)
        {
            Debug.Log("SendGuildJoinResponse");
            NetMessage msg = new NetMessage();
            msg.Request = new NetMessageRequest();
            msg.Request.guildJoinRes = new GuildJoinResponse
            {
                Result = Result.Success,
                Apply = request.Apply
            };
            msg.Request.guildJoinRes.Apply.Result = accept ? ApplyResult.ApplyResultAccept : ApplyResult.ApplyResultReject;
            NetClient.Instance.SendMessage(msg);
        }

        /// <summary>
        /// 发送加入公会审批
        /// </summary>
        /// <param name="accept"></param>
        /// <param name="apply"></param>
        public void SendGuildJoinApply(bool accept, NGuildApplyInfo apply)
        {
            Debug.Log("SendGuildJoinApply");
            NetMessage msg = new NetMessage();
            msg.Request = new NetMessageRequest();
            msg.Request.guildJoinRes = new GuildJoinResponse
            {
                Result = Result.Success,
                Apply = apply
            };
            msg.Request.guildJoinRes.Apply.Result = accept ? ApplyResult.ApplyResultAccept : ApplyResult.ApplyResultReject;
            NetClient.Instance.SendMessage(msg);
        }



        /// <summary>
        /// 发送公会列表请求
        /// </summary>
        internal void SendGuildListRequest()
        {
            Debug.Log("SendGuildListRequest");
            NetMessage msg = new NetMessage();
            msg.Request = new NetMessageRequest();
            msg.Request.guildList = new GuildListRequest();
            NetClient.Instance.SendMessage(msg);
        }

        /// <summary>
        /// 发送离开公会请求
        /// </summary>
        public void SendGuildLeaveRequest()
        {
            Debug.Log("SendGuildLeaveRequest");
            NetMessage msg = new NetMessage();
            msg.Request = new NetMessageRequest();
            msg.Request.guildLeave = new GuildLeaveRequest();
            NetClient.Instance.SendMessage(msg);
        }

        /// <summary>
        /// 发送公会信息请求
        /// </summary>
        public void SendGuildRequest()
        {
            Debug.Log("SendGuildRequest");
            NetMessage msg = new NetMessage();
            msg.Request = new NetMessageRequest();
            msg.Request.Guild = new GuildRequest();
            NetClient.Instance.SendMessage(msg);
        }

        /// <summary>
        /// 发送相关管理权限请求
        /// </summary>
        /// <param name="command"></param>
        /// <param name="memberId"></param>
        public void SendAdminCommand(GuildAdminCommand command, int memberId)
        {
            Debug.Log("SendAdminCommand");
            NetMessage msg = new NetMessage();
            msg.Request = new NetMessageRequest();
            msg.Request.guildAdmin = new GuildAdminRequest()
            {
                Command = command,
                Target = memberId
            };
            NetClient.Instance.SendMessage(msg);
        }
    }
}
