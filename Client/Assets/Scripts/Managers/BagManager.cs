using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Models;
using SkillBridge.Message;
using Managers;


class BagManager : Singleton<BagManager>, IDisposable
{
    public int Unlocked;
    public BagItem[] Items;
    NBagInfo Info;

    unsafe public void Init(NBagInfo info)
    {
        this.Info = info;
        this.Unlocked = info.Unlocked;
        Items = new BagItem[this.Unlocked];

        if(info.Items != null && info.Items.Length >= this.Unlocked)
        {
            Analyze(info.Items);
        }
        else
        {
            info.Items = new byte[sizeof(BagItem) * this.Unlocked];
            Reset();
        } 
            
    }

    public void Reset()
    {
        int i = 0;
        foreach(var kv in ItemManager.Instance.Items)
        {
            if(kv.Value.Count <= kv.Value.Define.StackLimit)
            {
                this.Items[i].ItemId = (ushort)kv.Key;
                this.Items[i].Count = (ushort)kv.Value.Count;
            }
            else
            {
                int count = kv.Value.Count;
                while(count > kv.Value.Define.StackLimit)
                {
                    this.Items[i].ItemId = (ushort)kv.Key;
                    this.Items[i].Count = (ushort)kv.Value.Define.StackLimit;
                    i++;
                    count -= kv.Value.Define.StackLimit;
                }
                if(count != 0)
                {
                    this.Items[i].ItemId = (ushort)kv.Key;
                    this.Items[i].Count = (ushort)count;
                }
                else
                {
                    i--;
                }
            }
            i++;
        }
    }

    unsafe void Analyze(byte[] data)
    {
        //指向 data 的指针
        //使用fixed 关键字固定 data 的内存位置，防止垃圾回收器移动它
        fixed (byte* pt = data)
        {
            
            for(int i = 0;i < this.Unlocked;i++)
            {
                //pt 最开始的指针 + i (n - 1) * sizeof(BagItem) 偏移量，指向第 i (n - 1) 个 BagItem 的位置
                BagItem* item = (BagItem*)(pt + i * sizeof(BagItem));
                //把指针指向的数据赋值给 Items[i]
                Items[i] = *item;
            }
        }
    }

    //从结构体数据变为字节数据
    unsafe public NBagInfo GetBagInfo()
    {
        fixed (byte* pt = Info.Items)
        {
            for(int i = 0;i < this.Unlocked;i++)
            {
                BagItem* item = (BagItem*)(pt + i * sizeof(BagItem));
                //把 Items[i] 的数据赋值给指针指向的位置
                *item = Items[i];
            }
        }
        return this.Info;
    }

    public BagManager()
    {
    }

    public void Dispose()
    {
    }

    //增加道具到背包。操作采用全有或全无，避免背包空间不足时只写入一部分。
    public bool AddItem(int itemId, int count)
    {
        if (this.Items == null || count <= 0 || DataManager.Instance.Items == null)
            return false;

        Common.Data.ItemDefine define;
        if (!DataManager.Instance.Items.TryGetValue(itemId, out define))
            return false;

        int stackLimit = define.StackLimit;
        if (stackLimit <= 0 || stackLimit > ushort.MaxValue || itemId <= 0 || itemId > ushort.MaxValue)
            return false;

        // 先预估总容量。预检通过后才修改 Items，避免部分写入。
        long capacity = 0;
        for (int i = 0; i < this.Items.Length; i++)
        {
            if (this.Items[i].ItemId == itemId)
            {
                int free = stackLimit - this.Items[i].Count;
                if (free > 0)
                    capacity += free;
            }
            else if (this.Items[i].ItemId == 0)
            {
                capacity += stackLimit;
            }
        }

        if (capacity < count)
            return false;

        int remaining = count;

        // 第一阶段：优先填充已有的同类堆。
        for (int i = 0; i < this.Items.Length && remaining > 0; i++)
        {
            if (this.Items[i].ItemId != itemId)
                continue;

            int free = stackLimit - this.Items[i].Count;
            if (free <= 0)
                continue;

            int add = Math.Min(remaining, free);
            this.Items[i].Count = (ushort)(this.Items[i].Count + add);
            remaining -= add;
        }

        // 第二阶段：数量仍有剩余时，按 StackLimit 拆分到空格。
        for (int i = 0; i < this.Items.Length && remaining > 0; i++)
        {
            if (this.Items[i].ItemId != 0)
                continue;

            int add = Math.Min(remaining, stackLimit);
            this.Items[i].ItemId = (ushort)itemId;
            this.Items[i].Count = (ushort)add;
            remaining -= add;
        }

        // capacity 已经预检过，正常情况下这里必然为 0。
        return remaining == 0;
    }

    public void RemoveItem(int itemId, int count)
    {
        
    }
}
