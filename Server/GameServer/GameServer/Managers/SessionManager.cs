using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Common;
using Network;

namespace GameServer.Managers
{
    class SessionManager : Singleton<SessionManager>
    {
        //Key 角色id  Value 角色与服务器的连接
        public Dictionary<int, NetConnection<NetSession>> sessions = new Dictionary<int, NetConnection<NetSession>>();

        public void AddSession(int characterId, NetConnection<NetSession> session)
        {
            this.sessions[characterId] = session;
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
