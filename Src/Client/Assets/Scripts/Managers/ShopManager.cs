using Common.Data;
using Services;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public class ShopManager : Singleton<ShopManager>
    {
        public void Init()
        {
            NpcManager.Instance.RegisterNpcEvent(NpcFunction.InvokeShop, OnOpenShop);
        }

        private bool OnOpenShop(NpcDefine npcDefine)
        {
            ShowShop(npcDefine.Param);
            return true;
        }

        private void ShowShop(int shopId)
        {
            if(DataManager.Instance.Shops.TryGetValue(shopId, out ShopDefine shop))
            {
                UIShop uIShop = UIManager.Instance.Show<UIShop>();
                if (uIShop != null)
                    uIShop.SetShop(shop);
            }
        }

        public bool BuyItem(int shopId, int shopItemId)
        {
            ItemService.Instance.SendBuyItem(shopId, shopItemId);
            return true;
        }
    }
}
