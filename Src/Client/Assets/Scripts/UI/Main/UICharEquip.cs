using Managers;
using Models;
using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICharEquip : UIWindow
{
    [SerializeField] GameObject itemPrefab;
    [SerializeField] GameObject itemEquipedPrefab;

    [SerializeField] Transform itemListRoot;
    [SerializeField] Transform[] slots;

    private void Start()
    {
        RefreshUI();
        EquipManager.Instance.OnEquipChanged += RefreshUI;
    }

    private void OnDestroy()
    {
        EquipManager.Instance.OnEquipChanged -= RefreshUI;
    }

    private void RefreshUI()
    {
        ClearAllEquipList();
        InitAllEquipItems();
        ClearEquipedList();
        InitEquipedItems();
    }

    void InitAllEquipItems()
    {
        foreach(var item in ItemManager.Instance.Items)
        {
            if(item.Value.itemDefine.Type == ItemType.Equip && item.Value.itemDefine.LimitClass == User.Instance.CurrentCharacter.Class)
            {
                if (EquipManager.Instance.Contains(item.Key)) continue;

                GameObject go = Instantiate(itemPrefab, itemListRoot);
                UIEquipItem uIEquipItem = go.GetComponent<UIEquipItem>();
                uIEquipItem.SetEquipItem(item.Key, item.Value, this, false);
            }
        }
    }

    void ClearAllEquipList()
    {
        foreach (var item in itemListRoot.GetComponentsInChildren<UIEquipItem>())
            Destroy(item.gameObject);
    }

    void ClearEquipedList()
    {
        foreach(var item in slots)
        {
            if (item.childCount > 1)
                Destroy(item.GetChild(1).gameObject);
        }
    }

    void InitEquipedItems()
    {
        for (int i = 0; i < (int)EquipSlot.SlotMax; i++)
        {
            var item = EquipManager.Instance.Equips[i];
            if (item != null)
            {
                GameObject go = Instantiate(itemEquipedPrefab, slots[i]);
                go.GetComponent<UIEquipItem>().SetEquipItem(i, item, this, true);
            }
        }
    }

    public void DoEquip(Item item)
    {
        EquipManager.Instance.EquipItem(item);
    }
    public void UnEquip(Item item)
    {
        EquipManager.Instance.UnEquipItem(item);
    }
}
