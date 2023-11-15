using SkillBridge.Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGuildMemberItem : ListView.ListViewItem
{
    [SerializeField] Text name;
    [SerializeField] Text @class;
    [SerializeField] Text level;
    [SerializeField] Text job;
    [SerializeField] Text joinTime;
    [SerializeField] Text status;

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

    public NGuildMemberInfo Info;
    public void SetGuildMemberInfo(NGuildMemberInfo info)
    {
        this.Info = info;
        if(this.name != null) this.name.text = this.Info.Info.Name;
        if(this.@class != null) this.@class.text = this.Info.Info.Class.ToString();
        if(this.level != null) this.level.text = this.Info.Info.Level.ToString();
        if(this.job != null) this.job.text = this.Info.Title.ToString();
        //if(this.joinTime != null) joinTime.text = TimeUnit.GetTime();
    }
}
