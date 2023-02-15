using Entities;
using SkillBridge.Message;
using System;
using System.Collections.Generic;

namespace Managers
{
    /// <summary>
    /// 实体通知接口（为了不用写很多委托）
    /// </summary>
    interface IEntityNotify
    {
        /// <summary>
        /// 移除通知
        /// </summary>
        void OnEntityRemoved();

        /// <summary>
        /// 数据改变通知
        /// </summary>
        /// <param name="entity"></param>
        void OnEntityChanged(Entity entity);

        /// <summary>
        /// 状态改变通知
        /// </summary>
        /// <param name="event"></param>
        void OnEntityEvent(EntityEvent @event,int param); 
    }

    /// <summary>
    /// 实体管理器
    /// </summary>
    class EntityManager : Singleton<EntityManager>
    {
        public Dictionary<int, Entity> entities = new Dictionary<int, Entity>();
        public Dictionary<int, IEntityNotify> notifiers = new Dictionary<int, IEntityNotify>();


        public void RegisterEntityChangeNotify(int entityId, IEntityNotify notify)
        {
            this.notifiers[entityId] = notify;
        }

        public void AddEntity(Entity entity)
        {
            this.entities[entity.entityId] = entity;
        }

        public void RemoveEntity(NEntity entity)
        {
            this.entities.Remove(entity.Id);
            if(notifiers.ContainsKey(entity.Id))
            {
                notifiers[entity.Id].OnEntityRemoved();
                notifiers.Remove(entity.Id);
            }
        }

        /// <summary>
        /// 实现其他玩家同步
        /// </summary>
        /// <param name="data"></param>
        internal void OnEntitySync(NEntitySync data)
        {
            Entity entity = null;
            entities.TryGetValue(data.Id, out entity);  //如果要查找和赋值，比ContainsKey省性能

            if (entity != null)
            {
                if (data.Entity != null)
                {
                    entity.EntityData = data.Entity;  //更新其他玩家位置
                }

                if (notifiers.ContainsKey(data.Id))
                {
                    notifiers[entity.entityId].OnEntityChanged(entity);
                    notifiers[entity.entityId].OnEntityEvent(data.Event,data.Param);
                }
            }

        }

        public Entity GetEntity(int entityId)
        {
            Entity entity = null;
            entities.TryGetValue(entityId, out entity);
            return entity;
        }
    }
}