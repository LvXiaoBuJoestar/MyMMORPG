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
            MessageBox.Show("请先选择您要邀请的好友");
            return;
        }
        if (selectedItem.info.Status == 0)
        {
            MessageBox.Show("您邀请的好友当前不在线");
            return;
        }
        MessageBox.Show(string.Format("确定要邀请【{0}】加入队伍吗", selectedItem.info.friendInfo.Name), "组队邀请", MessageBoxType.Confirm, "确定", "取消").OnYes = () =>
        {
            TeamService.Instance.SendTeamInviteRequest(selectedItem.info.friendInfo.Id, selectedItem.info.friendInfo.Name);
        };
    }

    public void OnClickFriendAdd()
    {
        InputBox.Show("请输入要添加的好友名称或ID：", "添加好友").OnSubmit += OnFriendAddSubmit;
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
            tips = "开玩笑吗，不能添加自己哦！";
            return false;
        }
        FriendService.Instance.SendFriendAddRequest(friendId, friendName);
        return true;
    }

    public void OnClickFriendChat()
    {
        MessageBox.Show("功能暂未开放", "" , MessageBoxType.Information);
    }

    public void OnClickFriendRemove()
    {
        if(selectedItem == null)
        {
            MessageBox.Show("请选择要删除的好友");
            return;
        }
        MessageBox.Show(string.Format("确定要删除好友【{0}】吗", selectedItem.info.friendInfo.Name), "删除好友", MessageBoxType.Confirm, "删除", "取消").OnYes = () =>
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
