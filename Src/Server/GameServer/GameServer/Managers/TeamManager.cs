using Common;
using GameServer.Entities;
using GameServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Managers
{
    class TeamManager : Singleton<TeamManager>
    {
        public List<Team> Teams = new List<Team>();
        public Dictionary<int, Team> CharacterTeams = new Dictionary<int, Team>();

        public void Init() { }

        public Team GetTeamByCharacter(int characterId)
        {
            Team team = null;
            CharacterTeams.TryGetValue(characterId, out team);
            return team;
        }

        internal void AddTeamMember(Character leader, Character member)
        {
            if (leader.Team == null)
                leader.Team = CreatTeam(leader);
            leader.Team.AddMember(member);
        }

        private Team CreatTeam(Character leader)
        {
            Team team = null;
            for(int i = 0; i < Teams.Count; i++)
            {
                team = Teams[i];
                if(team.Members.Count == 0)
                {
                    team.AddMember(leader);
                    return team;
                }
            }
            team = new Team(leader);
            Teams.Add(team);
            team.Id = Teams.Count;
            return team;
        }
    }
}
