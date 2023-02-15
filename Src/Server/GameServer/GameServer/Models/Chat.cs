using GameServer.Entities;
using GameServer.Managers;
using Server.Managers;
using SkillBridge.Message;

namespace GameServer.Models
{
    class Chat
    {
        private readonly Character _Owner;

        public int GuildIdx;
        public int LocalIdx;
        public int SystemIdx;
        public int TeamIdx;
        public int WorldIdx;

        public Chat(Character owner)
        {
            _Owner = owner;
        }

        public void PostProcess(NetMessageResponse message)
        {
            if (message.Chat == null)
            {
                message.Chat = new ChatResponse();
                message.Chat.Result = Result.Success;
            }

            LocalIdx = ChatManager.Instance.GetLocalMessages(_Owner.Info.mapId, LocalIdx, message.Chat.localMessages);
            WorldIdx = ChatManager.Instance.GetWorldMessages(WorldIdx, message.Chat.worldMessages);
            SystemIdx = ChatManager.Instance.GetSystemMessages(SystemIdx, message.Chat.systemMssages);

            if (_Owner.Team != null)
                TeamIdx = ChatManager.Instance.GetTeamMessages(_Owner.Team.Id, TeamIdx, message.Chat.teamMessages);

            if (_Owner.Guild != null)
                GuildIdx = ChatManager.Instance.GetGuildMessages(_Owner.Guild.Id, GuildIdx, message.Chat.guildMessages);
        }
    }
}