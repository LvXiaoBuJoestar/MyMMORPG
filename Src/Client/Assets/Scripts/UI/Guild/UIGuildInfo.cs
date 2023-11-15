using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGuildInfo : MonoBehaviour
{
    [SerializeField] Text name;
    [SerializeField] Text id;
    [SerializeField] Text leader;
    [SerializeField] Text notice;
    [SerializeField] Text member;

    private NGuildInfo info;
    public NGuildInfo Info 
    {
        get { return info; }
        set
        {
            info = value;
            UpdateUI();
        }
    }

    private void UpdateUI()
    {
        if(info == null)
        {
            this.name.text = "��";
            this.id.text = "ID: 00000000";
            this.leader.text = "�᳤����";
            this.notice.text = "";
            this.member.text = "��Ա������0 / 0";
        }
        else
        {
            this.name.text = info.GuildName;
            this.id.text = "ID: " + info.Id;
            this.leader.text = "�᳤��" + info.leaderName;
            this.notice.text = info.Notice;
            this.member.text = string.Format("��Ա������{0} / {1}", info.memberCount, 40);
        }
    }
}
