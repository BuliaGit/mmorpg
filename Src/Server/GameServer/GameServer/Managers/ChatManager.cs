using System.Collections.Generic;
using Common.Utils;
using Common;
using GameServer.Entities;

using SkillBridge.Message;

namespace GameServer.Managers
{
    class ChatManager : Singleton<ChatManager>
    {
        private Dictionary<int, List<ChatMessage>> _Guild = new Dictionary<int, List<ChatMessage>>(); // 公会
        private Dictionary<int, List<ChatMessage>> _Local = new Dictionary<int, List<ChatMessage>>(); // 本地
        private List<ChatMessage> _System = new List<ChatMessage>(); // 系统
        private Dictionary<int, List<ChatMessage>> _Team = new Dictionary<int, List<ChatMessage>>(); // 队伍
        private List<ChatMessage> _World = new List<ChatMessage>(); // 世界

        public void Init()
        {
        }

        public void AddMessage(Character from, ChatMessage message)
        {
            message.FromId = from.Id;
            message.FromName = from.Name;
            message.Time = TimeUtil.timestamp;

            switch (message.Channel)
            {
                case ChatChannel.ChatChannelLocal:
                    AddLocalMessage(from.Info.mapId, message);
                    break;
                case ChatChannel.ChatChannelWorld:
                    AddWorldMessage(message);
                    break;
                case ChatChannel.ChatChannelSystem:
                    AddSystemMessage(message);
                    break;
                case ChatChannel.ChatChannelTeam:
                    AddTeamMessage(from.Team.Id, message);
                    break;
                case ChatChannel.ChatChannelGuild:
                    AddGuildMessage(from.Guild.Id, message);
                    break;
            }
        }

        private void AddGuildMessage(int guild_id, ChatMessage message)
        {
            if (!_Guild.TryGetValue(guild_id, out var messages))
            {
                messages = new List<ChatMessage>();
                _Guild[guild_id] = messages;
            }

            messages.Add(message);
        }

        private void AddTeamMessage(int team_id, ChatMessage message)
        {
            if (!_Team.TryGetValue(team_id, out var messages))
            {
                messages = new List<ChatMessage>();
                _Team[team_id] = messages;
            }

            messages.Add(message);
        }

        private void AddSystemMessage(ChatMessage message)
        {
            _System.Add(message);
        }

        private void AddWorldMessage(ChatMessage message)
        {
            _World.Add(message);
        }

        private void AddLocalMessage(int map_id, ChatMessage message)
        {
            if (!_Local.TryGetValue(map_id, out var messages))
            {
                messages = new List<ChatMessage>();
                _Local[map_id] = messages;
            }

            messages.Add(message);
        }

        public int GetLocalMessages(int map_id, int index, IList<ChatMessage> result)
        {
            if (!_Local.TryGetValue(map_id, out var messages)) return 0;

            return GetNewMessages(index, result, messages);
        }

        public int GetWorldMessages(int index, IList<ChatMessage> result)
        {
            return GetNewMessages(index, result, _World);
        }

        public int GetSystemMessages(int index, IList<ChatMessage> result)
        {
            return GetNewMessages(index, result, _System);
        }

        public int GetTeamMessages(int team_id, int index, IList<ChatMessage> result)
        {
            if (!_Team.TryGetValue(team_id, out var messages)) return 0;

            return GetNewMessages(index, result, messages);
        }
         
        public int GetGuildMessages(int guild_id, int index, IList<ChatMessage> result)
        {
            if (!_Guild.TryGetValue(guild_id, out var messages)) return 0;

            return GetNewMessages(index, result, messages);
        }

        private int GetNewMessages(int index, ICollection<ChatMessage> result, IList<ChatMessage> messages)
        {
            if (index == 0)
                if (messages.Count > GameDefine.MaxChatRecordNums)
                    index = messages.Count - GameDefine.MaxChatRecordNums;

            for (; index < messages.Count; index += 1) result.Add(messages[index]);

            return index;
        }
    }
}