using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Common;
using GameServer.Entities;
using GameServer.Services;
using Network;
using SkillBridge.Message;

namespace GameServer.Managers
{
    class FriendManager
    {
        private Character Owner;

        List<NFriendInfo> friends = new List<NFriendInfo>();

        bool friendChanged = false;

        public FriendManager(Character owner)
        {
            this.Owner = owner;
            this.InitFriends();
        }

        private void InitFriends()
        {
            this.friends.Clear();
            foreach(var friend in this.Owner.Data.Friends)
            {
                this.friends.Add(GetFriendInfo(friend));
            }
        }

        public void GetFriendInfos(List<NFriendInfo> list)
        {
            foreach(var f in this.friends)
            {
                list.Add(f);
            }
        }

        internal void PostProcess(NetMessageResponse message)
        {
            if (this.friendChanged)
            {
                Log.InfoFormat("PostProcess > FriendManager : CharacterID:{0}:{1}  ", this.Owner.Id, this.Owner.Info.Name);
                this.InitFriends();
                if (message.friendList == null)
                {
                    message.friendList = new FriendListResponse();
                    message.friendList.Friends.AddRange(this.friends);
                }
                this.friendChanged = false;
            }
        }

        public void OfflineNotify()
        {
            foreach (var friendinfo in friends)
            {
                var friend = CharacterManager.Instance.GetCharacter(friendinfo.friendInfo.Id);
                if (friend != null)
                {
                    friend.FriendManager.UpdateFriendInfo(this.Owner.Info, 0);
                }
            }
        }
        public void UpdateFriendInfo(NCharacterInfo friendInfo, int status)
        {
            foreach (var friend in this.friends)
            {
                if (friend.friendInfo.Id == friendInfo.Id)
                {
                    friend.Status = status;
                    break;
                }
            }
            this.friendChanged = true;
        }
        public NFriendInfo GetFriendInfo(int friendId)
        {
            foreach (var fri in this.friends)
            {
                if (fri.friendInfo.Id == friendId)
                {
                    return fri;
                }
            }
            return null;
        }
        public NFriendInfo GetFriendInfo(TCharacterFriend friend)
        {
            NFriendInfo friendInfo = new NFriendInfo();
            var character = CharacterManager.Instance.GetCharacter(friend.FriendID);
            friendInfo.friendInfo = new NCharacterInfo();
            friendInfo.Id = friend.Id;

            if (character == null)
            {
                friendInfo.friendInfo.Id = friend.FriendID;
                friendInfo.friendInfo.Name = friend.FriendName;
                friendInfo.friendInfo.Class = (CharacterClass)friend.Class;
                friendInfo.friendInfo.Level = friend.Level;
                friendInfo.Status = 0;
            }
            else
            {
                //存在交叉引用 引用的 Character 中的 friendInfo 也可能引用这些 造成循环
                //friendInfo.friendInfo = character.GetBasicInfo();

                //解决方法
                friendInfo.friendInfo = this.GetBasicInfo(character.Info);
                friendInfo.friendInfo.Name = character.Info.Name;
                friendInfo.friendInfo.Class = character.Info.Class;
                friendInfo.friendInfo.Level = character.Info.Level;
                if (friend.Level != character.Info.Level)
                {
                    friend.Level = character.Info.Level;
                }
                character.FriendManager.UpdateFriendInfo(this.Owner.Info, 1);
                friendInfo.Status = 1;
            }
            Log.InfoFormat("{0}:{1} GetFriendInfo :{2}:{3} status:{4}", this.Owner.Id, this.Owner.Info.Name, friendInfo.friendInfo.Id, friendInfo.friendInfo.Name, friendInfo.Status);
            return friendInfo;
        }

        NCharacterInfo GetBasicInfo(NCharacterInfo info)
        {
            return new NCharacterInfo()
            {
                Id = info.Id,
                Name = info.Name,
                Class = info.Class,
                Level = info.Level,
            };
        }

        public void AddFriend(Character friend)
        {
            TCharacterFriend tf = new TCharacterFriend()
            {
                FriendID = friend.Id,
                FriendName = friend.Data.Name,
                Class = friend.Data.Class,
                Level = friend.Data.Level

            };
            this.Owner.Data.Friends.Add(tf);
            friendChanged = true;
        }

        public bool RemoveFriendByFriendId(int friendId)
        {
            var removeItem = this.Owner.Data.Friends.FirstOrDefault(v => v.FriendID == friendId);
            if(removeItem != null)
            {
                DBService.Instance.Entities.CharacterFriends.Remove(removeItem);
            }
            friendChanged = true;
            return true;
        }

        public bool RemoveFriendById(int id)
        {
            var removeItem = this.Owner.Data.Friends.FirstOrDefault(v => v.Id == id);
            if(removeItem != null)
            {
                DBService.Instance.Entities.CharacterFriends.Remove(removeItem);
            }
            friendChanged = true;
            return true;
        }
    }
}
