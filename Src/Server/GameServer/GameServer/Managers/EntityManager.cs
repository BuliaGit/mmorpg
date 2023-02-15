using Common;
using GameServer.Core;
using GameServer.Entities;
using System;
using System.Collections.Generic;

namespace GameServer.Managers
{
    class EntityManager : Singleton<EntityManager>
    {
        private int idx = 0;
        public Dictionary<int, Entity> AllEntities = new Dictionary<int, Entity>();
        public Dictionary<int, List<Entity>> MapEntities = new Dictionary<int, List<Entity>>();

        public void AddEntity(int mapId, Entity entity)
        {
            entity.EntityData.Id = ++idx;   //todo：加入管理器生成唯一Id（entityId）
            AllEntities.Add(entity.EntityData.Id, entity);

            List<Entity> entities = null;
            if (!MapEntities.TryGetValue(mapId, out entities))
            {
                entities = new List<Entity>();
                MapEntities[mapId] = entities;
            }
            entities.Add(entity);
        }

        public void RemoveEntity(int mapId, Entity entity)
        {
            AllEntities.Remove(entity.entityId);
            MapEntities[mapId].Remove(entity);
        }

        public Entity GetEntity(int entityId)
        {
            Entity entity = null;
            AllEntities.TryGetValue(entityId, out entity);
            return entity;
        }
        public Creature GetCreature(int entityId)
        {
            return GetEntity(entityId) as Creature;
        }

        public List<T> GetMapEntities<T>(int mapId,Predicate<Entity> match) where T:Creature
        {
            List<T> result = new List<T>();
            foreach (var entity in MapEntities[mapId])
            {
                if(entity is T && match.Invoke(entity))
                {
                    result.Add((T)entity);
                }
            }
            return result;
        }

        public List<T> GetMapEntitiesInRange<T>(int mapId,Vector3Int pos,int range)where T : Creature
        {
            return GetMapEntities<T>(mapId, (entity) =>
            {
                T creature = entity as T;
                return creature.Distance(pos) < range;
            });
        }
    }
}
