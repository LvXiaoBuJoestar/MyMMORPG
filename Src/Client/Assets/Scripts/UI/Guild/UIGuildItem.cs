using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGuildItem : ListView.ListViewItem
{
    [SerializeField] Text id;
    [SerializeField] Text name;
    [SerializeField] Text leader;
    [SerializeField] Text member;

    [SerializeField] UnityEngine.UI.Image background;
    private Color normalColor;
    private Color selectedColor;

    private void Awake()
    {
        normalColor = background.color;
        selectedColor = background.color;
        selectedColor.a = 1f;
    }

    public override void onSelected(bool selected)
    {
        background.color = selected ? selectedColor : normalColor;
    }

    public NGuildInfo Info;
    internal void SetGuildInfo(NGuildInfo info)
    {
        this.Info = info;
        if (this.name != null) this.name.text = this.Info.GuildName;
        if (this.leader != null) this.leader.text = this.Info.leaderName;
        if (this.id != null) this.id.text = this.Info.Id.ToString();
        if (this.member != null) this.member.text = string.Format("{0} / {1}", info.memberCount, 40);
    }
}
