using Services;
using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGuildList : UIWindow
{
    [SerializeField] GameObject itemPrefab;
    [SerializeField] ListView listView;
    [SerializeField] Transform itemRoot;
    [SerializeField] UIGuildInfo uiInfo;
    UIGuildItem selectedItem;

    private void Start()
    {
        listView.onItemSelected += OnGuildItemSelected;
        uiInfo.Info = null;

        GuildService.Instance.OnGuildListResult += UpdateGuildList;
        GuildService.Instance.SendGuildListRequest();
    }

    private void OnDestroy()
    {
        GuildService.Instance.OnGuildListResult -= UpdateGuildList;
    }

    private void UpdateGuildList(List<NGuildInfo> guilds)
    {
        listView.RemoveAll();
        foreach(var guild in guilds)
        {
            GameObject go = Instantiate(itemPrefab, itemRoot);
            UIGuildItem ui = go.GetComponent<UIGuildItem>();
            ui.SetGuildInfo(guild);
            listView.AddItem(ui);
        }
    }

    private void OnGuildItemSelected(ListView.ListViewItem listViewItem)
    {
        this.selectedItem = listViewItem as UIGuildItem;
        this.uiInfo.Info = selectedItem.Info;
    }

    public void OnClickJoin()
    {
        if (selectedItem == null)
        {
            MessageBox.Show("请选择要加入的公会");
            return;
        }
        MessageBox.Show(string.Format("您确定要加入公会【{0}】吗？", selectedItem.Info.GuildName), "加入公会", MessageBoxType.Confirm, "确定", "取消").OnYes = () =>
        {
            GuildService.Instance.SendGuildJoinRequest(selectedItem.Info.Id);
        };
    }
}
