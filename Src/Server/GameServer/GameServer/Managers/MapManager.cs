using Common;
using GameServer.Models;
using System.Collections.Generic;

namespace GameServer.Managers
{
    class MapManager : Singleton<MapManager>
    {
        Dictionary<int, Map> Maps = new Dictionary<int, Map>();

        public void Init()
        {
            foreach (var mapdefine in DataManager.Instance.Maps.Values) //地图配置
            {
                Map map = new Map(mapdefine);
                Log.InfoFormat("MapManager.Init > Map:{0}:{1}", map.Define.ID, map.Define.Name);
                this.Maps[mapdefine.ID] = map;
            }
        }

        //重载this
        public Map this[int key]
        {
            get
            {
                return this.Maps[key];
            }
        }


        public void Update()
        {
            foreach(var map in this.Maps.Values)
            {
                map.Update();   //以后用于刷新怪物
            }
        }
    }
}
