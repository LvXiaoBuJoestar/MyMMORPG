using Managers;
using Models;
using Network;
using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Services
{
    public class QuestService : Singleton<QuestService>, IDisposable
    {
        public QuestService()
        {
            MessageDistributer.Instance.Subscribe<QuestAcceptResponse>(OnQuestAccept);
            MessageDistributer.Instance.Subscribe<QuestSubmitResponse>(OnQuestSubmit);
        }

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<QuestAcceptResponse>(OnQuestAccept);
            MessageDistributer.Instance.Unsubscribe<QuestSubmitResponse>(OnQuestSubmit);
        }

        public bool SendQuestAccept(Quest quest)
        {
            Debug.Log("SendQuestAccept");
            NetMessage netMessage = new NetMessage();
            netMessage.Request = new NetMessageRequest();
            netMessage.Request.questAccept = new QuestAcceptRequest();
            netMessage.Request.questAccept.QuestId = quest.Define.ID;
            NetClient.Instance.SendMessage(netMessage);
            return true;
        }

        private void OnQuestAccept(object sender, QuestAcceptResponse message)
        {
            Debug.LogFormat("OnQuestAccept:{0},Error:{1}", message.Result, message.Errormsg);
            if (message.Result == Result.Success)
                QuestManager.Instance.OnQuestAccept(message.Quest);
            else
                MessageBox.Show("任务接受失败", "错误", MessageBoxType.Error);
        }

        public bool SendQuestSubmit(Quest quest)
        {
            Debug.Log("SendQuestSubmit");
            NetMessage netMessage = new NetMessage();
            netMessage.Request = new NetMessageRequest();
            netMessage.Request.questSubmit = new QuestSubmitRequest();
            netMessage.Request.questSubmit.QuestId = quest.Define.ID;
            NetClient.Instance.SendMessage(netMessage);
            return true;
        }

        private void OnQuestSubmit(object sender, QuestSubmitResponse message)
        {
            Debug.LogFormat("OnQuestSubmit:{0},Error:{1}", message.Result, message.Errormsg);
            if (message.Result == Result.Success)
                QuestManager.Instance.OnQuestSubmited(message.Quest);
            else
                MessageBox.Show("任务完成失败", "错误", MessageBoxType.Error);
        }
    }
}
