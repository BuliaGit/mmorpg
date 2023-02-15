using Common;
using GameServer.Managers;
using GameServer.Models;
using Network;
using Server.Models;
using SkillBridge.Message;
using System;

namespace GameServer.Entities
{
    /// <summary>
    /// 玩家角色类
    /// </summary>
    class Character : Creature, IPostResponser
    {
        public TCharacter Data;

        public ItemManager ItemManager;
        public StatusManager StatusManager;
        public QuestManager QuestManager;
        public FriendManager FriendManager;

        public Team Team;
        public double TeamUpdateTs;// 时间戳

        public Guild Guild;
        public double GuildUpdateTs;// 时间戳

        public Chat Chat;

        public long Gold
        {
            get { return this.Data.Gold; }
            set
            {
                if (this.Data.Gold == value)
                    return;

                this.StatusManager.AddGoldChange((int)(value - this.Data.Gold));
                this.Data.Gold = value;
                this.Info.Gold = value;
            }
        }
        public int Ride
        {
            get { return Info.Ride; }
            set
            {
                if (Info.Ride == value)
                {
                    return;
                }
                Info.Ride = value;
            }
        }

        public long Exp
        {
            get { return Data.Exp; }
            private set
            {
                if (Data.Exp == value)
                {
                    return;
                }
                StatusManager.AddExpChange((int)(value - Data.Exp));
                Data.Exp = value;
                Info.Exp = value;
            }
        }
        public int Level
        {
            get { return Data.Level; }
            private set
            {
                if(Data.Level == value)
                {
                    return;
                }
                StatusManager.AddLevelUp(value - Data.Level);
                Data.Level = value;
                Info.Level = value;
            }
        }

        /// <summary>
        /// 初始化(在进入游戏时)
        /// </summary>
        /// <param name="type"></param>
        /// <param name="cha"></param>
        public Character(CharacterType type, TCharacter cha) :
            base(type,cha.TID,cha.Level,new Core.Vector3Int(cha.MapPosX, cha.MapPosY, cha.MapPosZ), new Core.Vector3Int(100, 0, 0))
        {
            this.Data = cha;
            this.Id = cha.ID;
            this.Info.Id = cha.ID;
            this.Info.Exp = cha.Exp;
            this.Info.Class = (CharacterClass)cha.Class;
            this.Info.mapId = cha.MapID;
            this.Info.Gold = cha.Gold;
            //坐骑初始id
            Info.Ride = 0;
            this.Info.Name = cha.Name;

            //道具
            this.ItemManager = new ItemManager(this);
            this.ItemManager.GetItemInfos(this.Info.Items);
            //背包
            this.Info.Bag = new NBagInfo
            {
                Unlocked = this.Data.Bag.Unlocked,
                Items = this.Data.Bag.Items     //背包项数量
            };

            this.Info.Equips = this.Data.Equips;

            this.QuestManager = new QuestManager(this);
            this.QuestManager.GetQuestInfos(this.Info.Quests);

            //状态管理
            this.StatusManager = new StatusManager(this);

            this.FriendManager = new FriendManager(this);
            this.FriendManager.GetFriendInfos(this.Info.Friends);

            Guild = GuildManager.Instance.GetGuild(Data.GuildId);

            Chat = new Chat(this);

            Info.attrDynamic = new NAttributeDynamic()
            {
                Hp = cha.HP,
                Mp = cha.MP
            };
        }

        public void AddExp(int exp)
        {
            this.Exp += exp;
            this.CheckLevelUp();
        }

        private void CheckLevelUp()
        {
            //经验公式：EXP = POWER(LV,3) * 10 +LV * 40 + 50;
            long needExp = (long)Math.Pow(this.Level, 3) * 10 + this.Level * 40 + 50;
            if(this.Exp > needExp)
            {
                this.LevelUp();
            }
        }

        private void LevelUp()
        {
            this.Level += 1;
            Log.InfoFormat("Character[{0}:{1}] LevelUp:{2}", Id, Info.Name, Level);
            CheckLevelUp();
        }

        

        //消息后处理
        public void PostProcess(NetMessageResponse message)
        {
            Log.InfoFormat("PostProcess > Character: characterID:{0}:{1}", this.Id, this.Info.Name);
            this.FriendManager.PostProcess(message);

            if (this.Team != null)
            {
                Log.InfoFormat("PostProcess > Team: characterID:{0}:{1}  {2}<{3}", Id, Info.Name, TeamUpdateTs, Team.timestamp);

                if (TeamUpdateTs < Team.timestamp)
                {
                    TeamUpdateTs = Team.timestamp;
                    Team.PostProcess(message);
                }
            }

            if (Guild != null)
            {
                Log.InfoFormat("PostProcess > Guild: characterID:{0}:{1}  {2}<{3}", Id, Info.Name, GuildUpdateTs,
                    Guild.timestamp);

                if (Info.Guild == null)
                {
                    Info.Guild = Guild.GuildInfo(this);
                    if (message.mapCharacterEnter != null)
                        GuildUpdateTs = Guild.timestamp;
                }

                if (GuildUpdateTs < Guild.timestamp && message.mapCharacterEnter == null)
                {
                    GuildUpdateTs = Guild.timestamp;
                    Guild.PostProcess(this, message);
                }
            }


            if (this.StatusManager.HasStatus)
            {
                this.StatusManager.PostProcess(message);
            }
            Chat.PostProcess(message);
        }

        /// <summary>
        /// 角色离开时调用
        /// </summary>
        public void Clear()
        {
            this.FriendManager.OfflineNotify();
        }

        public NCharacterInfo GetBasicInfo()
        {
            return new NCharacterInfo
            {
                Id = Id,
                Name = Info.Name,
                Class = Info.Class,
                Level = Info.Level
            };
        }
    }
}
