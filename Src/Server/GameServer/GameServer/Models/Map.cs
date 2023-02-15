using Common;
using Common.Data;
using GameServer.Battle;
using GameServer.Entities;
using GameServer.Managers;
using GameServer.Services;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;

namespace GameServer.Models
{
    class Map
    {
        internal class MapCharacter
        {
            public NetConnection<NetSession> connection;
            public Character character;

            public MapCharacter(NetConnection<NetSession> conn, Character cha)
            {
                this.connection = conn;
                this.character = cha;
            }
        }

        public int ID
        {
            get { return this.Define.ID; }
        }

        public MapDefine Define;

        /// <summary>
        /// 地图中的角色，以CharacterId为Key
        /// </summary>
        private Dictionary<int, MapCharacter> MapCharacters = new Dictionary<int, MapCharacter>();

        /// <summary>
        /// 刷怪管理器
        /// </summary>
        private SpawnManager SpawnManager = new SpawnManager();

        public MonsterManager MonsterManager = new MonsterManager();

        public Battle.Battle Battle;

        internal Map(MapDefine define)
        {
            this.Define = define;
            this.SpawnManager.Init(this);
            this.MonsterManager.Init(this);
            Battle = new Battle.Battle(this);
        }

        internal void Update()
        {
            SpawnManager.Update();
            Battle.Update();
        }

        #region 进入地图
        /// <summary>
        /// 角色进入地图
        /// </summary>
        /// <param name="character"></param>
        internal void CharacterEnter(NetConnection<NetSession> sender, Character character)
        {
            Log.InfoFormat("CharacterEnter: Map:{0} characterId:{1}", this.Define.ID, character.Id);

            character.Info.mapId = this.ID;
            this.MapCharacters[character.Id] = new MapCharacter(sender, character); //添加自己entityId

            sender.Session.Response.mapCharacterEnter = new MapCharacterEnterResponse();
            sender.Session.Response.mapCharacterEnter.mapId = Define.ID;

            //todo:以后在这里处理AOI,向其他人发送进入地图消息
            foreach (var kv in this.MapCharacters)
            {
                sender.Session.Response.mapCharacterEnter.Characters.Add(kv.Value.character.Info);
                if (kv.Value.character != character)
                {
                    this.SendCharacterEnterMap(kv.Value.connection, character.Info);
                }
            }

            foreach (var kv in this.MonsterManager.Monsters)
            {
                sender.Session.Response.mapCharacterEnter.Characters.Add(kv.Value.Info);
            }

            //向自己发送进入地图消息
            sender.SendResponse();
        }

        /// <summary>
        /// 怪物进入地图
        /// </summary>
        /// <param name="character"></param>
        internal void MonsterEnter(Monster monster)
        {
            Log.InfoFormat("MonsterEnter: Map:{0} monsterId:{1}", this.Define.ID, monster.Id);

            monster.OnEnterMap(this);
            foreach (var kv in this.MapCharacters)
            {
                this.SendCharacterEnterMap(kv.Value.connection, monster.Info);
            }
        }


        void SendCharacterEnterMap(NetConnection<NetSession> conn, NCharacterInfo character)
        {
            if (conn.Session.Response.mapCharacterEnter == null)
            {
                conn.Session.Response.mapCharacterEnter = new MapCharacterEnterResponse();
                conn.Session.Response.mapCharacterEnter.mapId = this.Define.ID;
            }

            conn.Session.Response.mapCharacterEnter.Characters.Add(character);
            conn.SendResponse();    //todo：后面优化时，再考虑
        }
        #endregion

        #region 离开地图
        internal void CharacterLeave(Character cha)
        {
            Log.InfoFormat("CharacterLeave: Map:{0} characterId:{1}", this.Define.ID, cha.Id);

            foreach (var kv in this.MapCharacters)
            {
                this.SendCharacterLeaveMap(kv.Value.connection, cha);
            }
            this.MapCharacters.Remove(cha.Id); //entityId
        }
       

        void SendCharacterLeaveMap(NetConnection<NetSession> conn, Character character)
        {
            Log.InfoFormat("SendCharacterLeaveMap To {0}:{1} : Map:{2} Character:{3}:{4}", conn.Session.Character.Id, conn.Session.Character.Info.Name, this.Define.ID, character.Id, character.Info.Name);

            conn.Session.Response.mapCharacterLeave = new MapCharacterLeaveResponse();
            conn.Session.Response.mapCharacterLeave.entityId = character.entityId;
            conn.SendResponse();
        }
        #endregion

        #region 同步
        public Character curChar;

        /// <summary>
        /// 同步原理
        /// </summary>
        /// <param name="entity"></param>
        internal void UpdateEntity(NEntitySync entity)
        {
            foreach (var kv in this.MapCharacters)
            {
                if (kv.Value.character.entityId == entity.Id)
                {
                    //自己：修改自己位置
                    kv.Value.character.Position = entity.Entity.Position;
                    kv.Value.character.Direction = entity.Entity.Direction;
                    kv.Value.character.Speed = entity.Entity.Speed;
                    //没用的变量
                    curChar = kv.Value.character;
                    //坐骑
                    if (entity.Event == EntityEvent.Ride)
                    {
                        kv.Value.character.Ride = entity.Param;
                    }
                }
                else
                {   
                    //向其他人发送我的位置信息
                    MapService.Instance.SendEntityUpdate(kv.Value.connection, entity);
                }
            }
        }

        internal void BroadCastBattleResponse(NetMessageResponse response)
        {
            foreach (var item in MapCharacters)
            {
                if (response.skillCast != null)
                {
                    item.Value.connection.Session.Response.skillCast = response.skillCast;
                }
                if (response.skillHits != null)
                {
                    item.Value.connection.Session.Response.skillHits = response.skillHits;
                }
                if (response.buffRes != null)
                {
                    item.Value.connection.Session.Response.buffRes = response.buffRes;
                }
                item.Value.connection.SendResponse();
            }
        }
        #endregion
    }
}
