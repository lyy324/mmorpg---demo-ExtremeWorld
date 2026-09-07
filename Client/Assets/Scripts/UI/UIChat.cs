using Candlelight.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using Managers;
using Newtonsoft.Json.Bson;

public class UIChat : MonoBehaviour
{
    public HyperText textArea;
    public TabView channelTab;
    public InputField chatText;
    public Text chatTarget;
    public Dropdown channelSelect;

    private void Start()
    {
        this.channelTab.OnTabSelect += OnDisplayChannelSelected;
        ChatManager.Instance.OnChat += RefreshUI;
    }
    private void OnDestroy()
    {
        //this.channelTab.OnTabSelect -= OnDisplayChannelSelected;
        ChatManager.Instance.OnChat -= RefreshUI;
    }
    private void Update()
    {
        InputManager.Instance.IsInputMode = chatText.isFocused;
    }
    private void OnDisplayChannelSelected(int idx)
    {
        ChatManager.Instance.displayChannel = (ChatManager.LocalChannel)idx;
        RefreshUI();
    }

    private void RefreshUI()
    {
        this.textArea.text = ChatManager.Instance.GetCurrentMessages();
        // SendChannel is a bit-mask enum (Local=1, World=2, Guild=8...).
        // The dropdown uses the sequential LocalChannel values instead.
        this.channelSelect.value = (int)ChatManager.Instance.sendChannel - 1;
        if(ChatManager.Instance.SendChannel == SkillBridge.Message.ChatChannel.Private)
        {
            this.chatTarget.gameObject.SetActive(true);
            if(ChatManager.Instance.PrivateID != 0)
            {
                this.chatTarget.text = ChatManager.Instance.PrivateName + ":";
            }
            else
            {

                this.chatTarget.text = "<无>";
            }
        }
        else
        {
            this.chatTarget.gameObject.SetActive(false);
        }
    }

    public void OnClickChatLink(HyperText text,HyperText.LinkInfo link)
    {
        if(string.IsNullOrEmpty(link.Name))
            return;
        //<a name="c:1001:name" class="Player">Name</a>
        //<a name="i:1001:name" class="Item">Name</a>
        if(link.Name.StartsWith("c:"))
        {
            string[] strs = link.Name.Split(new[] { ':' }, 3);
            int targetId;
            if (strs.Length != 3 || !int.TryParse(strs[1], out targetId) || targetId <= 0)
                return;

            string targetName = Uri.UnescapeDataString(strs[2]);
            UIPopChar menu = UIManager.Instance.Show<UIPopChar>();
            if (menu == null)
                return;
            menu.targetId = targetId;
            menu.targetName = targetName;
        }
    }

    public void OnClickSend()
    {
        OnEndInput(this.chatText.text);
    }

    public void OnEndInput(string text)
    {
        if (!string.IsNullOrEmpty(text.Trim()))
            this.SendChat(text);
        this.chatText.text = "";
    }

    private void SendChat(string content)
    {
        ChatManager.Instance.SendChat(content, ChatManager.Instance.PrivateID, ChatManager.Instance.PrivateName);
    }

    public void OnSendChannelChanged(int idx)
    {
        if(ChatManager.Instance.sendChannel == (ChatManager.LocalChannel)(idx + 1))
            return;
        if (!ChatManager.Instance.SetSendChannel((ChatManager.LocalChannel)(idx + 1)))
        {
            this.channelSelect.value = (int)ChatManager.Instance.sendChannel - 1;
        }
        else
        {
            this.RefreshUI();
        }
    }
}
