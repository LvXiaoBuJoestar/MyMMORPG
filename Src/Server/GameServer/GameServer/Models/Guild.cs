using Common;
using Common.Utils;
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
    internal class Guild
    {
        public int Id { get { return this.Data.Id; } }
        public string Name { get { return this.Data.Name; } }
        public TGuild Data;

        public double timestamp;

        public Guild(TGuild guild)
        {
            this.Data = guild;
        }

        internal bool JoinApply(NGuildApplyInfo apply)
        {
            var oldApply = this.Data.Applies.FirstOrDefault(v => v.CharacterId == apply.characterId);
            if (oldApply != null) return false;

            var dbApply = DBService.Instance.Entities.GuildApplies.Create();
            dbApply.TGuildId = apply.GuildId;
            dbApply.CharacterId = apply.characterId;
            dbApply.Name = apply.Name;
            dbApply.Class = apply.Class;
            dbApply.Level = apply.Level;
            dbApply.ApplyTime = DateTime.Now;

            DBService.Instance.Entities.GuildApplies.Add(dbApply);
            this.Data.Applies.Add(dbApply);
            DBService.Instance.Save();
            this.timestamp = TimeUtil.timestamp;

            return true;
        }

        internal bool JoinAppove(NGuildApplyInfo apply)
        {
            var oldApply = this.Data.Applies.FirstOrDefault(v => v.CharacterId == apply.characterId && v.Result == 0);
            if (oldApply == null) return false;

            oldApply.Result = (int)apply.Result;
            if(apply.Result == ApplyResult.Accept)
                AddMember(apply.characterId, apply.Name, apply.Class, apply.Level, GUILDTitle.None);

            DBService.Instance.Save();
            this.timestamp = TimeUtil.timestamp;

            return true;
        }

        public void AddMember(int characterId, string name, int @class, int level, GUILDTitle title)
        {
            DateTime now = DateTime.Now;
            TGuildMember dbMember = new TGuildMember()
            {
                CharacterId = characterId,
                Name = name,
                Class = @class,
                Level = level,
                Title = (int)title,
                JoinTime = now,
                LastTime = now,
            };
            this.Data.Members.Add(dbMember);

            Character character = CharacterManager.Instance.GetCharacter(characterId);
            if (character != null)
                character.Data.GuildId = this.Id;
            else
            {
                //DBService.Instance.Entities.Database.ExecuteSqlCommand("UPDATE Characters SET GuildId = @p0 WHERE CharacterId = @p2", this.Id, characterId);
                TCharacter dbChar = DBService.Instance.Entities.Characters.SingleOrDefault(c => c.ID == characterId);
                dbChar.GuildId = this.Id;
            }

            timestamp = TimeUtil.timestamp;
        }

        public void Leave(Character member)
        {
            Log.InfoFormat("Leave Guild: {0}:{1}", member.Id, member.Info.Name);

            var removeItem = this.Data.Members.FirstOrDefault(v => v.CharacterId == member.Id);
            if (removeItem != null)
                DBService.Instance.Entities.GuildMembers.Remove(removeItem);
            var removeApply = this.Data.Applies.FirstOrDefault(v => v.CharacterId == member.Id);
            if (removeApply != null)
                DBService.Instance.Entities.GuildApplies.Remove(removeApply);
            member.Data.GuildId = 0;

            timestamp = TimeUtil.timestamp;
        }

        public void PostProcess(Character from, NetMessageResponse message)
        {
            if(message.Guild == null)
            {
                message.Guild = new GuildResponse();
                message.Guild.Result = Result.Success;
                message.Guild.guildInfo = GuildInfo(from);
            }
        }

        internal NGuildInfo GuildInfo(Character from)
        {
            NGuildInfo info = new NGuildInfo()
            {
                Id = this.Id,
                GuildName = this.Name,
                Notice = this.Data.Notice,
                leaderId = this.Data.LeaderId,
                leaderName = this.Data.LeaderName,
                creatTime = (long)TimeUtil.GetTimestamp(this.Data.CreateTime),
                memberCount = this.Data.Members.Count,
            };

            if(from != null)
            {
                info.Members.AddRange(GetMemberInfos());
                if (from.Id == this.Data.LeaderId)
                    info.Applies.AddRange(GetApplyInfos());
            }
            return info;
        }

        List<NGuildMemberInfo> GetMemberInfos()
        {
            List<NGuildMemberInfo> members = new List<NGuildMemberInfo>();

            foreach(var member in this.Data.Members)
            {
                NGuildMemberInfo memberInfo = new NGuildMemberInfo()
                {
                    Id = member.Id,
                    characterId = member.CharacterId,
                    Title = (GUILDTitle)member.Title,
                    joinTime = (long)TimeUtil.GetTimestamp(member.JoinTime),
                    lastTime = (long)TimeUtil.GetTimestamp(member.LastTime),
                };

                var character = CharacterManager.Instance.GetCharacter(member.CharacterId);
                if (character != null)
                {
                    memberInfo.Info = character.GetBasicInfo();
                    memberInfo.Status = 1;
                    member.Level = character.Data.Level;
                    member.Name = character.Data.Name;
                    member.LastTime = DateTime.Now;
                }
                else
                {
                    memberInfo.Info = GetMemberInfo(member);
                    memberInfo.Status = 0;
                }

                members.Add(memberInfo);
            }
            return members;
        }

        NCharacterInfo GetMemberInfo(TGuildMember member)
        {
            return new NCharacterInfo()
            {
                Id = member.CharacterId,
                Name = member.Name,
                Class = (CharacterClass)member.Class,
                Level = member.Level,
            };
        }

        List<NGuildApplyInfo> GetApplyInfos()
        {
            List<NGuildApplyInfo> applies = new List<NGuildApplyInfo>();
            foreach(var apply in this.Data.Applies)
            {
                if (apply.Result != (int)ApplyResult.None) continue;
                applies.Add(new NGuildApplyInfo()
                {
                    characterId = apply.CharacterId,
                    GuildId = apply.TGuildId,
                    Class = apply.Class,
                    Level = apply.Level,
                    Name = apply.Name,
                    Result = (ApplyResult)apply.Result,
                });
            }
            return applies;
        }

        TGuildMember GetDBMember(int characterId)
        {
            foreach(var member in this.Data.Members)
            {
                if(member.CharacterId == characterId)
                    return member;
            }
            return null;
        }

        internal void ExecuteAdmin(GuildAdminCommand command, int targetId, int sourceId)
        {
            var target = GetDBMember(targetId);
            var source = GetDBMember(sourceId);
            switch(command)
            {
                case GuildAdminCommand.Promote:
                    target.Title = (int)GUILDTitle.VicePresident;
                    break;
                case GuildAdminCommand.Depost:
                    target.Title = (int)GUILDTitle.None;
                    break;
                case GuildAdminCommand.Transfer:
                    target.Title = (int)GUILDTitle.President;
                    source.Title = (int)GUILDTitle.None;
                    this.Data.LeaderId = targetId;
                    this.Data.LeaderName = target.Name;
                    break;
                case GuildAdminCommand.Kickout:
                    var removeItem = this.Data.Members.FirstOrDefault(v => v.CharacterId == targetId);
                    if (removeItem != null)
                        DBService.Instance.Entities.GuildMembers.Remove(removeItem);
                    var removeApply = this.Data.Applies.FirstOrDefault(v => v.CharacterId == targetId);
                    if (removeApply != null)
                        DBService.Instance.Entities.GuildApplies.Remove(removeApply);

                    TCharacter dbChar = DBService.Instance.Entities.Characters.SingleOrDefault(c => c.ID == targetId);
                    dbChar.GuildId = 0;
                    break;
            }
            DBService.Instance.Save();
            timestamp = TimeUtil.timestamp;
        }
    }
}
