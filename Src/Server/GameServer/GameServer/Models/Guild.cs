using Common.Utils;
using Common;
using GameServer.Entities;
using GameServer.Managers;
using GameServer.Services;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Models
{
    class Guild
    {
        public int Id => Data.Id;
        public string Name => Data.Name;

        public double timestamp;

        public TGuild Data;

        public Guild(TGuild guild)
        {
            Data = guild;
        }

        /// <summary>
        /// 加入公会申请
        /// </summary>
        /// <param name="apply"></param>
        /// <returns></returns>
        internal bool JoinApply(NGuildApplyInfo apply)
        {
            var oldApply = Data.GuildApplies.FirstOrDefault(v => v.CharacterId == apply.characterId);
            if (oldApply != null) return false;

            var dbApply = DBService.Instance.Entities.TGuildApplies.Create();

            dbApply.GuildId = apply.GuildId;
            dbApply.CharacterId = apply.characterId;
            dbApply.Name = apply.Name;
            dbApply.Class = apply.Class;
            dbApply.Level = apply.Level;
            dbApply.ApplyTime = DateTime.Now;

            DBService.Instance.Entities.TGuildApplies.Add(dbApply);
            Data.GuildApplies.Add(dbApply);

            DBService.Instance.Save();

            timestamp = TimeUtil.timestamp;
            return true;
        }

        internal bool JoinApprove(NGuildApplyInfo apply)
        {
            var oldApply = Data.GuildApplies.FirstOrDefault(v => v.CharacterId == apply.characterId && v.Result == 0);
            if (oldApply == null) return false;

            oldApply.Result = (int)apply.Result;

            if (apply.Result == ApplyResult.ApplyResultAccept)
                AddMember(apply.characterId, apply.Name, apply.Class, apply.Level, GuildTitle.GuildTitleNone);

            DBService.Instance.Save();

            timestamp = TimeUtil.timestamp;
            return true;
        }

        public void AddMember(int character_id, string name, int @class, int level, GuildTitle title)
        {
            var now = DateTime.Now;
            var db_member = new TGuildMember
            {
                CharacterId = character_id,
                Name = name,
                Class = @class,
                Level = level,
                Title = (int)title,
                JoinTime = now,
                LastTime = now
            };

            Data.GuildMembers.Add(db_member);
            
            var character = CharacterManager.Instance.GetCharacter(character_id);
            if (character != null)
            {
                character.Data.GuildId = Id;
            }
            else
            {
                var dbCharacter =
                    DBService.Instance.Entities.Characters.SingleOrDefault(c => c.ID == character_id);
                dbCharacter.GuildId = Id;
            }
       
            timestamp = TimeUtil.timestamp;
        }

        public void Leave(Character member)
        {
            Log.Info($"Leave Guild:{member.Id}:{member.Info.Name}");
            // TODO 这里的逻辑和 AddMember 是相对应的
            //Members.Remove(member);
            //if(member == Leader)
            //{
            //    if (Members.Count > 0)
            //    {
            //        Leader = Members[0];
            //    }
            //    else
            //    {
            //        Leader = null;
            //    }
            //}

            //member.Guild = null;
            timestamp = TimeUtil.timestamp;
        }

        public void PostProcess(Character from, NetMessageResponse message)
        {
            if (message.Guild == null)
                message.Guild = new GuildResponse
                {
                    Result = Result.Success,
                    guildInfo = GuildInfo(from)
                };
        }

        internal NGuildInfo GuildInfo(Character from)
        {
            var info = new NGuildInfo
            {
                Id = Id,
                GuildName = Name,
                Notice = Data.Notice,
                leaderId = Data.LeaderID,
                leaderName = Data.LeaderName,
                createTime = (long)TimeUtil.GetTimestamp(Data.CreateTime),
                memberCount = Data.GuildMembers.Count
            };

            if (from != null) // from 有值，说明是当前公会的成员
            {
                info.Members.AddRange(GetMemberInfos());
                if (from.Id == Data.LeaderID) // 获取人是队长
                    info.Applies.AddRange(GetApplyInfos());
            }

            return info;
        }

        private List<NGuildMemberInfo> GetMemberInfos()
        {
            var members = new List<NGuildMemberInfo>();

            foreach (var member in Data.GuildMembers)
            {
                var memberInfo = new NGuildMemberInfo
                {
                    Id = member.Id,
                    characterId = member.CharacterId,
                    Title = (GuildTitle)member.Title,
                    joinTime = (long)TimeUtil.GetTimestamp(member.JoinTime),
                    lastTime = (long)TimeUtil.GetTimestamp(member.LastTime)
                };

                // TODO 应该增加更多检查
                var character = CharacterManager.Instance.GetCharacter(member.CharacterId);
                if (character != null) // 在线
                {
                    memberInfo.Info = character.GetBasicInfo();
                    memberInfo.Status = 1;
                    member.Level = character.Data.Level;
                    member.Name = character.Data.Name;
                    member.LastTime = DateTime.Now;
                    
                }
                else // 不在线
                {
                    memberInfo.Info = GetMemberInfo(member);
                    memberInfo.Status = 0;
                    
                }
                members.Add(memberInfo);
            }

            return members;
        }

        private NCharacterInfo GetMemberInfo(TGuildMember member)
        {
            return new NCharacterInfo
            {
                Id = member.CharacterId,
                Name = member.Name,
                Class = (CharacterClass)member.Class,
                Level = member.Level
            };
        }

        private List<NGuildApplyInfo> GetApplyInfos()
        {
            List<NGuildApplyInfo> applies = new List<NGuildApplyInfo>();
            foreach (var apply in Data.GuildApplies)
            {
                if (apply.Result != (int)ApplyResult.ApplyResultNone) continue;

                // 没有经过审批的申请
                applies.Add(new NGuildApplyInfo()
                {
                    characterId = apply.CharacterId,
                    GuildId = apply.GuildId,
                    Class = apply.Class, 
                    Level = apply.Level,
                    Name = apply.Name,
                    Result = (ApplyResult)apply.Result
                });
            }

            return applies;
        }

        private TGuildMember GetDBMember(int characterID)
        {
            foreach (var member in Data.GuildMembers)
                if (member.CharacterId == characterID)
                    return member;

            return null;
        }

        public void ExecuteAdmin(GuildAdminCommand command, int targetID, int sourceID)
        {
            var target = GetDBMember(targetID);
            var source = GetDBMember(sourceID);

            switch (command)
            {
                case GuildAdminCommand.GuildAdminCommandPromote:
                    target.Title = (int)GuildTitle.GuildTitleVicePresident;
                    break;
                case GuildAdminCommand.GuildAdminCommandDepost:
                    target.Title = (int)GuildTitle.GuildTitleNone;
                    break;
                case GuildAdminCommand.GuildAdminCommandTransfer:
                    target.Title = (int)GuildTitle.GuildTitlePresident;
                    source.Title = (int)GuildTitle.GuildTitleNone;
                    Data.LeaderID = targetID;
                    Data.LeaderName = target.Name;
                    break;
                case GuildAdminCommand.GuildAdminCommandKickout:
                    // TODO
                    break;
            }

            DBService.Instance.Save();
            timestamp = TimeUtil.timestamp;
        }
    }
}
