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
        public PlayerInputController currentCharacter { get; set; }
        public NteamInfo TeamInfo { get; set; }

        public void AddGold(int gold)
        {
            this.CurrentCharacter.Gold += gold;
        }

        public int CurrentRide = 0;
        internal void Ride(int id)
        {
            if(CurrentRide != id)
            {
                CurrentRide = id;
                currentCharacter.SendEntityEvent(EntityEvent.Ride, id);
            }
            else
            {
                CurrentRide = 0;
                currentCharacter.SendEntityEvent(EntityEvent.Ride, 0);
            }
        }
    }
}
