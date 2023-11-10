using SkillBridge.Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public class FriendManager : Singleton<FriendManager>
    {
        public List<NFriendInfo> allFriends;

        public void Init(List<NFriendInfo> friends)
        {
            allFriends = friends;
        }
    }
}
