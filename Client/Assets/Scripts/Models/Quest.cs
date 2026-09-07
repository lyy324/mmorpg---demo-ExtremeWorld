using Common.Data;
using SkillBridge.Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Models
{
    public class Quest
    {
        public QuestDefine Define;
        public NQuestInfo info;

        public Quest()
        {

        }

        public Quest(NQuestInfo info)
        {
            this.info = info;
            this.Define = DataManager.Instance.Quests[info.QuestId];
        }

        public Quest(QuestDefine define)
        {
            this.Define = define;
            this.info = null;
        }

        public string GetTypeName( )
        {
            return EnumUtil.GetEnumDescription(this.Define.Type);
        }
    }
}


