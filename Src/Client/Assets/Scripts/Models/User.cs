using Common.Data;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Models
{
    class User : Singleton<User>
    {
        SkillBridge.Message.NUserInfo userInfo;


        public SkillBridge.Message.NUserInfo Info
        {
            get { return userInfo; }
        }


        public void SetupUserInfo(SkillBridge.Message.NUserInfo info)
        {
            this.userInfo = info;
        }

        public MapDefine currentMapData { get; set; }
        public SkillBridge.Message.NCharacterInfo CurrentCharacter { get; set; }
        public GameObject currentCharacter { get; set; }
        public NteamInfo TeamInfo { get; set; }

        public void AddGold(int gold)
        {
            this.CurrentCharacter.Gold += gold;
        }
    }
}
