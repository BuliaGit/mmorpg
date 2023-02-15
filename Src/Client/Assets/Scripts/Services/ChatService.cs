using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Services
{
    class ChatService : Singleton<ChatService>, IDisposable
    {
        public ChatService()
        {
            MessageDistributer.Instance.Subscribe<ChatResponse>(OnChat);
        }

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<ChatResponse>(OnChat);
        }

        public void Init()
        {
        }

        public void SendChat(ChatChannel channel, string content, int toId, string toName)
        {
            Debug.Log("SendChat");

            var message = new NetMessage
            {
                Request = new NetMessageRequest
                {
                    Chat = new ChatRequest
                    {
                        Message = new ChatMessage
                        {
                            Channel = channel,
                            ToId = toId,
                            ToName = toName,
                            Message = content
                        }
                    }
                }
            };

            NetClient.Instance.SendMessage(message);
        }

        private void OnChat(object sender, ChatResponse message)
        {
            if (message.Result == Result.Success)
            {
                ChatManager.Instance.AddMessages(ChatChannel.ChatChannelLocal, message.localMessages);
                ChatManager.Instance.AddMessages(ChatChannel.ChatChannelWorld, message.worldMessages);
                ChatManager.Instance.AddMessages(ChatChannel.ChatChannelSystem, message.systemMssages);
                ChatManager.Instance.AddMessages(ChatChannel.ChatChannelPrivate, message.privateMessages);
                ChatManager.Instance.AddMessages(ChatChannel.ChatChannelTeam, message.teamMessages);
                ChatManager.Instance.AddMessages(ChatChannel.ChatChannelGuild, message.guildMessages);
            }
            else
            {
                ChatManager.Instance.AddSystemMessage(message.Errormsg);
            }
        }
    }
}
