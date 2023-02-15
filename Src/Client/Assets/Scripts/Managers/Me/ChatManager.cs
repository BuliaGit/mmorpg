using Entities;
using Models;
using Services;
using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

public class ChatManager : Singleton<ChatManager>
{
    public event Action OnChat;
    public LocalChannel displayChannel;
    public LocalChannel LocalSendChannel;

    public ChatChannel SendChannel
    {
        get
        {
            switch (LocalSendChannel)
            {
                case LocalChannel.Local:return ChatChannel.ChatChannelLocal;
                case LocalChannel.World:return ChatChannel.ChatChannelWorld;
                case LocalChannel.Team:return ChatChannel.ChatChannelTeam;
                case LocalChannel.Guild:return ChatChannel.ChatChannelGuild;
                case LocalChannel.Private:return ChatChannel.ChatChannelPrivate;
            }
            return ChatChannel.ChatChannelLocal;
        }
    }


    public int PrivateID = 0;
    public string PrivateName = "";

    private readonly ChatChannel[] _channel_filter =
        {
            ChatChannel.ChatChannelLocal | ChatChannel.ChatChannelWorld | ChatChannel.ChatChannelGuild | ChatChannel.ChatChannelTeam | ChatChannel.ChatChannelPrivate |
            ChatChannel.ChatChannelSystem,
            ChatChannel.ChatChannelLocal,
            ChatChannel.ChatChannelWorld,
            ChatChannel.ChatChannelTeam,
            ChatChannel.ChatChannelGuild,
            ChatChannel.ChatChannelPrivate
        };
    public List<ChatMessage>[] chatMessages = new List<ChatMessage>[6]
    {
        new List<ChatMessage>(),
        new List<ChatMessage>(),
        new List<ChatMessage>(),
        new List<ChatMessage>(),
        new List<ChatMessage>(),
        new List<ChatMessage>()
    };

    public Dictionary<int, Creature> characters = new Dictionary<int, Creature>();
    public enum LocalChannel
    {
        All = 0,    //所有
        Local = 1,  //本地
        World = 2,  //世界
        Team = 3,   //队伍
        Guild = 4,  //公会
        Private = 5,    //私聊
    }

    internal void Init()
    {
        foreach (var msg in chatMessages)
        {
            msg.Clear();
        }
        if (OnChat != null)
        {
            OnChat();
        }
    }

    internal void SendChat(string content, int to_id = 0, string to_name = "")
    {
        ChatService.Instance.SendChat(SendChannel, content, to_id, to_name);
    }

    /// <summary>
    /// 设置发送频道
    /// </summary>
    /// <param name="channel"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public bool SetSendChannel(LocalChannel channel)
    {
        if (channel == LocalChannel.Team)
        {
            if (User.Instance.TeamInfo == null)
            {
                AddSystemMessage("你没有加入任何队伍");
                return false;
            }
        }
        if (channel == LocalChannel.Guild)
        {
            if (User.Instance.CurrentCharacterInfo.Guild == null)
            {
                AddSystemMessage("你没有加入任何公会");
                return false;
            }
        }
        LocalSendChannel = channel;
        Debug.LogFormat("SetSendChannel:{0}", LocalSendChannel);
        return true;
    }

    public void AddMessages(ChatChannel channel, List<ChatMessage> messages)
    {
        for (int ch = 0; ch < 6; ch++)
        {
            if ((_channel_filter[ch] & channel) == channel)
            {
                chatMessages[ch].AddRange(messages);
            }
        }
        if (OnChat != null)
        {
            OnChat();
        }
    }

    public void AddSystemMessage(string msg, string from = "")
    {
        chatMessages[(int)LocalChannel.All].Add(new ChatMessage()
        {
            Channel = ChatChannel.ChatChannelSystem,
            Message = msg,
            FromName = from
        });
        if (OnChat != null)
        {
            OnChat();
        }
    }
    internal string GetCurrentMessages()
    {
        StringBuilder sb = new StringBuilder();
        foreach (var message in chatMessages[(int)displayChannel])
        {
            sb.AppendLine(FormatMessage(message));
        }
        return sb.ToString();
    }

    private string FormatMessage(ChatMessage message)
    {
        switch (message.Channel)
        {
            case ChatChannel.ChatChannelLocal:
                return string.Format("[本地]{0}{1}", FormatFromPlayer(message), message.Message);
            case ChatChannel.ChatChannelWorld:
                return string.Format("<color=cyan>[世界]{0}{1}</color>", FormatFromPlayer(message), message.Message);
            case ChatChannel.ChatChannelSystem:
                return string.Format("<color=yellow>[系统]{0}</color>", message.Message);
            case ChatChannel.ChatChannelPrivate:
                return string.Format("<color=magenta>[私聊]{0}{1}</color>", FormatFromPlayer(message), message.Message);
            case ChatChannel.ChatChannelTeam:
                return string.Format("<color=green>[队伍]{0}{1}</color>", FormatFromPlayer(message), message.Message);
            case ChatChannel.ChatChannelGuild:
                return string.Format("<color=blue>[公会]{0}{1}</color>", FormatFromPlayer(message), message.Message);
        }
        return "";
    }

    private string FormatFromPlayer(ChatMessage message)
    {
        if (message.FromId == User.Instance.CurrentCharacterInfo.Id)
        {
            return "<a name=\"\" class=\"player\">[我]</a>";
        }
        else
        {
            return String.Format("<a name=\"c:{0}:{1}\" class=\"player\">[{1}]</a>", message.FromId, message.FromName);
        }
    }

    internal void StartPrivateChat(int targetId, string targetName)
    {
        PrivateID = targetId;
        PrivateName = targetName;
        LocalSendChannel = LocalChannel.Private;
        if (OnChat != null)
        {
            OnChat();
        }
    }
    public Creature GetCharacter(int id)
    {
        Creature character;
        characters.TryGetValue(id, out character);
        return character;
    }


}
