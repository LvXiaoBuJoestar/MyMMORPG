using Managers;
using Models;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRide : UIWindow
{
    [SerializeField] GameObject itemPrefab;
    [SerializeField] Transform itemRoot;
    [SerializeField] ListView listView;
    [SerializeField] Text description;

    private UIRideItem selectedItem;

    private void OnEnable()
    {
        RefreshUI();
    }

    private void Start()
    {
        this.listView.onItemSelected += this.OnItemSelected;
    }

    private void OnItemSelected(ListView.ListViewItem item)
    {
        this.selectedItem = item as UIRideItem;
        this.description.text = this.selectedItem.item.itemDefine.Description;
    }

    private void RefreshUI()
    {
        this.listView.RemoveAll();

        foreach(var kv in ItemManager.Instance.Items)
        {
            if(kv.Value.itemDefine.Type == SkillBridge.Message.ItemType.Ride && 
                (kv.Value.itemDefine.LimitClass == SkillBridge.Message.CharacterClass.None || kv.Value.itemDefine.LimitClass == User.Instance.CurrentCharacter.Class)){
                GameObject go = Instantiate(itemPrefab, itemRoot);
                UIRideItem ui = go.GetComponent<UIRideItem>();
                ui.SetRideItem(kv.Value, this, false);
                this.listView.AddItem(ui);
            }
        }
    }

    public void DoRide()
    {
        if(this.selectedItem == null)
        {
            MessageBox.Show("请选择你要骑乘的坐骑", "提示");
            return;
        }
        User.Instance.Ride(this.selectedItem.item.Id);
    }
}
