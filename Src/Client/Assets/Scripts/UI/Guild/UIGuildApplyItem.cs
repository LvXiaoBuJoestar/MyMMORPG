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
        MessageBox.Show(string.Format("ȷ��Ҫͨ��{0}�����������", this.Info.Name), "�������", MessageBoxType.Confirm, "ͬ��", "�ܾ�").OnYes = () =>
        {
            GuildService.Instance.SendGuildJoinApply(true, this.Info);
        };
    }

    public void OnReject()
    {
        MessageBox.Show(string.Format("ȷ��Ҫ�ܾ�{0}�����������", this.Info.Name), "�������", MessageBoxType.Confirm, "ȷ��", "ȡ��").OnYes = () =>
        {
            GuildService.Instance.SendGuildJoinApply(false, this.Info);
        };
    }
}
