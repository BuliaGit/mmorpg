using System;
using Network;
using UnityEngine;

using Common.Data;
using SkillBridge.Message;
using Models;
using Managers;
using Assets.Scripts.Entities;

namespace Services
{
    class MapService : Singleton<MapService>, IDisposable
    {
        public int CurrentMapId;

        public MapService()
        {
            MessageDistributer.Instance.Subscribe<MapCharacterEnterResponse>(this.OnMapCharacterEnter);
            MessageDistributer.Instance.Subscribe<MapCharacterLeaveResponse>(this.OnMapCharacterLeave);

            MessageDistributer.Instance.Subscribe<MapEntitySyncResponse>(this.OnMapEntitySync);

            SceneManager.Instance.onSceneLoadDone += OnLoadDone;
        }

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<MapCharacterEnterResponse>(this.OnMapCharacterEnter);
            MessageDistributer.Instance.Unsubscribe<MapCharacterLeaveResponse>(this.OnMapCharacterLeave);

            MessageDistributer.Instance.Unsubscribe<MapEntitySyncResponse>(this.OnMapEntitySync);

            SceneManager.Instance.onSceneLoadDone += OnLoadDone;
        }

        public void Init()
        {

        }

        private void OnLoadDone()//-----
        {
            //GameObjectManager.Instance.Characters
        }

        #region 进入地图
        private void OnMapCharacterEnter(object sender, MapCharacterEnterResponse response)
        {
            Debug.LogFormat("OnMapCharacterEnter:Map:{0} Count:{1}", response.mapId, response.Characters.Count);
           
            foreach (var cha in response.Characters)
            {
                if (User.Instance.CurrentCharacterInfo == null || (cha.Type == CharacterType.Player && User.Instance.CurrentCharacterInfo.Id == cha.Id))//防止进入地图过程中，服务器发放奖励拿不到，再次确认角色信息(因为服务器先发了进入游戏消息，才发进入地图消息)
                {
                    User.Instance.CurrentCharacterInfo = cha;
                    if(User.Instance.CurrentCharacter == null)
                    {
                        User.Instance.CurrentCharacter = new Character(cha);
                    }
                    else
                    {
                        User.Instance.CurrentCharacter.UpdateInfo(cha);
                    }
                    User.Instance.CharacterInited();
                    CharacterManager.Instance.AddCharacter((Character)User.Instance.CurrentCharacter);
                    continue;
                }

                CharacterManager.Instance.AddCharacter(new Character(cha));
            }

            //当前角色切换新地图
            if (CurrentMapId != response.mapId)
            {
                this.EnterMap(response.mapId);
                this.CurrentMapId = response.mapId;
            }
        }

        public void EnterMap(int mapId)
        {
            if (DataManager.Instance.Maps.ContainsKey(mapId))
            {
                MapDefine map = DataManager.Instance.Maps[mapId];
                User.Instance.CurrentMapData = map;
                SceneManager.Instance.LoadScene(map.Resource);
                SoundManager.Instance.PlayMusic(map.Music);
            }
            else
                Debug.LogErrorFormat("EnterMap: Map {0} not existed", mapId);
        }
        #endregion

        #region 离开地图
        private void OnMapCharacterLeave(object sender, MapCharacterLeaveResponse response)
        {
            Debug.LogFormat("OnMapCharacterLeave: CharID:{0}", response.entityId);

            if (response.entityId != User.Instance.CurrentCharacterInfo.EntityId)
                CharacterManager.Instance.RemoveCharacter(response.entityId);
            else
                CharacterManager.Instance.Clear();
        }
        #endregion

        #region 移动同步
        public void SendMapEntitySync(EntityEvent entityEvent, NEntity entity,int param)
        {
            //Debug.LogFormat("MapEntityUpdateRequest :ID:{0} POS:{1} DIR:{2} SPD:{3} ", entity.Id, entity.Position.String(), entity.Direction.String(), entity.Speed);

            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.mapEntitySync = new MapEntitySyncRequest();
            message.Request.mapEntitySync.entitySync = new NEntitySync()
            {
                Id = entity.Id,
                Event = entityEvent,
                Entity = entity,
                Param = param
            };
            NetClient.Instance.SendMessage(message);
        }

        void OnMapEntitySync(object sender, MapEntitySyncResponse response)
        {
            //注释是用于调试输出同步日志
            //System.Text.StringBuilder sb = new System.Text.StringBuilder();
            //sb.AppendFormat("MapEntityUpdateResponse: Entitys:{0}", response.entitySyncs.Count);
            //sb.AppendLine();
            foreach (var entity in response.entitySyncs)
            {
                EntityManager.Instance.OnEntitySync(entity);   //实际有用  
                //sb.AppendFormat("    [{0}]evt:{1} entity:{2}", entity.Id, entity.Event, entity.Entity.String());
                //sb.AppendLine();
            } 
            //Debug.Log(sb.ToString());
        }
        #endregion

        #region 传送门
        /// <param name="teleporterID">ID</param>
        public void SendMapTeleport(int teleporterID)
        {
            Debug.LogFormat("MapTeleportRequest :teleporterID:{0}", teleporterID);

            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.mapTeleport = new MapTeleportRequest();
            message.Request.mapTeleport.teleporterId = teleporterID;
            NetClient.Instance.SendMessage(message);
        }
        #endregion     
    }
}
