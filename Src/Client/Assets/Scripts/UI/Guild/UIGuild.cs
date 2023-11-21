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
        MessageBox.Show("ȷ��Ҫ�˳�������", "�˳�����", MessageBoxType.Confirm, "ȷ��", "ȡ��").OnYes = () =>
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
            MessageBox.Show("����ѡ����Ҫ�߳��ĳ�Ա");
            return;
        }
        MessageBox.Show(string.Format("��ȷ��Ҫ��{0}�߳�������", selectedItem.Info.Info.Name), "�߳�����", MessageBoxType.Confirm, "ȷ��", "ȡ��").OnYes = () =>
        {
            GuildService.Instance.SendAdminCommand(GuildAdminCommand.Kickout, this.selectedItem.Info.Info.Id);
        };
    }

    public void OnClickPromote()
    {
        if (selectedItem == null)
        {
            MessageBox.Show("����ѡ����Ҫ�����ĳ�Ա");
            return;
        }
        if(selectedItem.Info.Title != GUILDTitle.None)
        {
            MessageBox.Show("�Է��Ѿ�������");
            return;
        }
        MessageBox.Show(string.Format("��ȷ��Ҫ����{0}Ϊ���᳤��", selectedItem.Info.Info.Name), "��Ա����", MessageBoxType.Confirm, "ȷ��", "ȡ��").OnYes = () =>
        {
            GuildService.Instance.SendAdminCommand(GuildAdminCommand.Promote, this.selectedItem.Info.Info.Id);
        };
    }

    public void OnClickDepose()
    {
        if (selectedItem == null)
        {
            MessageBox.Show("����ѡ����Ҫ����ĳ�Ա");
            return;
        }
        if (selectedItem.Info.Title == GUILDTitle.None)
        {
            MessageBox.Show("�Է��ƺ��Ѿ���ְ������");
            return;
        }
        if (selectedItem.Info.Title == GUILDTitle.President)
        {
            MessageBox.Show("�᳤����ʲô��ݣ���ʲô��λ����");
            return;
        }
        MessageBox.Show(string.Format("��ȷ��Ҫ����{0}��ְλ��", selectedItem.Info.Info.Name), "��Ա����", MessageBoxType.Confirm, "ȷ��", "ȡ��").OnYes = () =>
        {
            GuildService.Instance.SendAdminCommand(GuildAdminCommand.Depost, this.selectedItem.Info.Info.Id);
        };
    }

    public void OnClickTransfer()
    {
        if (selectedItem == null)
        {
            MessageBox.Show("����ѡ����Ҫת�û᳤ְλ�ĳ�Ա");
            return;
        }
        MessageBox.Show(string.Format("��ȷ��Ҫ�����᳤ת�ø�{0}��", selectedItem.Info.Info.Name), "ת�û᳤", MessageBoxType.Confirm, "ȷ��", "ȡ��").OnYes = () =>
        {
            GuildService.Instance.SendAdminCommand(GuildAdminCommand.Transfer, this.selectedItem.Info.Info.Id);
        };
    }

    public void OnClickSetNotice()
    {
        //////////////
    }
}
