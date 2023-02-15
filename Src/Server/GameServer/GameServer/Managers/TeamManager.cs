using System.Collections.Generic;
using Common;
using GameServer.Entities;

using Server.Models;


namespace Server.Managers
{

    class TeamManager : Singleton<TeamManager>
    {
        public List<Team> Teams = new List<Team>();
        public Dictionary<int, Team> CharacterTeams = new Dictionary<int, Team>();

        public void Init()
        {

        }

        public Team GetTeamByCharacter(int characterId)
        {
            Team team = null;
            CharacterTeams.TryGetValue(characterId, out  team);
            return team;
        }

        public void AddTeamMember(Character leader, Character member)
        {
            if(leader.Team == null)
            {
                leader.Team = CreateTeam(leader);
            }
            leader.Team.AddMember(member);
        }

        Team CreateTeam(Character leader)
        {
            Team team;
            foreach (var t in Teams)
            {
                team = t;
                if (team.Members.Count != 0) continue;
                // 找到一个空队伍
                team.AddMember(leader);
                return team;
            }

            team = new Team(leader);
            Teams.Add(team);
            team.Id = Teams.Count;
            return team;
        }
    }
}