using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

using SkillBridge.Message;
namespace Managers
{
    class FriendManager : Singleton<FriendManager>, IDisposable
    {
        public List<NFriendInfo> allFriends = new List<NFriendInfo>();
        //public Dictionary<int,NFriendInfo> allFriends = new Dictionary<int,NFriendInfo>();
        public FriendManager()
        {

        }

        public void Dispose()
        {
        }
        public void Init(List<NFriendInfo> friends)
        {
            this.allFriends = friends;
        }
    }
}


