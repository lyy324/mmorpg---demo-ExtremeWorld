using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common.Data;

namespace Managers
{
    public class NpcManager: Singleton<NpcManager>
    {
        public delegate bool NpcActionHandler(NpcDefine npc);
        Dictionary<NpcFunction,NpcActionHandler> eventMap = new Dictionary<NpcFunction, NpcActionHandler>();

        public void RegisterNpcEvent(NpcFunction function,NpcActionHandler action)
        {
            if(!eventMap.ContainsKey(function))
            {
                eventMap[function] = action;
            }
            else
                eventMap[function] += action;
        }

        public NpcDefine GetNpcDefine(int npcID)
        {
            NpcDefine npc = null;
            DataManager.Instance.Npcs.TryGetValue(npcID,out npc);
            return npc;
        }


        //判断 npc 是否存在 存在找到数据调用 判断NPC职能的函数
        public bool Interactive(int npcID)
        {
            if(DataManager.Instance.Npcs.ContainsKey(npcID))
            {
                var npc = DataManager.Instance.Npcs[npcID];
                return Interactive(npc);
            }
            return false;
        }

        //判断 npc 的职能 并且调用
        public bool Interactive(NpcDefine npc)
        {
            if(DoTaskInteractive(npc))
            {
                return true;
            }
            //if(npc.Type == NpcType.Task)
            //{
            //    return DoTaskInteractive(npc);
            //}
            else if(npc.Type == NpcType.Functional)
            {
                return DoFunctionInteractive(npc);
            }
            return false;
        }

        //通过这里 点击 npc 获取任务 
        private bool DoTaskInteractive(NpcDefine npc)
        {
            //整个 npc 系统对任务系统的调用只有两行代码 任务系统对NPC系统没有引用 耦合性低
            var status = QuestManager.Instance.GetQuestStatusByNpc(npc.ID);
            if (status == NpcQuestStatus.None)
                return false;
            return QuestManager.Instance.OpenNpcQuest(npc.ID);
        }

        private bool DoFunctionInteractive(NpcDefine npc)
        {
            if(npc.Type != NpcType.Functional)
                return false;
            if(!eventMap.ContainsKey(npc.Function))
                return false;
            return eventMap[npc.Function](npc);
        }



    }
}
