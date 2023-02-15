using Common;
using System;
using System.IO;

namespace GameServer
{
    class Program
    {
        static void Main(string[] args)
        {
            FileInfo fi = new FileInfo("log4net.xml");  //获取log4net.xml文件信息
            log4net.Config.XmlConfigurator.ConfigureAndWatch(fi);
            Log.Init("GameServer"); //设置日志名称（日志文件可见）
            Log.Info("Game Server Init");

            GameServer server = new GameServer();
            server.Init();
            server.Start();
            Console.WriteLine("Game Server Running......"); //服务器初始化完毕

            CommandHelper.Run();
            Log.Info("Game Server Exiting...");
            server.Stop();
            Log.Info("Game Server Exited");
        }
    }
}
