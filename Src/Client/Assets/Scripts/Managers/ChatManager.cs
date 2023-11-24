using Models;
using Services;
using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Managers
{
    public class ChatManager : Singleton<ChatManager>
    {
        public enum LocalChannel
        {
            All = 0,
            Local,
            World,
            Team,
            Guild,
            Private,
        }

        private ChatChannel[] ChannelFilter = new ChatChannel[6]
        {
            ChatChannel.Local | ChatChannel.World | ChatChannel.Team | ChatChannel.Guild | ChatChannel.Private | ChatChannel.System,
            ChatChannel.Local,
            ChatChannel.World,
            ChatChannel.Team,
            ChatChannel.Guild,
            ChatChannel.Private,
        };

        public List<ChatMessage>[] Messages = new List<ChatMessage>[6]
        {
            new List<ChatMessage>(),
            new List<ChatMessage>(),
            new List<ChatMessage>(),
            new List<ChatMessage>(),
            new List<ChatMessage>(),
            new List<ChatMessage>(),
        };

        public LocalChannel displayChannel;
        public LocalChannel sendChannel;

        public int PrivateId = 0;
        public string PrivateName = "";

        public Action OnChat { get; internal set; }

        public ChatChannel SendChannel
        {
            get
            {
                switch (sendChannel)
                {
                    case LocalChannel.Local: return ChatChannel.Local;
                    case LocalChannel.World: return ChatChannel.World;
                    case LocalChannel.Team: return ChatChannel.Team;
                    case LocalChannel.Guild: return ChatChannel.Guild;
                    case LocalChannel.Private: return ChatChannel.Private;
                }
                return ChatChannel.Local;
            }
        }

        public void Init()
        {
            foreach (var message in Messages)
            {
                message.Clear();
            }
        }

        internal void StartPrivateChat(int targetId, string targetName)
        {
            this.PrivateId = targetId;
            this.PrivateName = targetName;
            this.sendChannel = LocalChannel.Private;
            if (this.OnChat != null) OnChat();
        }

        public void SendChat(string content)
        {
            ChatService.Instance.SendChat(this.SendChannel, content, this.PrivateId, this.PrivateName);
        }

        public bool SetSendChannel(LocalChannel channel)
        {
            if(channel == LocalChannel.Team)
            {
                if(User.Instance.TeamInfo == null)
                {
                    AddSystemMessage("您还没有加入任何队伍");
                    return false;
                }
            }
            if (channel == LocalChannel.Guild)
            {
                if (User.Instance.CurrentCharacter.Guild == null)
                {
                    AddSystemMessage("您还没有加入任何公会");
                    return false;
                }
            }

            this.sendChannel = channel;
            Debug.LogFormat("SetSendChannel: {0}", this.sendChannel);
            return true;
        }

        internal void AddMessages(ChatChannel channel, List<ChatMessage> messages)
        {
            for(int i = 0; i < 6; i++)
            {
                if ((this.ChannelFilter[i] & channel) == channel)
                    this.Messages[i].AddRange(messages);
            }
            if (this.OnChat != null) OnChat();
        }

        public void AddSystemMessage(string message, string from = "")
        {
            this.Messages[(int)LocalChannel.All].Add(new ChatMessage()
            {
                Channel = ChatChannel.System,
                Message = message,
                FromName = from,
            });
            if (this.OnChat != null) this.OnChat();
        }

        public string GetCurrentMessages()
        {
            StringBuilder sb = new StringBuilder();
            foreach(var message in this.Messages[(int)displayChannel])
            {
                sb.AppendLine(FormatMessage(message));
            }
            return sb.ToString();
        }

        private string FormatMessage(ChatMessage message)
        {
            switch (message.Channel)
            {
                case ChatChannel.Local:
                    return string.Format("[本地]{0}{1}", FormatFromPlayer(message), message.Message);
                case ChatChannel.World:
                    return string.Format("<color=cyan>[世界]{0}{1}</color>", FormatFromPlayer(message), message.Message);
                case ChatChannel.System:
                    return string.Format("<color=yellow>[系统]{0}</color>", message.Message);
                case ChatChannel.Private:
                    return string.Format("<color=green>[私聊]{0}{1}</color>", FormatFromPlayer(message), message.Message);
                case ChatChannel.Team:
                    return string.Format("<color=magenta>[队伍]{0}{1}</color>", FormatFromPlayer(message), message.Message);
                case ChatChannel.Guild:
                    return string.Format("<color=blue>[公会]{0}{1}</color>", FormatFromPlayer(message), message.Message);
            }
            return "";
        }

        private string FormatFromPlayer(ChatMessage message)
        {
            if (message.FromId == User.Instance.CurrentCharacter.Id)
                return "<a name=\"\" class=\"player\">[我]</a>";
            else
                return string.Format("<a name=\"c:{0}:{1}\" class=\"player\">[{1}]</a>", message.FromId, message.FromName);
        }
    }
}
