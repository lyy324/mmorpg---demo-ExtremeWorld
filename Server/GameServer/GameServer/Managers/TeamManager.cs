using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Configuration;
using System.Text;

using Common;
using GameServer.Entities;
using GameServer.Models;

namespace GameServer.Managers
{
    class TeamManager : Singleton<TeamManager>
    {
        public List<Team> Teams = new List<Team>();
        public Dictionary<int, Team> CharacterTeams = new Dictionary<int, Team>();
        public Team GetTeamByCharacter(int characterId)
        {
            Team team = null;
            this.CharacterTeams.TryGetValue(characterId, out team);
            return team;
        }
        internal void AddTeamMember(Character leader, Character member)
        {
            if(leader.Team == null)
            {
                leader.Team = CreateTeam(leader);
            }
            leader.Team.AddMember(member);
        }

        //队伍创建之后就不会销毁 当队伍所有人都离开时 创建队伍时占用这个队伍 
        private Team CreateTeam(Character leader)
        {
            Team team = null;
            for(int i = 0;i <this.Teams.Count;i ++)
            {
                team = this.Teams[i];
                if(team.Members.Count == 0)
                {
                    team.AddMember(leader);
                    return team;
                }
            }
            team = new Team(leader);
            this.Teams.Add(team);
            team.Id = this.Teams.Count;
            return team;
        }

        public void Init()
        {
        }
    }
}
