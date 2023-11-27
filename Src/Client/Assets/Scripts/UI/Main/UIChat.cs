using Candlelight.UI;
using Managers;
using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIChat : MonoBehaviour
{
    [SerializeField] HyperText textArea;
    [SerializeField] TabView channelTab;

    [SerializeField] InputField chatText;
    [SerializeField] GameObject target;
    [SerializeField] Text chatTarget;

    [SerializeField] Dropdown channelSelect;

    private void Start()
    {
        this.channelTab.OnTabSelect += OnDisplayChannelSelected;
        ChatManager.Instance.OnChat += RefreshUI;
        channelSelect.onValueChanged.AddListener(OnSendChannelChanged);      
    }

    private void OnDestroy()
    {
        ChatManager.Instance.OnChat -= RefreshUI;
    }

    private void Update()
    {
        InputManager.Instance.isInputMode = chatText.isFocused;
    }

    void OnDisplayChannelSelected(int index)
    {
        ChatManager.Instance.displayChannel = (ChatManager.LocalChannel)index;
        RefreshUI();
    }

    private void RefreshUI()
    {
        this.textArea.text = ChatManager.Instance.GetCurrentMessages();
        this.channelSelect.value = (int)ChatManager.Instance.sendChannel - 1;

        if(ChatManager.Instance.SendChannel == ChatChannel.Private)
        {
            target.SetActive(true);
            if (ChatManager.Instance.PrivateId != 0)
                chatTarget.text = ChatManager.Instance.PrivateName + ":";
            else
                chatTarget.text = "<нч>:";
        }
        else
            target.SetActive(false);
    }

    public void OnClickChatLink(HyperText text, HyperText.LinkInfo link)
    {
        if (string.IsNullOrEmpty(link.Name)) return;

        if (link.Name.StartsWith("c:"))
        {
            string[] strings = link.Name.Split(':');
            UIPopChatMenu menu = UIManager.Instance.Show<UIPopChatMenu>();
            menu.targetId = int.Parse(strings[1]);
            menu.targetName = strings[2];
        }
    }

    public void OnClickSend()
    {
        OnEndInput(this.chatText.text);
    }

    void OnEndInput(string text)
    {
        if (!string.IsNullOrEmpty(text.Trim()))
            SendChat(text);
        this.chatText.text = "";
    }

    void SendChat(string text)
    {
        ChatManager.Instance.SendChat(text);
    }

    public void OnSendChannelChanged(int index)
    {
        if (ChatManager.Instance.sendChannel == (ChatManager.LocalChannel)index + 1) return;

        if (!ChatManager.Instance.SetSendChannel((ChatManager.LocalChannel)index + 1))
            this.channelSelect.value = (int)ChatManager.Instance.sendChannel - 1;
        else
            RefreshUI();
    }
}
