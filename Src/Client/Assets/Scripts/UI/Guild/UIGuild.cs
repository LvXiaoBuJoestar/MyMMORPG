using Services;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGuild : UIWindow
{
    [SerializeField] GameObject itemPrefab;
    [SerializeField] ListView listView;
    [SerializeField] Transform itemRoot;
    UIGuildInfo uiInfo;
    UIGuildItem selectedItem;

    private void Start()
    {
        GuildService.Instance.OnGuildUpdate = UpdateUI;
        listView.onItemSelected += OnGuildMemberSelected;
        UpdateUI();
    }
    private void OnDestroy()
    {
        GuildService.Instance.OnGuildUpdate = null;
    }

    private void OnGuildMemberSelected(ListView.ListViewItem item)
    {
        selectedItem = item as UIGuildItem;
    }

    private void UpdateUI()
    {
        uiInfo.Info = GuildManager.Instance.guildInfo;
        listView.RemoveAll();

        foreach(var item in GuildManager.Instance.guildInfo.Members)
        {
            GameObject go = Instantiate(itemPrefab, itemRoot);
            UIGuildMemberItem ui = go.GetComponent<UIGuildMemberItem>();
            ui.SetGuildMemberInfo(item);
            listView.AddItem(ui);
        }
    }
}
