using Network;
using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking.Types;

namespace Services
{
    public class GuildService : Singleton<GuildService>, IDisposable
    {
        public UnityAction OnGuildUpdate;
        public UnityAction<bool> OnGuildCreateResult;

        public UnityAction<List<NGuildInfo>> OnGuildListResult;

        public void Init() { }

        public GuildService()
        {
            MessageDistributer.Instance.Subscribe<GuildCreateResponse>(OnGuildCreate);
            MessageDistributer.Instance.Subscribe<GuildJoinRequest>(OnGuildJoinRequest);
            MessageDistributer.Instance.Subscribe<GuildJoinResponse>(OnGuildJoinResponse);
            MessageDistributer.Instance.Subscribe<GuildListResponse>(OnGuildList);
            MessageDistributer.Instance.Subscribe<GuildResponse>(OnGuild);
            MessageDistributer.Instance.Subscribe<GuildLeaveResponse>(OnGuildLeave);
            MessageDistributer.Instance.Subscribe<GuildAdminResponse>(OnGuildAdmin);
        }

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<GuildCreateResponse>(OnGuildCreate);
            MessageDistributer.Instance.Unsubscribe<GuildJoinRequest>(OnGuildJoinRequest);
            MessageDistributer.Instance.Unsubscribe<GuildJoinResponse>(OnGuildJoinResponse);
            MessageDistributer.Instance.Unsubscribe<GuildListResponse>(OnGuildList);
            MessageDistributer.Instance.Unsubscribe<GuildResponse>(OnGuild);
            MessageDistributer.Instance.Unsubscribe<GuildLeaveResponse>(OnGuildLeave);
            MessageDistributer.Instance.Unsubscribe<GuildAdminResponse>(OnGuildAdmin);
        }

        public void SendGuildCreate(string guildName, string notice)
        {
            Debug.Log("SendGuildCrete");
            NetMessage netMessage = new NetMessage();
            netMessage.Request = new NetMessageRequest();
            netMessage.Request.guildCreate = new GuildCreateRequest();
            netMessage.Request.guildCreate.GuildName = guildName;
            netMessage.Request.guildCreate.GuildNotice = notice;
            NetClient.Instance.SendMessage(netMessage);
        }

        private void OnGuildCreate(object sender, GuildCreateResponse message)
        {
            Debug.Log("OnGuildCreateResponse");
            if (OnGuildCreateResult != null)
                OnGuildCreateResult(message.Result == Result.Success);
            if (message.Result == Result.Success)
            {
                GuildManager.Instance.Init(message.guildInfo);
                MessageBox.Show(string.Format("{0} 公会创建成功", message.guildInfo.GuildName), "公会");
            }
            else
                MessageBox.Show(message.Errormsg, "公会");
        }

        public void SendGuildJoinRequest(int guildId)
        {
            Debug.Log("SendGuildJoinRequest");
            NetMessage netMessage = new NetMessage();
            netMessage.Request = new NetMessageRequest();
            netMessage.Request.guildJoinReq = new GuildJoinRequest();
            netMessage.Request.guildJoinReq.Apply = new NGuildApplyInfo();
            netMessage.Request.guildJoinReq.Apply.GuildId = guildId;
            NetClient.Instance.SendMessage(netMessage);
        }

        public void SendGuildJoinResponse(bool accept, GuildJoinRequest request)
        {
            Debug.Log("SendGuildJoinResponse");
            NetMessage netMessage = new NetMessage();
            netMessage.Request = new NetMessageRequest();
            netMessage.Request.guildJoinRes = new GuildJoinResponse();
            netMessage.Request.guildJoinRes.Result = Result.Success;
            netMessage.Request.guildJoinRes.Apply = request.Apply;
            netMessage.Request.guildJoinRes.Apply.Result = accept ? ApplyResult.Accept : ApplyResult.Reject;
            NetClient.Instance.SendMessage(netMessage);
        }

        private void OnGuildJoinRequest(object sender, GuildJoinRequest message)
        {
            var messageBox = MessageBox.Show(string.Format("{0} 申请加入公会", message.Apply.Name), "公会申请", MessageBoxType.Confirm, "同意", "拒绝");
            messageBox.OnYes = () => {
                SendGuildJoinResponse(true, message);
            };
            messageBox.OnNo = () =>
            {
                SendGuildJoinResponse(false, message);
            };
        }

        private void OnGuildJoinResponse(object sender, GuildJoinResponse message)
        {
            if (message.Result == Result.Success)
                MessageBox.Show("加入公会成功", "加入公会");
            else
                MessageBox.Show(message.Errormsg, "加入公会");
        }

        public void SendGuildListRequest()
        {
            Debug.Log("SendGuildListRequest");
            NetMessage netMessage = new NetMessage();
            netMessage.Request = new NetMessageRequest();
            netMessage.Request.guildList = new GuildListRequest();
            NetClient.Instance.SendMessage(netMessage);
        }

        private void OnGuildList(object sender, GuildListResponse message)
        {
            if (OnGuildListResult != null)
                OnGuildListResult(message.Guilds);
        }

        private void OnGuild(object sender, GuildResponse message)
        {
            Debug.LogFormat("OnGuild: {0}, {1}, {2}", message.Result, message.guildInfo.Id, message.guildInfo.GuildName);
            GuildManager.Instance.Init(message.guildInfo);
            if (this.OnGuildUpdate != null)
                OnGuildUpdate();
        }

        public void SendGuildLeaveRequest()
        {
            Debug.Log("SendGuildLeaveRequest");
            NetMessage netMessage = new NetMessage();
            netMessage.Request = new NetMessageRequest();
            netMessage.Request.guildLeave = new GuildLeaveRequest();
            NetClient.Instance.SendMessage(netMessage);
        }

        private void OnGuildLeave(object sender, GuildLeaveResponse message)
        {
            if(message.Result == Result.Success)
            {
                GuildManager.Instance.Init(null);
                MessageBox.Show("退出公会成功", "公会").OnYes = () =>
                {
                    UIManager.Instance.Close(typeof(UIGuild));
                };
            }
            else
                MessageBox.Show("退出公会失败", "公会", MessageBoxType.Error);
        }

        public void SendGuildJoinApply(bool accept, NGuildApplyInfo apply)
        {
            Debug.Log("SendGuildJoinApply");
            NetMessage netMessage = new NetMessage();
            netMessage.Request = new NetMessageRequest();
            netMessage.Request.guildJoinRes = new GuildJoinResponse();
            netMessage.Request.guildJoinRes.Result = Result.Success;
            netMessage.Request.guildJoinRes.Apply = apply;
            netMessage.Request.guildJoinRes.Apply.Result = accept ? ApplyResult.Accept : ApplyResult.Reject;
            NetClient.Instance.SendMessage(netMessage);
        }

        internal void SendAdminCommand(GuildAdminCommand command, int characterId)
        {
            Debug.Log("SendAdminCommand");
            NetMessage netMessage = new NetMessage();
            netMessage.Request = new NetMessageRequest();
            netMessage.Request.guildAdmin = new GuildAdminRequest();
            netMessage.Request.guildAdmin.Command = command;
            netMessage.Request.guildAdmin.Target = characterId;
            NetClient.Instance.SendMessage(netMessage);
        }

        private void OnGuildAdmin(object sender, GuildAdminResponse message)
        {
            Debug.LogFormat("OnGuildAdmin: {0}:{1}", message.Command, message.Result);
            MessageBox.Show(string.Format("执行操作：{0}\n结果：{1}：{2}", message.Command, message.Result, message.Errormsg));
        }
    }
}
