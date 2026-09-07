using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Data;
using Services;
using SkillBridge.Message;
using UnityEngine;
using UnityEngine.Events;

namespace Models
{
    class User : Singleton<User>
    {
        public UnityAction<int> GoldChange;
        SkillBridge.Message.NUserInfo userInfo;


        public SkillBridge.Message.NUserInfo Info
        {
            get { return userInfo; }
        }


        public void SetupUserInfo(SkillBridge.Message.NUserInfo info)
        {
            this.userInfo = info;
        }


        public MapDefine CurrentMapData { get; set; }

        public SkillBridge.Message.NCharacterInfo CurrentCharacter { get; set; }

        public PlayerInputController CurrentCharacterObject { get; set; }

        public NTeamInfo TeamInfo { get; set; }

        public void AddGold(int gold)
        {
            this.CurrentCharacter.Gold += gold;

            if(GoldChange != null)
                GoldChange((int)this.CurrentCharacter.Gold);
        }


        public int CurrentRide = 0;
        internal void Ride(int id)
        {
            if (CurrentRide != id)
            {
                CurrentRide = id;
                CurrentCharacterObject.SendEntityEvent(EntityEvent.Ride, CurrentRide);
            }
            else
            {
                CurrentRide = 0;
                CurrentCharacterObject.SendEntityEvent(EntityEvent.Ride, 0);
            }
        }
    }
}
