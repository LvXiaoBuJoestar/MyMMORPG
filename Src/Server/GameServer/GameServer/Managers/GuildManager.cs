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

        internal void Init()
        {
            Guilds.Clear();
            foreach(var  guild in DBService.Instance.Entities.Guilds)
            {
                AddGuild(new Guild(guild));
            }
        }

        void AddGuild(Guild guild)
        {
            this.Guilds.Add(guild.Id, guild);
            GuildNames.Add(guild.Name);
            guild.timestamp = TimeUtil.timestamp;
        }

        public bool CheckNameExisted(string name)
        {
            return GuildNames.Contains(name);
        }

        public bool CreateGuild(string name, string notice, Character leader)
        {
            DateTime now = DateTime.Now;
            TGuild dbGuild = DBService.Instance.Entities.Guilds.Create();
            dbGuild.Name = name;
            dbGuild.Notice = notice;
            dbGuild.LeaderId = leader.Id;
            dbGuild.LeaderName = leader.Info.Name;
            dbGuild.CreateTime = now;
            DBService.Instance.Entities.Guilds.Add(dbGuild);

            Guild guild = new Guild(dbGuild);
            guild.AddMember(leader.Id, leader.Info.Name, leader.Data.Class, leader.Data.Level, SkillBridge.Message.GUILDTitle.President);
            leader.Guild = guild;
            DBService.Instance.Save();
            leader.Data.GuildId = dbGuild.Id;
            DBService.Instance.Save();
            this.AddGuild(guild);

            return true;
        }

        internal Guild GetGuild(int guildId)
        {
            if(guildId == 0) return null;
            Guild guild = null;
            Guilds.TryGetValue(guildId, out guild);
            return guild;
        }

        internal List<NGuildInfo> GetGuildsInfo()
        {
            List<NGuildInfo> result = new List<NGuildInfo>();
            foreach(var kv in Guilds)
            {
                result.Add(kv.Value.GuildInfo(null));
            }
            return result;
        }
    }
}
