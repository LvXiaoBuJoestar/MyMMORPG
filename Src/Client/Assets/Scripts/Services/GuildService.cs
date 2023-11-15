using Network;
using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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
        }

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<GuildCreateResponse>(OnGuildCreate);
            MessageDistributer.Instance.Unsubscribe<GuildJoinRequest>(OnGuildJoinRequest);
            MessageDistributer.Instance.Unsubscribe<GuildJoinResponse>(OnGuildJoinResponse);
            MessageDistributer.Instance.Unsubscribe<GuildListResponse>(OnGuildList);
            MessageDistributer.Instance.Unsubscribe<GuildResponse>(OnGuild);
            MessageDistributer.Instance.Unsubscribe<GuildLeaveResponse>(OnGuildLeave);
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
                MessageBox.Show(string.Format("{0} ���ᴴ���ɹ�", message.guildInfo.GuildName), "����");
            }
            else
                MessageBox.Show(string.Format("{0} ���ᴴ��ʧ��", message.guildInfo.GuildName), "����");
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
            var messageBox = MessageBox.Show(string.Format("{0} ������빫��", message.Apply.Name), "��������", MessageBoxType.Confirm, "ͬ��", "�ܾ�");
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
                MessageBox.Show("���빫��ɹ�", "����");
            else
                MessageBox.Show("���빫��ʧ��", "����");
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
                MessageBox.Show("�˳�����ɹ�", "����");
            }
            else
                MessageBox.Show("�˳�����ʧ��", "����", MessageBoxType.Error);
        }
    }
}
