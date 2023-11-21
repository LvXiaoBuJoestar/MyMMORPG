using Models;
using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuildManager : Singleton<GuildManager>
{
    public NGuildInfo guildInfo;
    public NGuildMemberInfo myMemberInfo;

    public bool HasGuild() {  return this.guildInfo != null; }

    public void Init(NGuildInfo guild)
    {
        this.guildInfo = guild;

        if (guild == null)
        {
            myMemberInfo = null;
            return;
        }
        foreach (var member in guild.Members)
        {
            if (member.characterId == User.Instance.CurrentCharacter.Id)
            {
                myMemberInfo = member;
                break;
            }
        }
    }

    public void ShowGuild()
    {
        if (this.HasGuild())
            UIManager.Instance.Show<UIGuild>();
        else
        {
            var window = UIManager.Instance.Show<UIGuildPopNoGuild>();
            window.OnClose += PopNoGuild_OnClose;
        }
    }

    private void PopNoGuild_OnClose(UIWindow uIWindowCom, UIWindow.WindowResult result)
    {
        if (result == UIWindow.WindowResult.yes)
            UIManager.Instance.Show<UIGuildPopCreate>();
        else if (result == UIWindow.WindowResult.no)
            UIManager.Instance.Show<UIGuildList>();
    }
}
