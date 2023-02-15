using GameServer.Entities;
using GameServer.Models;
using SkillBridge.Message;
using System.Collections.Generic;

namespace GameServer.Managers
{
    class MonsterManager
    {
        /// <summary>
        /// 当前地图
        /// </summary>
        private Map Map;

        public Dictionary<int, Monster> Monsters = new Dictionary<int, Monster>();

        public void Init(Map map)
        {
            this.Map = map;
        }

        /// <summary>
        /// 创建怪物
        /// </summary>
        /// <param name="spawnMonID"></param>
        /// <param name="spawnLevel"></param>
        /// <param name="psstion"></param>
        /// <param name="dirtion"></param>
        /// <param name="spw"></param>
        /// <returns></returns>
        internal Monster Create(int spawnMonID, int spawnLevel, NVector3 psstion, NVector3 dirtion, int spw)
        {
            Monster monster = new Monster(spawnMonID, spawnLevel, psstion, dirtion);
            EntityManager.Instance.AddEntity(this.Map.ID, monster);
            monster.Id = monster.entityId;      //可有可无
            monster.Info.EntityId = monster.entityId;
            monster.Info.mapId = this.Map.ID;
            Monsters[monster.Id] = monster;

            this.Map.MonsterEnter(monster);
            return monster;
        }

        //internal void RemoveMonster(int monsterId)
        //{
        //    if (this.Monsters.ContainsKey(monsterId))
        //    {
        //        var cha = this.Monsters[monsterId];
        //        EntityManager.Instance.RemoveEntity(this.Map.ID, cha);
        //        this.Monsters.Remove(monsterId);
        //    }
        //}
    }
}
