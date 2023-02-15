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
    //聊天内容
    public HyperText textArea;
    //频道选择
    public Dropdown channelSelect;
    //私聊目标
    public Text chatTarget;
    //消息输入框
    public InputField chatInputText;

    void Start()
    {
        ChatManager.Instance.OnChat += RefreshUI;
    }

    private void OnDestroy()
    {
        ChatManager.Instance.OnChat -= RefreshUI;
    }

    private void Update()
    {
        InputManager.Instance.isInputMode = chatInputText.isFocused;
    }

    public void OnDisplayChannelSelected(int idx)
    {
        ChatManager.Instance.displayChannel = (ChatManager.LocalChannel)idx;
        RefreshUI();
    }

    private void RefreshUI()
    {
        textArea.text = ChatManager.Instance.GetCurrentMessages();
        channelSelect.value = (int)ChatManager.Instance.LocalSendChannel - 1;
        if (ChatManager.Instance.LocalSendChannel == ChatManager.LocalChannel.Private)
        {
            chatTarget.gameObject.SetActive(true);
            if (ChatManager.Instance.PrivateID != 0)
            {
                chatTarget.text = ChatManager.Instance.PrivateName + ":";
            }
            else
            {
                chatTarget.text = "<无>:";
            }
        }
        else
        {
            chatTarget.gameObject.SetActive(false);
        }
    }

    public void OnClickChatLink(HyperText text, HyperText.LinkInfo link)
    {
        if (string.IsNullOrEmpty(link.Name)) return;
        if (link.Name.StartsWith("c:"))
        {
            string[] strs = link.Name.Split(':');
            UIPopChatMenu menu = UIManagerMe.Instance.Show<UIPopChatMenu>();
            menu.targetId = int.Parse(strs[1]);
            menu.targetName = strs[2];
        }
    }

    public void OnClickSend()
    {
        OnEndInput(chatInputText.text);
    }

    public void OnEndInput(string text)
    {
        if (!string.IsNullOrEmpty(text.Trim()))
        {
            SendChat(text);
        }
        chatInputText.text = string.Empty;
    }

    private void SendChat(string content)
    {
        ChatManager.Instance.SendChat(content, ChatManager.Instance.PrivateID, ChatManager.Instance.PrivateName);
    }


    /// <summary>
    /// 发送频道切换时 
    /// </summary>
    /// <param name="idx"></param>
    public void OnSendChannelChanged(int idx)
    {
        if (ChatManager.Instance.LocalSendChannel == (ChatManager.LocalChannel)(idx + 1)) return;

        if (!ChatManager.Instance.SetSendChannel((ChatManager.LocalChannel)(idx + 1)))
        {
            channelSelect.value = (int)ChatManager.Instance.LocalSendChannel - 1;
        }
        else
        {
            RefreshUI();
        }
    }
}
