using Services;
using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGuildApplyItem : ListView.ListViewItem
{
    [SerializeField] Text name;
    [SerializeField] Text @class;
    [SerializeField] Text level;

    public NGuildApplyInfo Info;

    internal void SetItemInfo(NGuildApplyInfo info)
    {
        this.Info = info;
        if (this.name != null) this.name.text = this.Info.Name;
        if (this.@class != null) this.@class.text = this.Info.Class.ToString();
        if (this.level != null) this.level.text = this.Info.Level.ToString();
    }

    public void OnAccept()
    {
        MessageBox.Show(string.Format("确定要通过{0}的入会申请吗", this.Info.Name), "入会申请", MessageBoxType.Confirm, "同意", "拒绝").OnYes = () =>
        {
            GuildService.Instance.SendGuildJoinApply(true, this.Info);
        };
    }

    public void OnReject()
    {
        MessageBox.Show(string.Format("确定要拒绝{0}的入会申请吗", this.Info.Name), "入会申请", MessageBoxType.Confirm, "确定", "取消").OnYes = () =>
        {
            GuildService.Instance.SendGuildJoinApply(false, this.Info);
        };
    }
}
