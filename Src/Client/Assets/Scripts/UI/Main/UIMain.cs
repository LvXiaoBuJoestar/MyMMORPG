using Models;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMain : MonoSingleton<UIMain>
{
    [SerializeField] Text avaterName;
    [SerializeField] Text avatarLevel;

    [SerializeField] UITeam TeamWindow;

    protected override void OnStart()
    {
        UpdateAvatar();
    }

    private void UpdateAvatar()
    {
        avaterName.text = User.Instance.CurrentCharacter.Name;
        avatarLevel.text = User.Instance.CurrentCharacter.Level.ToString();
    }

    public void OnClickSetting()
    {
        UIManager.Instance.Show<UISetting>();
    }

    public void OnClickBag()
    {
        UIManager.Instance.Show<UIBag>();
    }
    public void OnClickEquip()
    {
        UIManager.Instance.Show<UICharEquip>();
    }
    public void OnClickQuest()
    {
        UIManager.Instance.Show<UIQuestSystem>();
    }
    public void OnClickFriend()
    {
        UIManager.Instance.Show<UIFriends>();
    }
    public void OnClickGuild()
    {
        GuildManager.Instance.ShowGuild();
    }
    public void ShowTeamUI(bool show)
    {
        TeamWindow.ShowTeam(show);
    }
}
