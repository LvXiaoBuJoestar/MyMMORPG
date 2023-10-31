using Common.Data;
using Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBag : UIWindow
{
    [SerializeField] Text moneyText;
    [SerializeField] Transform[] pages;
    [SerializeField] GameObject bagItem;

    List<Image> slots;

    private void Start()
    {
        slots = new List<Image>();
        for(int i = 0; i < pages.Length; i++)
        {
            slots.AddRange(pages[i].GetComponentsInChildren<Image>(true));
        }
        StartCoroutine(nameof(InitBag));
    }

    IEnumerator InitBag()
    {
        for(int i = 0; i < BagManager.Instance.Items.Length; i++)
        {
            var item = BagManager.Instance.Items[i];
            if(item.ItemId > 0)
            {
                GameObject go = Instantiate(bagItem, slots[i].transform);
                UIIconItem uIIconItem = go.GetComponent<UIIconItem>();
                ItemDefine itemDefine = ItemManager.Instance.Items[item.ItemId].itemDefine;
                uIIconItem.SetMainIcon(itemDefine.Icon, item.Count.ToString());
            }
        }
        for(int i = BagManager.Instance.Items.Length; i < slots.Count; i++)
        {
            slots[i].color = Color.gray;
        }
        yield return null;
    }

    public void OnReset()
    {
        BagManager.Instance.Reset();
        Clear();
        StartCoroutine(nameof(InitBag));
    }

    void Clear()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].transform.childCount > 0)
                Destroy(slots[i].transform.GetChild(0).gameObject);
        }
    }
}
