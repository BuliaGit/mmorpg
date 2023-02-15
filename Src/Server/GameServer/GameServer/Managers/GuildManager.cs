using Common;
using Common.Utils;
using GameServer.Entities;
using GameServer.Models;
using GameServer.Services;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Managers
{
    class GuildManager : Singleton<GuildManager>
    {
        public Dictionary<int, Guild> Guilds = new Dictionary<int, Guild>();
        private HashSet<string> GuildNames = new HashSet<string>();
        public void Init()
        {
            Guilds.Clear();
            foreach (var guild in DBService.Instance.Entities.TGuilds)
            {
                AddGuild(new Guild(guild));
            } 
        }

        private void AddGuild(Guild guild)
        {
            Guilds.Add(guild.Id, guild);
            GuildNames.Add(guild.Name);
            guild.timestamp = TimeUtil.timestamp;
        }

        internal bool CheckNameExisted(string guildName)
        {
            return GuildNames.Contains(guildName);
        }

        public bool CreateGuild(string name, string notice, Character leader)
        {
            var now = DateTime.Now;
            var dbGuild = DBService.Instance.Entities.TGuilds.Create();

            dbGuild.Name = name;
            dbGuild.Notice = notice;
            dbGuild.LeaderID = leader.Id;
            dbGuild.LeaderName = leader.Name;
            dbGuild.CreateTime = now;

            DBService.Instance.Entities.TGuilds.Add(dbGuild);

            var guild = new Guild(dbGuild);
            guild.AddMember(leader.Id, leader.Name, leader.Data.Class, leader.Data.Level, GuildTitle.GuildTitlePresident);

            leader.Guild = guild;
            DBService.Instance.Save();
            leader.Data.GuildId = dbGuild.Id;
            DBService.Instance.Save();
            AddGuild(guild);

            return true;
        }

        internal Guild GetGuild(int guildId)
        {
            if (guildId == 0) return null;
            Guild guild = null;
            Guilds.TryGetValue(guildId, out guild);
            return guild;
        }

        internal List<NGuildInfo> GetGuildsInfo()
        {
            var result = new List<NGuildInfo>();
            foreach (var kv in Guilds) result.Add(kv.Value.GuildInfo(null));

            return result;
        }
    }
}
