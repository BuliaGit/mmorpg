using Common;
using GameServer.Managers;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Services
{
    public class GuildService:Singleton<GuildService>
    {
        public GuildService()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<GuildCreateRequest>(OnGuildCreate);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<GuildListRequest>(OnGuildList);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<GuildJoinRequest>(OnGuildJoinRequest);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<GuildJoinResponse>(OnGuildJoinResponse);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<GuildLeaveRequest>(OnGuildLeave);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<GuildAdminRequest>(OnGuildAdmin);
        }

        public void Init()
        {
            GuildManager.Instance.Init();
        }

        // 收到创建公会消息
        private void OnGuildCreate(NetConnection<NetSession> sender, GuildCreateRequest request)
        {
            var character = sender.Session.Character;

            Log.Info($"OnGuildCreate::GuildName:{request.GuildName} character:[{character.Id}]{character.Info.Name}");

            sender.Session.Response.guildCreate = new GuildCreateResponse();

            if (character.Guild != null)
            {
                sender.Session.Response.guildCreate.Result = Result.Failed;
                sender.Session.Response.guildCreate.Errormsg = "已经有公会";
                sender.SendResponse();
                return;
            }

            if (GuildManager.Instance.CheckNameExisted(request.GuildName))
            {
                sender.Session.Response.guildCreate.Result = Result.Failed;
                sender.Session.Response.guildCreate.Errormsg = "公会名称已存在";
                sender.SendResponse();
                return;
            }

            GuildManager.Instance.CreateGuild(request.GuildName, request.GuildNotice, character);
            sender.Session.Response.guildCreate.guildInfo = character.Guild.GuildInfo(character);
            sender.Session.Response.guildCreate.Result = Result.Success;
            sender.SendResponse();
        }

        private void OnGuildList(NetConnection<NetSession> sender, GuildListRequest request)
        {
            var character = sender.Session.Character;

            Log.Info($"OnGuildList: character:[{character.Id}]{character.Name}");

            sender.Session.Response.guildList = new GuildListResponse();
            sender.Session.Response.guildList.Guilds.AddRange(GuildManager.Instance.GetGuildsInfo());
            sender.Session.Response.guildList.Result = Result.Success;
            sender.SendResponse();
        }

        // 收到加公会请求
        private void OnGuildJoinRequest(NetConnection<NetSession> sender, GuildJoinRequest request)
        {
            var character = sender.Session.Character;

            Log.Info(
                $"OnGuildJoinRequest::GuildId:{request.Apply.GuildId} CharacterId:[{request.Apply.characterId}]{request.Apply.Name}");

            var guild = GuildManager.Instance.GetGuild(request.Apply.GuildId);
            if (guild == null)
            {
                sender.Session.Response.guildJoinRes = new GuildJoinResponse
                {
                    Result = Result.Failed,
                    Errormsg = "公会不存在"
                };
                sender.SendResponse();
                return;
            }

            request.Apply.characterId = character.Data.ID;
            request.Apply.Name = character.Data.Name;
            request.Apply.Class = character.Data.Class;
            request.Apply.Level = character.Data.Level;

            if (guild.JoinApply(request.Apply))
            {
                var leader = SessionManager.Instance.GetSession(guild.Data.LeaderID);
                if (leader != null) // 给会长发送申请加入请求
                {
                    leader.Session.Response.guildJoinReq = request;
                    leader.SendResponse();
                }
            }
            else
            {
                sender.Session.Response.guildJoinRes = new GuildJoinResponse
                {
                    Result = Result.Failed,
                    Errormsg = "请勿重复申请"
                };
                sender.SendResponse();
            }
        }

        // 收到加公会响应
        private void OnGuildJoinResponse(NetConnection<NetSession> sender, GuildJoinResponse response)
        {
            var character = sender.Session.Character;

            Log.Info(
                $"OnGuildJoinResponse::GuildId:{response.Apply.GuildId} CharacterId:[{response.Apply.characterId}]{response.Apply.Name}");

            var guild = GuildManager.Instance.GetGuild(response.Apply.GuildId);
            if (response.Result == Result.Success) // 接受了公会请求
                guild.JoinApprove(response.Apply);

            var requester = SessionManager.Instance.GetSession(response.Apply.characterId);
            if (requester != null)
            {
                requester.Session.Character.Guild = guild;

                
                requester.Session.Response.guildJoinRes = response;
                requester.Session.Response.guildJoinRes.Result = Result.Success;
                requester.Session.Response.guildJoinRes.Errormsg = "加入公会成功";

                requester.SendResponse();
            }
        }

        private void OnGuildLeave(NetConnection<NetSession> sender, GuildLeaveRequest request)
        {
            var character = sender.Session.Character;

            Log.Info($"OnGuildLeave::character:{character.Id}");

            sender.Session.Response.guildLeave = new GuildLeaveResponse();
            character.Guild.Leave(character);
            sender.Session.Response.guildLeave.Result = Result.Success;

            DBService.Instance.Save();
            sender.SendResponse();
        }

        private void OnGuildAdmin(NetConnection<NetSession> sender, GuildAdminRequest message)
        {
            var character = sender.Session.Character;

            Log.Info($"OnGuildAdmin::character:{character.Id}");

            var guildAdminResponse = new GuildAdminResponse();
            sender.Session.Response.guildAdmin = guildAdminResponse;

            if (character.Guild == null)
            {
                guildAdminResponse.Result = Result.Failed;
                guildAdminResponse.Errormsg = "你还没有公会！";
                sender.SendResponse();
                return;
            }

            // TODO 这里还应该判断是否有执行权限

            character.Guild.ExecuteAdmin(message.Command, message.Target, character.Id);

            // TODO 这里应该判断命令是否执行成功

            var target = SessionManager.Instance.GetSession(message.Target);
            if (target != null)
            {
                target.Session.Response.guildAdmin = new GuildAdminResponse
                {
                    Result = Result.Success,
                    Command = message
                };
                target.SendResponse();
            }

            sender.Session.Response.guildAdmin.Result = Result.Success;
            sender.Session.Response.guildAdmin.Command = message;
            sender.SendResponse();
        }
    }
}
