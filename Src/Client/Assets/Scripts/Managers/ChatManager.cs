using Models;
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

        public List<ChatMessage> Messages = new List<ChatMessage>();

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

        public void Init() { }

        internal void StartPrivateChat(int targetId, string targetName)
        {
            this.PrivateId = targetId;
            this.PrivateName = targetName;
            this.sendChannel = LocalChannel.Private;
            if (this.OnChat != null) OnChat();
        }

        public void SendChat(string content, int toId = 0, string toName = "")
        {
            this.Messages.Add(new ChatMessage()
            {
                Channel = ChatChannel.Local,
                Message = content,
                FromId = User.Instance.CurrentCharacter.Id,
                FromName = User.Instance.CurrentCharacter.Name,
            });
        }

        public bool SetSendChannel(LocalChannel channel)
        {
            if(channel == LocalChannel.Team)
            {
                if(User.Instance.TeamInfo == null)
                {
                    AddSystemMessage("����û�м����κζ���");
                    return false;
                }
            }
            if (channel == LocalChannel.Guild)
            {
                if (User.Instance.CurrentCharacter.Guild == null)
                {
                    AddSystemMessage("����û�м����κι���");
                    return false;
                }
            }

            this.sendChannel = channel;
            Debug.LogFormat("SetSendChannel: {0}", this.sendChannel);
            return true;
        }

        public void AddSystemMessage(string message, string from = "")
        {
            this.Messages.Add(new ChatMessage()
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
            foreach(var message in this.Messages)
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
                    return string.Format("[����]{0}{1}", FormatFromPlayer(message), message.Message);
                case ChatChannel.World:
                    return string.Format("<color=cyan>[����]{0}{1}</color>", FormatFromPlayer(message), message.Message);
                case ChatChannel.System:
                    return string.Format("<color=yellow>[ϵͳ]{0}</color>", message.Message);
                case ChatChannel.Private:
                    return string.Format("<color=green>[˽��]{0}{1}</color>", FormatFromPlayer(message), message.Message);
                case ChatChannel.Team:
                    return string.Format("<color=magenta>[����]{0}{1}</color>", FormatFromPlayer(message), message.Message);
                case ChatChannel.Guild:
                    return string.Format("<color=blue>[����]{0}{1}</color>", FormatFromPlayer(message), message.Message);
            }
            return "";
        }

        private string FormatFromPlayer(ChatMessage message)
        {
            if (message.FromId == User.Instance.CurrentCharacter.Id)
                return "<a name=\"\" class=\"player\">[��]</a>";
            else
                return string.Format("<a name=\"c:{0}:{1}\" class=\"player\">[{1}]</a>", message.FromId, message.FromName);
        }
    }
}
