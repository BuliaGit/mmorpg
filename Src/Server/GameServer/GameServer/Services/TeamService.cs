using Common;
using GameServer.Managers;
using Network;
using Server.Managers;

using SkillBridge.Message;

namespace Server.Services
{
    class TeamService : Singleton<TeamService>
    {
        public TeamService()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<TeamInviteRequest>(OnTeamInviteRequest);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<TeamInviteResponse>(OnTeamInviteResponse);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<TeamLeaveRequest>(OnTeamLeave);
        }
        public void Init()
        {
            TeamManager.Instance.Init();
        }

        /// <summary>
        /// 收到组队请求
        /// </summary> 
        /// <param name="sender"></param>
        /// <param name="request"></param>
        void OnTeamInviteRequest(NetConnection<NetSession> sender, TeamInviteRequest request)
        {
            var character = sender.Session.Character;

            Log.Info(
                $"OnTeamInviteRequest::FromId:{request.FromId} FromName:{request.FromName} ToID:{request.ToId} ToName:{request.ToName}");

            var target = SessionManager.Instance.GetSession(request.ToId);

            if (target == null)
            {
                sender.Session.Response.teamInviteRes = new TeamInviteResponse
                {
                    Result = Result.Failed,
                    Errormsg = "好友不在线"
                };
                sender.SendResponse();
                return;
            }

            if (target.Session.Character.Team != null)
            {
                sender.Session.Response.teamInviteRes = new TeamInviteResponse
                {
                    Result = Result.Failed,
                    Errormsg = "对方已经有队伍"
                };
                sender.SendResponse();
                return;
            }

            Log.Info(
                $"ForwardTeamInviteRequest::FromId:{request.FromId} FromName:{request.FromName} ToID:{request.ToId} ToName:{request.ToName}");

            target.Session.Response.teamInviteReq = request;
            target.SendResponse();
        }


        void OnTeamLeave(NetConnection<NetSession> sender, TeamLeaveRequest message)
        {
            var character = sender.Session.Character;

            Log.Info($"OnTeamLeave::character:{character.Id} TeamID:{message.TeamId}:{message.characterId}");

            sender.Session.Response.teamLeave = new TeamLeaveResponse
            {
                Result = Result.Success,
                characterId = message.characterId
            };

            character.Team.Leave(character);

            sender.SendResponse();
        }

        void OnTeamInviteResponse(NetConnection<NetSession> sender, TeamInviteResponse response)
        {
            var character = sender.Session.Character;

            Log.Info(
                $"OnTeamInviteResponse::character:{character.Id} Result:{response.Result} FromId:{response.Request.FromId} ToId:{response.Request.ToId}");

            sender.Session.Response.teamInviteRes = response;

            if (response.Result == Result.Success) // 接受了组队请求
            {
                var requester = SessionManager.Instance.GetSession(response.Request.FromId);
                if (requester == null)
                {
                    sender.Session.Response.teamInviteRes.Result = Result.Failed;
                    sender.Session.Response.teamInviteRes.Errormsg = "请求者已下线";
                }
                else
                {
                    TeamManager.Instance.AddTeamMember(requester.Session.Character, character);
                    requester.Session.Response.teamInviteRes = response;
                    requester.SendResponse(); // 发送给组队的邀请方
                }
            }
            sender.SendResponse(); // 发送给组队的接受方
        }
    }
}