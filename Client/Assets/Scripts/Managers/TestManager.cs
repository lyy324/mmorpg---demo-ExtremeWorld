using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Managers;
using Common.Data;
namespace Managers
{
    public class TestManager : Singleton<TestManager> {

        public void Init()
        {
            //NpcManager.Instance.RegisterNpcEvent(NpcFunction.InvokeShop, OnInvokeShop);
            NpcManager.Instance.RegisterNpcEvent(NpcFunction.InvokeInstance, OnInvokeInstance);
        }

        public bool OnInvokeShop(NpcDefine npc)
        {
            Debug.LogFormat("TestManager.OnInvokeShop, Npc [{0},{1}]  type{2} func{3}", npc.ID,npc.Name,npc.Type,npc.Function);
            UITest test = UIManager.Instance.Show<UITest>();
            test.SetTitle(npc.ID + npc.Name + npc.Description);
            return true;
        }


        public bool OnInvokeInstance(NpcDefine npc)
        {
            Debug.LogFormat("TestManager.OnInvokeInstance, Npc [{0},{1}]  type{2} func{3}", npc.ID, npc.Name, npc.Type, npc.Function);
            MessageBox.Show("点击了Npc  " + npc.Name, "Npc对话");
            return true;
        }
    }
}

