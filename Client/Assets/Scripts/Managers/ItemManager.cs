using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Models;
using SkillBridge.Message;
using Common.Data;
using Services;

namespace Managers
{
    class ItemManager : Singleton<ItemManager>, IDisposable
    {
        public Dictionary<int, Item> Items = new Dictionary<int, Item>( );

        public void Init(List<NItemInfo> items)
        {
            this.Items.Clear();
            foreach(var info in items)
            {
                Item item = new Item(info);
                this.Items.Add(item.Id, item);
                Debug.LogFormat("ItemManager: Init {{0}}",item);
            }
            StatusService.Instance.RegisterStatusNotify(StatusType.Item, OnItemNotify);
        }

        private bool OnItemNotify(NStatus status)
        {
            if(status.Type == StatusType.Item)
            {
                if(status.Action == StatusAction.Add)
                {
                    this.AddItem(status.Id, status.Value);
                }
                if(status.Action ==StatusAction.Delete)
                {
                    this.RemoveItem(status.Id, status.Value);
                }
                return true;
            }
            return false;
        }

        private void AddItem(int itemId,int count)
        {
            // 先更新背包格子；背包空间不足时，不修改物品汇总数据。
            if (!BagManager.Instance.AddItem(itemId, count))
            {
                Debug.LogWarningFormat("ItemManager: cannot add item {0} count {1} to bag", itemId, count);
                return;
            }

            Item item = null;
            if (this.Items.TryGetValue(itemId, out item))
            {
                item.Count += count;
            }
            else
            {
                item = new Item(itemId, count);
                this.Items.Add(itemId, item);
            }
        }

        private void RemoveItem(int itemId,int count)
        {
            if (!this.Items.ContainsKey(itemId))
                return;
            Item item = this.Items[itemId];
            if(item.Count < count)
                Items.Remove(itemId);
            else
            {
                item.Count -= count;
            }
            BagManager.Instance.RemoveItem(itemId,count);
        }

        //还在制作中 ... ...
        public ItemDefine GetItem(int itemId)
        {
            return null;
        }

        public bool UseItem(int itemId)
        {
            return false;
        }

        public bool UseItem(ItemDefine item)
        {
            return false;
        }


        public ItemManager()
        {
        }

        public void Dispose()
        {
        }

    }
}


