using Common.Data;
using Managers;
using Models;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIShop : UIWindow
{
    [SerializeField] Text title;
    [SerializeField] Text money;

    [SerializeField] GameObject shopItem;
    [SerializeField] Transform itemRoot;

    ShopDefine shopDefine;
    UIShopItem selectedShopItem;

    private void Start()
    {
        StartCoroutine(nameof(InitShop));
    }

    IEnumerator InitShop()
    {
        foreach(var kv in DataManager.Instance.ShopItems[shopDefine.ID])
        {
            if (kv.Value.Status > 0)
            {
                GameObject go = Instantiate(shopItem, itemRoot);
                UIShopItem uIShopItem = go.GetComponent<UIShopItem>();
                uIShopItem.SetShopItem(kv.Key, kv.Value, this);
            }
        }
        yield return null;
    }

    public void SetShop(ShopDefine shopDefine)
    {
        this.shopDefine = shopDefine;
        this.title.text = shopDefine.Name;
        this.money.text = User.Instance.CurrentCharacter.Gold.ToString();
    }

    internal void SelectShopItem(UIShopItem uIShopItem)
    {
        if (selectedShopItem != null)
            selectedShopItem.IsSelected = false;
        selectedShopItem = uIShopItem;
    }

    public void OnClickBuy()
    {
        if (selectedShopItem == null)
        {
            MessageBox.Show("请先选择要购买的道具", "购买提醒");
            return;
        }
        ShopManager.Instance.BuyItem(shopDefine.ID, selectedShopItem.ShopItemId);
    }
}
