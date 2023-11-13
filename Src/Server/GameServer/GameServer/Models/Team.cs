using Common;
using GameServer.Entities;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Models
{
    class Team
    {
        public int Id;
        public Character Leader;

        public List<Character> Members = new List<Character>();

        public int timestamp;

        public Team(Character leader)
        {
            AddMember(leader);
        }

        public void AddMember(Character member)
        {
            if (Members.Count == 0)
                Leader = member;
            Members.Add(member);
            member.Team = this;
            timestamp = Time.timestamp;
        }

        public void Leave(Character member)
        {
            Log.InfoFormat("Leave Team: {0}, {1}", member.Id, member.Info.Name);
            Members.Remove(member);
            if(member == Leader)
            {
                if (Members.Count > 0)
                    Leader = Members[0];
                else
                    Leader = null;
            }
            member.Team = null;
            timestamp = Time.timestamp;
        }

        public void PostProcess(NetMessageResponse message)
        {
            if(message.teamInfo == null)
            {
                message.teamInfo = new TeamInfoResponse();
                message.teamInfo.Result = Result.Success;
                message.teamInfo.Team = new NteamInfo();
                message.teamInfo.Team.Id = this.Id;
                message.teamInfo.Team.Leader = this.Leader.Id;
                foreach(var member in Members)
                {
                    message.teamInfo.Team.Members.Add(member.GetBasicInfo());
                }
            }
        }
    }
}
