using Common;
using GameServer.Entities;
using GameServer.Managers;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Services
{
    class GuildService : Singleton<GuildService>
    {
        public GuildService()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<GuildJoinRequest>(this.OnGuildJoinRequest);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<GuildJoinResponse>(this.OnGuildJoinResponse);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<GuildCreateRequest>(this.OnGuildCreate);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<GuildListRequest>(this.OnGuildList);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<GuildLeaveRequest>(this.OnGuildLeave);
        }

        public void Init()
        {
            GuildManager.Instance.Init();
        }

        private void OnGuildCreate(NetConnection<NetSession> sender, GuildCreateRequest message)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnGuildCreate: GuildName:{0}, Character:[{1}]{2}", message.GuildName, character.Id, character.Info.Name);
            sender.Session.Response.guildCreate = new GuildCreateResponse();

            if(character.Guild != null)
            {
                sender.Session.Response.guildCreate.Result = Result.Failed;
                sender.Session.Response.guildCreate.Errormsg = "您已经有公会了";
                sender.SendResponse();
                return;
            }
            if (GuildManager.Instance.CheckNameExisted(message.GuildName))
            {
                sender.Session.Response.guildCreate.Result = Result.Failed;
                sender.Session.Response.guildCreate.Errormsg = "该公会名称已存在，请换一个名字吧";
                sender.SendResponse();
                return;
            }

            GuildManager.Instance.CreateGuild(message.GuildName, message.GuildNotice, character);
            sender.Session.Response.guildCreate.guildInfo = character.Guild.GuildInfo(character);
            sender.Session.Response.guildCreate.Result = Result.Success;
            sender.SendResponse();
        }

        private void OnGuildJoinRequest(NetConnection<NetSession> sender, GuildJoinRequest message)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnGuildJoinRequest: GuildID:{0}, Character:[{1}]{2}", message.Apply.GuildId, character.Id, character.Info.Name);
            var guild = GuildManager.Instance.GetGuild(message.Apply.GuildId);

            if(guild == null)
            {
                sender.Session.Response.guildJoinRes = new GuildJoinResponse();
                sender.Session.Response.guildJoinRes.Result = Result.Failed;
                sender.Session.Response.guildJoinRes.Errormsg = "该公会不存在";
                sender.SendResponse();
                return;
            }
            message.Apply.characterId = character.Data.ID;
            message.Apply.Name = character.Data.Name;
            message.Apply.Class = character.Data.Class;
            message.Apply.Level = character.Data.Level;

            if (guild.JoinApply(message.Apply))
            {
                var leader = SessionManager.Instance.GetSession(guild.Data.LeaderId);
                if (leader != null)
                {
                    leader.Session.Response.guildJoinReq = message;
                    leader.SendResponse();
                }
                else
                {
                    sender.Session.Response.guildJoinRes = new GuildJoinResponse();
                    sender.Session.Response.guildJoinRes.Result = Result.Failed;
                    sender.Session.Response.guildJoinRes.Errormsg = "请勿重复申请";
                    sender.SendResponse();
                }
            }
        }

        private void OnGuildJoinResponse(NetConnection<NetSession> sender, GuildJoinResponse message)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnGuildJoinResponse: GuildID:{0}, Character:[{1}]{2}", message.Apply.GuildId, character.Id, character.Info.Name);
            var guild = GuildManager.Instance.GetGuild(message.Apply.GuildId);

            if(message.Result == Result.Success)
                guild.JoinAppove(message.Apply);

            var requester = SessionManager.Instance.GetSession(message.Apply.characterId);
            if(requester != null)
            {
                requester.Session.Character.Guild = guild;

                requester.Session.Response.guildJoinRes = message;
                sender.Session.Response.guildJoinRes.Result = Result.Success;
                sender.Session.Response.guildJoinRes.Errormsg = "加入公会成功";
                sender.SendResponse();
            }
        }

        private void OnGuildList(NetConnection<NetSession> sender, GuildListRequest message)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnGuildList Character:[{0}]{1}", character.Id, character.Info.Name);

            sender.Session.Response.guildList = new GuildListResponse();
            sender.Session.Response.guildList.Guilds.AddRange(GuildManager.Instance.GetGuildsInfo());
            sender.Session.Response.guildList.Result = Result.Success;
            sender.SendResponse();
        }

        private void OnGuildLeave(NetConnection<NetSession> sender, GuildLeaveRequest message)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnGuildLeave Character:[{0}]{1}", character.Id, character.Info.Name);
            sender.Session.Response.guildLeave = new GuildLeaveResponse();

            character.Guild.Leave(character);
            sender.Session.Response.guildLeave.Result = Result.Success;
            DBService.Instance.Save();
            sender.SendResponse();
        }
    }
}
