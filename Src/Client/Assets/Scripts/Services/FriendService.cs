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
            message.Request.friendAddRes.Errormsg = accept ? "对方同意了您的好友申请" : "对方拒绝了您的好友申请";
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
            var confirm = MessageBox.Show(string.Format("{0} 请求添加您为好友", request.FromName), "好友请求", MessageBoxType.Confirm, "接受", "拒绝");
            confirm.OnYes = () => { SendFriendAddResponse(true, request); };
            confirm.OnNo = () => { SendFriendAddResponse(false, request); };
        }

        private void OnFriendAddResponse(object sender, FriendAddResponse message)
        {
            if (message.Result == Result.Success)
                MessageBox.Show(message.Request.ToName + "接受了您的好友请求", "添加好友成功");
            else
                MessageBox.Show(message.Errormsg, "添加好友失败");
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
                MessageBox.Show("删除成功", "删除好友");
            else
                MessageBox.Show("删除失败", "删除好友", MessageBoxType.Error);
        }
    }
}
