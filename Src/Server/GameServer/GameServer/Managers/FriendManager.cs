using Common;
using GameServer.Entities;
using GameServer.Services;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Managers
{
    class FriendManager
    {
        Character Owner;
        List<NFriendInfo> friends = new List<NFriendInfo>();

        bool friendChanged = false;
        
        public FriendManager(Character owner)
        {
            Owner = owner;
            InitFriends();
        }

        public void GetFriendInfos(List<NFriendInfo> friends)
        {
            foreach (var friend in friends)
            {
                friends.Add(friend);
            }
        }

        private void InitFriends()
        {
            this.friends.Clear();
            foreach(var friend in Owner.Data.Friends)
            {
                this.friends.Add(GetFriendInfo(friend));
            }
        }

        public NFriendInfo GetFriendInfo(TCharacterFriend friend)
        {
            NFriendInfo friendInfo = new NFriendInfo();
            var character = CharacterManager.Instance.GetCharacter(friend.FriendId);
            friendInfo.friendInfo = new NCharacterInfo();
            friendInfo.Id = friend.Id;
            if(character == null)
            {
                friendInfo.friendInfo.Id = friend.FriendId;
                friendInfo.friendInfo.Name = friend.FriendName;
                friendInfo.friendInfo.Class = (CharacterClass)friend.Class;
                friendInfo.friendInfo.Level = friend.Level;
                friendInfo.Status = 0;
            }
            else
            {
                friendInfo.friendInfo = character.GetBasicInfo();
                friendInfo.friendInfo.Name = character.Info.Name;
                friendInfo.friendInfo.Class = character.Info.Class;
                friendInfo.friendInfo.Level = character.Info.Level;

                if (friend.Level != character.Info.Level)
                    friend.Level = character.Info.Level;

                character.FriendManager.UpdateFriendInfo(this.Owner.Info, 1);
                friendInfo.Status = 1;
            }
            Log.Info("GetFriendInfo");
            return friendInfo;
        }

        public NFriendInfo GetFriendInfo(int friendId)
        {
            foreach (var f in this.friends)
            {
                if (f.friendInfo.Id == friendId)
                {
                    return f;
                }
            }
            return null;
        }

        public void AddFriend(Character friend)
        {
            TCharacterFriend tf = new TCharacterFriend()
            {
                FriendId = friend.Id,
                FriendName = friend.Data.Name,
                Class = friend.Data.Class,
                Level = friend.Data.Level,
            };
            Owner.Data.Friends.Add(tf);
            friendChanged = true;
        }

        public bool RemoveFriendByFriendId(int friendId)
        {
            var removeItem = Owner.Data.Friends.FirstOrDefault(v => v.FriendId == friendId);
            if (removeItem != null)
                DBService.Instance.Entities.CharacterFriends.Remove(removeItem);
            friendChanged = true;
            return true;
        }

        public bool RemoveFriendById(int id)
        {
            var removeItem = Owner.Data.Friends.FirstOrDefault(v => v.Id == id);
            if(removeItem != null)
                DBService.Instance.Entities.CharacterFriends.Remove(removeItem);
            friendChanged = true;
            return true;
        }

        public void UpdateFriendInfo(NCharacterInfo friendInfo, int status)
        {
            foreach(var f in this.friends)
            {
                if(f.friendInfo.Id == friendInfo.Id)
                {
                    f.Status = status;
                    break;
                }
            }
            this.friendChanged = true;
        }

        public void OffLineNotify()
        {
            foreach(var friendInfo in this.friends)
            {
                var friend = CharacterManager.Instance.GetCharacter(friendInfo.friendInfo.Id);
                if (friend != null)
                    friend.FriendManager.UpdateFriendInfo(this.Owner.Info, 0);
            }
        }

        public void PoseProcess(NetMessageResponse message)
        {
            if (friendChanged)
            {
                Log.Info("PostProcess");
                InitFriends();
                if(message.friendList == null)
                {
                    message.friendList = new FriendListResponse();
                    message.friendList.Friends.AddRange(friends);
                }
                friendChanged = false;
            }
        }
    }
}
