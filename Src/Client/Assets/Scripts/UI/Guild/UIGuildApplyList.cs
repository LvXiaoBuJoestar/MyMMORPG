using Services;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGuildApplyList : UIWindow
{
    [SerializeField] GameObject itemPrefab;
    [SerializeField] Transform itemRoot;
    [SerializeField] ListView listView;

    private void Start()
    {
        GuildService.Instance.OnGuildUpdate += UpdateList;
        GuildService.Instance.SendGuildListRequest();
        UpdateList();
    }

    private void OnDestroy()
    {
        GuildService.Instance.OnGuildUpdate -= UpdateList;
    }

    private void UpdateList()
    {
        listView.RemoveAll();

        foreach(var apply in GuildManager.Instance.guildInfo.Applies)
        {
            GameObject go = Instantiate(itemPrefab, itemRoot);
            UIGuildApplyItem ui = go.GetComponent<UIGuildApplyItem>();
            ui.SetItemInfo(apply);
            this.listView.AddItem(ui);
        }
    }
}
