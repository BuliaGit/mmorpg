using GameServer.Models;
using System.Collections.Generic;

namespace GameServer.Managers
{
    /// <summary>
    /// 刷怪管理器
    /// </summary>
    class SpawnManager
    {
        /// <summary>
        /// 刷怪规则列表
        /// </summary>
        private List<Spawner> Rules = new List<Spawner>();
        private Map Map;

        /// <summary>
        /// 初始化刷怪规则列表
        /// </summary>
        /// <param name="map"></param>
        public void Init(Map map)
        {
            this.Map = map;
            if(DataManager.Instance.SpawnRules.ContainsKey(map.Define.ID))
            {
                foreach(var define in DataManager.Instance.SpawnRules[map.Define.ID].Values)
                {
                    this.Rules.Add(new Spawner(define, this.Map));
                }
            }
        }
        public void Update()
        {
            if (Rules.Count == 0)
                return;

            for (int i = 0; i < this.Rules.Count; i++)
            {
                this.Rules[i].Update();
            }
        }
    }
}
