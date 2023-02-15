using GameServer.Managers;
using GameServer.Services;
using Network;
using Server.Services;
using System.Threading;

namespace GameServer
{
    class GameServer
    {
        NetService network;
        Thread thread;
        bool running = false;

        public void Init()
        {
            int Port = Properties.Settings.Default.ServerPort;
            network = new NetService();
            network.Init(Port);     //建立通讯

            //数据管理加载
            DataManager.Instance.Load();
            //数据库服务
            DBService.Instance.Init();
            //地图服务
            MapService.Instance.Init();
            //角色服务
            UserService.Instance.Init();
            //道具服务
            ItemService.Instance.Init();
            //任务服务
            QuestService.Instance.Init();
            //好友
            FriendService.Instance.Init();
            //组队
            TeamService.Instance.Init();
            //公会
            GuildService.Instance.Init();
            //聊天
            ChatService.Instance.Init();
            //战斗
            BattleService.Instance.Init();

            thread = new Thread(new ThreadStart(this.Update));  //创建线程
        }

        public void Start()
        {
            network.Start();
            running = true;
            thread.Start();
        }


        public void Stop()
        {
            running = false;
            thread.Join();
            network.Stop();
        }

        public void Update()
        {
            var mapManager = MapManager.Instance;
            while (running)
            {
                Time.Tick();
                Thread.Sleep(100); //休眠0.1秒
                //Console.WriteLine("{0} {1} {2} {3} {4}", Time.deltaTime, Time.frameCount, Time.ticks, Time.time, Time.realtimeSinceStartup);
                mapManager.Update();
            }
        }
    }
}
