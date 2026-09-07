using Common.Data;
using JetBrains.Annotations;
using Managers;
using Models;
using Services;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.TextCore.Text;

class EquipManager : Singleton<EquipManager>, IDisposable
{
    public delegate void OnEquipChangeHandler();
    public event OnEquipChangeHandler OnEquipChanged;
    //维护七个格子 
    public Item[] Equips = new Item[(int)EquipSlot.SlotMax];
    //用 data 来维持两者之间的转换 服务器发来的是一串 28 字节长度的数字 要先放到 data 中进行解析
    byte[] Data;

    public EquipManager()
    {
    }

    public void Dispose()
    {
    }

    
    public void Init(byte[] data)
    {
        this.Data = data;
        this.ParseEquipData(data);
    }

    public bool Contains(int equipId)
    {
        for(int i = 0;i < this.Equips.Length;i ++)
        {
            if (Equips[i] != null && Equips[i].Id == equipId)
                return true;
        }
        return false;
    }

    public Item GetEquip(EquipSlot slot)
    {
        return Equips[(int)slot];
    }

    unsafe void ParseEquipData(byte[] data)
    {
        //pt 为数组的首地址 
        fixed(byte* pt = this.Data)
        {
            for(int i = 0;i <this.Equips.Length;i ++)
            {
                int itemId = *(int*)(pt + i * sizeof(int));
                if (itemId > 0)
                    Equips[i] = ItemManager.Instance.Items[itemId];
                else
                    Equips[i] = null;
            }
        }
    }

    unsafe public byte[] GetEquipData()
    {
        fixed(byte *pt = this.Data)
        {
            for(int i = 0;i < (int)EquipSlot.SlotMax;i ++)
            {
                int* itemId = (int*)(pt + i * sizeof(int));
                if (Equips[i] == null)
                    *itemId = 0;
                else
                    *itemId = Equips[i].Id;
            }
        }
        return this.Data;
    }


    public void EquipItem(Item equip)
    {
        ItemService.Instance.SendEquipItem(equip, true);
    }
    public void UnEquipItem(Item equip)
    {
        ItemService.Instance.SendEquipItem(equip, false);
    }

    internal void OnEquipItem(Item equip)
    {
        if (this.Equips[(int)equip.EquipInfo.Slot] != null && this.Equips[(int)equip.EquipInfo.Slot].Id == equip.Id)
            return;
        this.Equips[(int)equip.EquipInfo.Slot] = ItemManager.Instance.Items[equip.Id];

        if (OnEquipChanged != null)
            OnEquipChanged();
    }

    internal void OnUnEquipItem(EquipSlot slot)
    {
        if (this.Equips[(int)slot] != null)
        {
            this.Equips[(int)slot] = null;
            if(OnEquipChanged != null)
                OnEquipChanged();
        }
    }
}
