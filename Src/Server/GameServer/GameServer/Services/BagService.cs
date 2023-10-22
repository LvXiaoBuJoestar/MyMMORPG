using Common;
using GameServer.Entities;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Services
{
    class BagService
    {
        public BagService()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<BagSaveRequest>(this.OnBagSave);
        }

        public void Init()
        {

        }

        private void OnBagSave(NetConnection<NetSession> sender, BagSaveRequest message)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("BagSaveRequest: Character:{0}, Unlocked:{1}", character.Id, message.BagInfo.Unlocked);

            if(message.BagInfo != null)
            {
                character.Data.Bag.Items = message.BagInfo.Items;
                DBService.Instance.Save();
            }
        }
    }
}
