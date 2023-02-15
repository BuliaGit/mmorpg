using Common;
using Common.Data;
using GameServer.Managers;

namespace GameServer.Models
{
    /// <summary>
    /// 刷怪器
    /// </summary>
    class Spawner
    {
        public SpawnRuleDefine Define { get; set; }
        private Map Map;
        /// <summary>
        /// 刷新时间
        /// </summary>
        private float spawnTime = 0;
        /// <summary>
        /// 消失时间
        /// </summary>
        private float unspawnTime = 0;
        private bool spawned = false;
        /// <summary>
        /// 当前刷怪点
        /// </summary>
        private SpawnPointDefine spawnPoint = null;

        public Spawner(SpawnRuleDefine define,Map map)
        {
            this.Define = define;
            this.Map = map;

            if(DataManager.Instance.SpawnPoints.ContainsKey(Map.ID))
            {
                if (DataManager.Instance.SpawnPoints[Map.ID].ContainsKey(Define.SpawnPoint))
                 {
                    spawnPoint = DataManager.Instance.SpawnPoints[Map.ID][Define.SpawnPoint];
                }
                else
                {
                    Log.ErrorFormat("SpawnRule[{0}] SpawnPoint[1] not existed", Define.ID, Define.SpawnPoint);
                }
            }
        }
        public void Update()
        {
            if(this.CanSpawn())
            {
                this.Spawn();
            }
        }

        private bool CanSpawn()
        {
            if (this.spawned)
                return false;
            if (this.unspawnTime + Define.SpawnPeriod > Time.time)
                return false;

            return true;
        }

        public void Spawn()
        {
            Log.InfoFormat("Map[{0}] Spawn[{1}:Mon:{2},lv:{3}] At Point;{4}", Define.MapID, Define.ID, Define.SpawnMonID, this.Define, this.Define);

            this.spawned = true;
            Map.MonsterManager.Create(Define.SpawnMonID, Define.SpawnLevel, spawnPoint.Position, spawnPoint.Direction, Define.ID);
        }   
    }
}
