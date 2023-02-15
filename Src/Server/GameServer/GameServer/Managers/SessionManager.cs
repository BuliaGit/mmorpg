using Common;
using Network;
using System.Collections.Generic;

namespace GameServer.Managers
{
    class SessionManager:Singleton<SessionManager>
    {
        public Dictionary<int, NetConnection<NetSession>> sessions = new Dictionary<int, NetConnection<NetSession>>();

        public void AddSession(int characterID, NetConnection<NetSession> session)
        {
            this.sessions[characterID] = session;
        }

        public void RemoveSession(int characterId)
        {
            this.sessions.Remove(characterId);
        }

        public NetConnection<NetSession> GetSession(int characterId)
        {
            NetConnection<NetSession> session = null;
            this.sessions.TryGetValue(characterId, out session);
            return session;
        }
    }
}
