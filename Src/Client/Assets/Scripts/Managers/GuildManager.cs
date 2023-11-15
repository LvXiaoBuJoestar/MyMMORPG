using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuildManager : Singleton<GuildManager>
{
    public NGuildInfo guildInfo;

    public bool HasGuild() {  return this.guildInfo != null; }

    public void Init(NGuildInfo guildInfo)
    {
        this.guildInfo = guildInfo;
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
        else
            UIManager.Instance.Show<UIGuildList>();
    }
}
