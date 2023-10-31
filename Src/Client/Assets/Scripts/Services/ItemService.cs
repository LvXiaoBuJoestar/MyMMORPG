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
    public class ItemService : Singleton<ItemService>, IDisposable
    {
        public ItemService()
        {
            MessageDistributer.Instance.Subscribe<ItemBuyResponse>(this.OnItemBuy);
            MessageDistributer.Instance.Subscribe<ItemEquipResponse>(this.OnItemEquip);
        }

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<ItemBuyResponse>(this.OnItemBuy);
            MessageDistributer.Instance.Unsubscribe<ItemEquipResponse>(this.OnItemEquip);
        }

        public void SendBuyItem(int shopId, int shopItemId)
        {
            NetMessage netMessage = new NetMessage();
            netMessage.Request = new NetMessageRequest();
            netMessage.Request.itemBuy = new ItemBuyRequest();
            netMessage.Request.itemBuy.shopId = shopId;
            netMessage.Request.itemBuy.shopItemId = shopItemId;
            NetClient.Instance.SendMessage(netMessage);
        }

        private void OnItemBuy(object sender, ItemBuyResponse message)
        {
            MessageBox.Show("购买结果：" + message.Result + "\n" + message.Errormsg, "购买完成");
        }

        Item pendingEquip = null;
        bool isEquip;

        public bool SendEquipItem(Item equip, bool isEquip)
        {
            if(pendingEquip != null)
                return false;
            Debug.Log("SendEquipItem");

            pendingEquip = equip;
            this.isEquip = isEquip;

            NetMessage netMessage = new NetMessage();
            netMessage.Request = new NetMessageRequest();
            netMessage.Request.itemEquip = new ItemEquipRequest();
            netMessage.Request.itemEquip.Slot = (int)equip.equipDefine.Slot;
            netMessage.Request.itemEquip.itemId = equip.Id;
            netMessage.Request.itemEquip.isEquip = isEquip;
            NetClient.Instance.SendMessage(netMessage);

            return true;
        }

        private void OnItemEquip(object sender, ItemEquipResponse message)
        {
            if (message.Result == Result.Success)
            {
                if (pendingEquip != null)
                {
                    if (isEquip)
                        EquipManager.Instance.OnEquipItem(pendingEquip);
                    else
                        EquipManager.Instance.OnUnEquipItem(pendingEquip.equipDefine.Slot);
                    pendingEquip = null;
                }
            }
        }
    }
}
