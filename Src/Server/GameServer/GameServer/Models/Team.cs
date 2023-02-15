using System.Collections.Generic;
using Common;
using Common.Utils;
using GameServer.Entities;

using SkillBridge.Message;

namespace Server.Models
{
    class Team
    {
        public List<Character> Members = new List<Character>();
        public int Id;
        public Character Leader;
        public double timestamp;

        public Team(Character leader)
        {
            AddMember(leader);
        }

        public void AddMember(Character member)
        {
            if (Members.Count == 0) Leader = member;

            Members.Add(member);
            member.Team = this;
            timestamp = TimeUtil.timestamp;
        }

        // TODO 改名
        public void Leave(Character member)
        {
            Log.Info($"Leave Team:{member.Id}:{member.Info.Name}");

            Members.Remove(member);
            if (member == Leader) Leader = Members.Count > 0 ? Members[0] : null;

            member.Team = null;
            timestamp = TimeUtil.timestamp; // TODO 感觉这种写法可能会因为服务器的精度问题而出 Bug
                                        // TODO PostProcess 的出现本质上是因为消息有不同的优先级，优先级较低的消息可以捎带发送
        }

        public void PostProcess(NetMessageResponse message)
        {
            if (message.teamInfo != null) return;
            message.teamInfo = new TeamInfoResponse
            {
                Result = Result.Success,
                Team = new NTeamInfo
                {
                    Id = Id,
                    Leader = Leader.Id
                }
            };

            foreach (var member in Members)
            {
                message.teamInfo.Team.Members.Add(member.GetBasicInfo());
            }
        }
    }
}