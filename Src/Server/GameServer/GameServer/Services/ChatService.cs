using Common;
using GameServer.Managers;
using Network;
using Server.Managers;

using SkillBridge.Message;

namespace GameServer.Services
{

    public class ChatService : Singleton<ChatService>
    {
        public ChatService()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<ChatRequest>(OnChat);
        }

        public void Init()
        {
            ChatManager.Instance.Init();
        }

        private void OnChat(NetConnection<NetSession> sender, ChatRequest request)
        {
            var character = sender.Session.Character;

            Log.Info(
                $"OnChat::character:{character.Id}:Channel:{request.Message.Channel} Message:{request.Message.Message}");

            if (request.Message.Channel == ChatChannel.ChatChannelPrivate)
            {
                var chatTo = SessionManager.Instance.GetSession(request.Message.ToId);
                if (chatTo == null)
                {
                    sender.Session.Response.Chat = new ChatResponse
                    {
                        Result = Result.Failed,
                        Errormsg = "对方不在线"
                    };
                    sender.Session.Response.Chat.privateMessages.Add(request.Message);
                    sender.SendResponse();
                }
                else
                {
                    if (chatTo.Session.Response.Chat == null) chatTo.Session.Response.Chat = new ChatResponse();

                    request.Message.FromId = character.Id;
                    request.Message.FromName = character.Name;
                    chatTo.Session.Response.Chat.Result = Result.Success;
                    chatTo.Session.Response.Chat.privateMessages.Add(request.Message);
                    chatTo.SendResponse();

                    // TODO 自己的消息不需要等待响应
                    if (sender.Session.Response.Chat == null) sender.Session.Response.Chat = new ChatResponse();

                    sender.Session.Response.Chat.Result = Result.Success;
                    sender.Session.Response.Chat.privateMessages.Add(request.Message);
                    sender.SendResponse();
                }
            }
            else
            {
                sender.Session.Response.Chat = new ChatResponse
                {
                    Result = Result.Success
                };
                ChatManager.Instance.AddMessage(character, request.Message);
                sender.SendResponse();
            }
        }
    }
}