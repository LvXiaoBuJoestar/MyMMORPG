using Managers;
using Models;
using Network;
using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Services
{
    public class FriendService : Singleton<FriendService>, IDisposable
    {
        public UnityAction OnFriendUpdate;

        public void Init()
        {

        }

        public FriendService()
        {
            MessageDistributer.Instance.Subscribe<FriendAddRequest>(OnFriendAddRequest);
            MessageDistributer.Instance.Subscribe<FriendAddResponse>(OnFriendAddResponse);
            MessageDistributer.Instance.Subscribe<FriendListResponse>(OnFriendList);
            MessageDistributer.Instance.Subscribe<FriendRemoveResponse>(OnFriendRemove);
        }

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<FriendAddRequest>(OnFriendAddRequest);
            MessageDistributer.Instance.Unsubscribe<FriendAddResponse>(OnFriendAddResponse);
            MessageDistributer.Instance.Unsubscribe<FriendListResponse>(OnFriendList);
            MessageDistributer.Instance.Unsubscribe<FriendRemoveResponse>(OnFriendRemove);
        }

        public void SendFriendAddRequest(int friendId, string friendName)
        {
            Debug.Log("SendFriendAddRequest");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.friendAddReq = new FriendAddRequest();
            message.Request.friendAddReq.FromId = User.Instance.CurrentCharacter.Id;
            message.Request.friendAddReq.FromName =User.Instance.CurrentCharacter.Name;
            message.Request.friendAddReq.ToId = friendId;
            message.Request.friendAddReq.ToName = friendName;
            NetClient.Instance.SendMessage(message);
        }

        public void SendFriendAddResponse(bool accept, FriendAddRequest request)
        {
            Debug.Log("SendFriendAddResponse");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.friendAddRes = new FriendAddResponse();
            message.Request.friendAddRes.Result = accept ? Result.Success : Result.Failed;
            message.Request.friendAddRes.Errormsg = accept ? "�Է�ͬ�������ĺ�������" : "�Է��ܾ������ĺ�������";
            message.Request.friendAddRes.Request = request;
            NetClient.Instance.SendMessage(message);
        }

        public void SendFriendRemoveRequest(int id, int friendId)
        {
            Debug.Log("SendFriendRemoveRequest");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.friendRemove = new FriendRemoveRequest();
            message.Request.friendRemove.Id = id;
            message.Request.friendRemove.friendId = friendId;
            NetClient.Instance.SendMessage(message);
        }

        private void OnFriendAddRequest(object sender, FriendAddRequest request)
        {
            var confirm = MessageBox.Show(string.Format("{0} ���������Ϊ����", request.FromName), "��������", MessageBoxType.Confirm, "����", "�ܾ�");
            confirm.OnYes = () => { SendFriendAddResponse(true, request); };
            confirm.OnNo = () => { SendFriendAddResponse(false, request); };
        }

        private void OnFriendAddResponse(object sender, FriendAddResponse message)
        {
            if (message.Result == Result.Success)
                MessageBox.Show(message.Request.ToName + "���������ĺ�������", "��Ӻ��ѳɹ�");
            else
                MessageBox.Show(message.Errormsg, "��Ӻ���ʧ��");
        }

        private void OnFriendList(object sender, FriendListResponse message)
        {
            Debug.Log("OnFriendList");
            FriendManager.Instance.Init(message.Friends);
            if (OnFriendUpdate != null)
                OnFriendUpdate();
        }

        private void OnFriendRemove(object sender, FriendRemoveResponse message)
        {
            if (message.Result == Result.Success)
                MessageBox.Show("ɾ���ɹ�", "ɾ������");
            else
                MessageBox.Show("ɾ��ʧ��", "ɾ������", MessageBoxType.Error);
        }
    }
}
