using GameServer;
using GameServer.Entities;
using GameServer.Services;
using SkillBridge.Message;

namespace Network
{
    /// <summary>
    /// 数据库数据缓存器（避免频繁从数据库获取数据）
    /// </summary>
    class NetSession : INetSession
    {
        /// <summary>
        /// 数据库用户实体
        /// </summary>
        public TUser User { get; set; }

        /// <summary>
        /// 数据库角色实体
        /// </summary>
        public Character Character { get; set; }

        /// <summary>
        /// 协议实体
        /// </summary>
        public NEntity Entity { get; set; }

        public IPostResponser PostResponser;

        public void Disconnected()
        {
            this.PostResponser = null;
            if (this.Character != null)
                UserService.Instance.CharacterLeave(this.Character);
        }

        /// <summary>
        /// 根消息
        /// </summary>
        NetMessage response;

        public NetMessageResponse Response
        {
            get
            {
                if (response == null)
                {
                    response = new NetMessage();
                }
                if (response.Response == null)
                {
                    response.Response = new NetMessageResponse();
                }

                return response.Response;
            }
        }

        public byte[] GetResponse()
        {
            if (response != null)
            {
                if (PostResponser != null)
                {
                    this.PostResponser.PostProcess(Response);
                }

                byte[] data = PackageHandler.PackMessage(response);
                response = null;    //保证唯一性
                return data;
            }
            return null;
        }
    }
}
