using Models;
using Network;
using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Services
{
    public class StatusService : Singleton<StatusService>, IDisposable
    {
        public delegate bool StatusNotifyHandler(NStatus status);

        Dictionary<StatusType, StatusNotifyHandler> m_StatusNotifyHandlers = new Dictionary<StatusType, StatusNotifyHandler>();

        public void Init()
        {

        }

        public void RegisterStatusNotify(StatusType function, StatusNotifyHandler action)
        {
            if (!m_StatusNotifyHandlers.ContainsKey(function))
                m_StatusNotifyHandlers[function] = action;
            else
                m_StatusNotifyHandlers[function] += action;
        }

        public StatusService()
        {
            MessageDistributer.Instance.Subscribe<StatusNotify>(this.OnStatusNotify);
        }

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<StatusNotify>(this.OnStatusNotify);
        }


        private void OnStatusNotify(object sender, StatusNotify message)
        {
            foreach (NStatus status in message.Status)
            {
                Notify(status);
            }
        }

        private void Notify(NStatus status)
        {
            if(status.Type == StatusType.Money)
            {
                if (status.Action == StatusAction.Add)
                    User.Instance.AddGold(status.Value);
                else if (status.Action == StatusAction.Delete)
                    User.Instance.AddGold(-status.Value);
            }

            if(m_StatusNotifyHandlers.TryGetValue(status.Type, out StatusNotifyHandler handler))
                handler(status);
        }
    }
}
