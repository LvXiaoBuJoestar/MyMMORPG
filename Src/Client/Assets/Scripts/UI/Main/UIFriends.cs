using Managers;
using Models;
using Services;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFriends : UIWindow
{
    [SerializeField] GameObject itemPrefab;
    [SerializeField] Transform itemRoot;
    [SerializeField] ListView listView;

    UIFriendItem selectedItem;

    private void Start()
    {
        FriendService.Instance.OnFriendUpdate = RefreshUI;
        listView.onItemSelected += OnFriendSelected;
        RefreshUI();
    }

    public void OnFriendSelected(ListView.ListViewItem item)
    {
        selectedItem = item as UIFriendItem;
    }

    public void OnClickFriendTeamInvite()
    {
        if (selectedItem == null)
        {
            MessageBox.Show("����ѡ����Ҫ����ĺ���");
            return;
        }
        if (selectedItem.info.Status == 0)
        {
            MessageBox.Show("������ĺ��ѵ�ǰ������");
            return;
        }
        MessageBox.Show(string.Format("ȷ��Ҫ���롾{0}�����������", selectedItem.info.friendInfo.Name), "�������", MessageBoxType.Confirm, "ȷ��", "ȡ��").OnYes = () =>
        {
            TeamService.Instance.SendTeamInviteRequest(selectedItem.info.friendInfo.Id, selectedItem.info.friendInfo.Name);
        };
    }

    public void OnClickFriendAdd()
    {
        InputBox.Show("������Ҫ��ӵĺ������ƻ�ID��", "��Ӻ���").OnSubmit += OnFriendAddSubmit;
    }

    private bool OnFriendAddSubmit(string inputText, out string tips)
    {
        tips = "";
        int friendId = 0;
        string friendName = "";
        if(!int.TryParse(inputText, out friendId))
            friendName = inputText;
        if(friendId == User.Instance.CurrentCharacter.Id || friendName == User.Instance.CurrentCharacter.Name)
        {
            tips = "����Ц�𣬲�������Լ�Ŷ��";
            return false;
        }
        FriendService.Instance.SendFriendAddRequest(friendId, friendName);
        return true;
    }

    public void OnClickFriendChat()
    {
        MessageBox.Show("������δ����", "" , MessageBoxType.Information);
    }

    public void OnClickFriendRemove()
    {
        if(selectedItem == null)
        {
            MessageBox.Show("��ѡ��Ҫɾ���ĺ���");
            return;
        }
        MessageBox.Show(string.Format("ȷ��Ҫɾ�����ѡ�{0}����", selectedItem.info.friendInfo.Name), "ɾ������", MessageBoxType.Confirm, "ɾ��", "ȡ��").OnYes = () =>
        {
            FriendService.Instance.SendFriendRemoveRequest(selectedItem.info.Id, selectedItem.info.friendInfo.Id);
        };
    }

    void RefreshUI()
    {
        listView.RemoveAll();

        foreach (var friendInfo in FriendManager.Instance.allFriends)
        {
            GameObject go = Instantiate(itemPrefab, itemRoot);
            UIFriendItem friendItem = go.GetComponent<UIFriendItem>();
            friendItem.SetFriendInfo(friendInfo);
            listView.AddItem(friendItem);
        }
    }
}
