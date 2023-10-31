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

    [SerializeField] int perPageCount = 10;
    [SerializeField] Text currentPageText;
    int currentPage = 1;
    int pageCount = 1;

    private void Start()
    {
        StartCoroutine(nameof(InitShop));
        SetPageCount();
    }

    public void SwitchLastPage()
    {
        if (currentPage <= 1) return;
        currentPage--;
        StartCoroutine(nameof(InitShop));
    }
    public void SwitchNextPage()
    {
        if (currentPage >= pageCount) return;
        currentPage++;
        StartCoroutine(nameof(InitShop));
    }

    IEnumerator InitShop()
    {
        if (itemRoot.childCount > 0)
        {
            for (int i = 0; i < itemRoot.childCount; i++)
            {
                Destroy(itemRoot.GetChild(i).gameObject);
            }
        }

        int index = 0;
        int page = 1;
        foreach(var kv in DataManager.Instance.ShopItems[shopDefine.ID])
        {
            index++;
            if(index > perPageCount)
            {
                index = 1;
                page++;
            }

            if (page == currentPage && kv.Value.Status > 0)
            {
                GameObject go = Instantiate(shopItem, itemRoot);
                UIShopItem uIShopItem = go.GetComponent<UIShopItem>();
                uIShopItem.SetShopItem(kv.Key, kv.Value, this);
            }
        }
        currentPageText.text = currentPage + "/" + pageCount;
        yield return null;
    }

    void SetPageCount()
    {
        pageCount = DataManager.Instance.ShopItems[shopDefine.ID].Count / perPageCount;
        if (DataManager.Instance.ShopItems.Count % perPageCount > 0)
            pageCount++;
        currentPageText.text = currentPage + "/" + pageCount;
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
            MessageBox.Show("����ѡ��Ҫ����ĵ���", "��������");
            return;
        }
        ShopManager.Instance.BuyItem(shopDefine.ID, selectedShopItem.ShopItemId);
    }
}
