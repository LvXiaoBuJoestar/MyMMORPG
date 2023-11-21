using Services;
using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGuild : UIWindow
{
    [SerializeField] GameObject itemPrefab;
    [SerializeField] ListView listView;
    [SerializeField] Transform itemRoot;
    [SerializeField] UIGuildInfo uiInfo;

    [SerializeField] GameObject[] AdminButtons;
    [SerializeField] GameObject[] LeaderButtons;

    UIGuildMemberItem selectedItem;

    private void Start()
    {
        GuildService.Instance.OnGuildUpdate += UpdateUI;
        listView.onItemSelected += OnGuildMemberSelected;
        UpdateUI();
    }
    private void OnDestroy()
    {
        GuildService.Instance.OnGuildUpdate -= UpdateUI;
    }

    private void OnGuildMemberSelected(ListView.ListViewItem item)
    {
        selectedItem = item as UIGuildMemberItem;
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

        foreach (var go in AdminButtons)
        {
            go.SetActive(GuildManager.Instance.myMemberInfo.Title > GUILDTitle.None);
        }
        foreach (var go in LeaderButtons)
        {
            go.SetActive(GuildManager.Instance.myMemberInfo.Title == GUILDTitle.President);
        }
    }

    public void OnClickAppliesList()
    {
        UIManager.Instance.Show<UIGuildApplyList>();
    }

    public void OnClickLeave()
    {
        Debug.Log("Leave Guild");
        MessageBox.Show("确定要退出公会吗？", "退出公会", MessageBoxType.Confirm, "确定", "取消").OnYes = () =>
        {
            GuildService.Instance.SendGuildLeaveRequest();
        };
    }

    public void OnClickChat()
    {

    }

    public void OnClickKickOut()
    {
        if (selectedItem == null)
        {
            MessageBox.Show("请先选择您要踢出的成员");
            return;
        }
        MessageBox.Show(string.Format("您确定要将{0}踢出公会吗？", selectedItem.Info.Info.Name), "踢出公会", MessageBoxType.Confirm, "确定", "取消").OnYes = () =>
        {
            GuildService.Instance.SendAdminCommand(GuildAdminCommand.Kickout, this.selectedItem.Info.Info.Id);
        };
    }

    public void OnClickPromote()
    {
        if (selectedItem == null)
        {
            MessageBox.Show("请先选择您要晋升的成员");
            return;
        }
        if(selectedItem.Info.Title != GUILDTitle.None)
        {
            MessageBox.Show("对方已经身份尊贵");
            return;
        }
        MessageBox.Show(string.Format("您确定要晋升{0}为副会长吗？", selectedItem.Info.Info.Name), "成员晋升", MessageBoxType.Confirm, "确定", "取消").OnYes = () =>
        {
            GuildService.Instance.SendAdminCommand(GuildAdminCommand.Promote, this.selectedItem.Info.Info.Id);
        };
    }

    public void OnClickDepose()
    {
        if (selectedItem == null)
        {
            MessageBox.Show("请先选择您要罢免的成员");
            return;
        }
        if (selectedItem.Info.Title == GUILDTitle.None)
        {
            MessageBox.Show("对方似乎已经无职可免了");
            return;
        }
        if (selectedItem.Info.Title == GUILDTitle.President)
        {
            MessageBox.Show("会长：你什么身份，我什么地位啊？");
            return;
        }
        MessageBox.Show(string.Format("您确定要罢免{0}的职位吗？", selectedItem.Info.Info.Name), "成员罢免", MessageBoxType.Confirm, "确定", "取消").OnYes = () =>
        {
            GuildService.Instance.SendAdminCommand(GuildAdminCommand.Depost, this.selectedItem.Info.Info.Id);
        };
    }

    public void OnClickTransfer()
    {
        if (selectedItem == null)
        {
            MessageBox.Show("请先选择您要转让会长职位的成员");
            return;
        }
        MessageBox.Show(string.Format("您确定要将将会长转让给{0}吗？", selectedItem.Info.Info.Name), "转让会长", MessageBoxType.Confirm, "确定", "取消").OnYes = () =>
        {
            GuildService.Instance.SendAdminCommand(GuildAdminCommand.Transfer, this.selectedItem.Info.Info.Id);
        };
    }

    public void OnClickSetNotice()
    {
        //////////////
    }
}
