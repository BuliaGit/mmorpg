using Common;
using System.Net;
using System.Net.Sockets;

namespace Network
{
    class NetService
    {
        private static TcpSocketListener ServerListener;    //创建监听器

        public void Init(int port)
        {
            //127.0.0.1:8080
            ServerListener = new TcpSocketListener("127.0.0.1", port);
            ServerListener.SocketConnected += OnSocketConnected;
        }


        public void Start()
        {
            //启动监听
            Log.Warning("Starting Listener...");
            ServerListener.Start();

            MessageDistributer<NetConnection<NetSession>>.Instance.Start(8); //使用8个线程来派发消息
            Log.Warning("NetService Started");
        }


        public void Stop()
        {
            Log.Warning("Stop NetService...");

            ServerListener.Stop();

            Log.Warning("Stoping Message Handler...");
            MessageDistributer<NetConnection<NetSession>>.Instance.Stop();
        }

        /// <summary>
        /// 连接回调
        /// </summary>
        /// <param name="sender">事件源</param>
        /// <param name="e">Socket对象</param>
        private void OnSocketConnected(object sender, Socket e)
        {
            IPEndPoint clientIP = (IPEndPoint)e.RemoteEndPoint; //获取客户端IP、端口
            //可以在这里对IP做一级验证,比如黑名单

            NetSession session = new NetSession();

            NetConnection<NetSession> connection = new NetConnection<NetSession>(
                e, 
                new NetConnection<NetSession>.DataReceivedCallback(DataReceived),
                new NetConnection<NetSession>.DisconnectedCallback(Disconnected), 
                session
                );

            Log.WarningFormat("Client[{0}]] Connected", clientIP);
        }


        /// <summary>
        /// 连接断开回调
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void Disconnected(NetConnection<NetSession> sender, SocketAsyncEventArgs e)
        {
            //Performance.ServerConnect = Interlocked.Decrement(ref Performance.ServerConnect);
            sender.Session.Disconnected();
            Log.WarningFormat("Client[{0}] Disconnected", e.RemoteEndPoint);
        }


        /// <summary>
        /// 接受数据回调
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void DataReceived(NetConnection<NetSession> sender, DataEventArgs e)
        {
            //Log.WarningFormat("Client[{0}] DataReceived Len:{1}", e.RemoteEndPoint, e.Length);
            //由包处理器处理封包
            lock (sender.packageHandler)
            {
                sender.packageHandler.ReceiveData(e.Data, 0, e.Data.Length);
            }
            //PacketsPerSec = Interlocked.Increment(ref PacketsPerSec);
            //RecvBytesPerSec = Interlocked.Add(ref RecvBytesPerSec, e.Data.Length);
        }
    }
}
