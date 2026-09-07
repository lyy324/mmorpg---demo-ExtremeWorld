using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Common;
using GameServer.Entities;
using GameServer.Models;
using GameServer.Services;
using SkillBridge.Message;

namespace GameServer.Managers
{
    //不是单例模式 角色创建用于获取 和 管理 角色的道具信息
    class ItemManager
    {
        private Character Owner;

        public Dictionary<int, Item> Items = new Dictionary<int, Item>();

        public ItemManager(Character owner)
        {
            this.Owner = owner;

            //Character 的 Data 是 TCharacter 类型的数据库对象 里面有一个 Items 列表 是 TCharacterItem 类型的数据库对象
            //可以同过 它们的关联 通过角色得到 所有的道具信息
            foreach (var item in owner.Data.Items)
            {
                this.Items.Add(item.ItemID, new Item(item));
            }
        }

        // 使用道具
        public bool UseItem(int itemID,int count = 1)
        {
            Log.InfoFormat("UseItem: characterID:{0}:{1} ItemID:{2} Count:{3}", Owner.Id, Owner.Data, itemID, count);
            Item item = null;
            
            if(this.Items.TryGetValue(itemID, out item))
            {
                if(item.Count < count)
                    return false;
                item.Remove(count);
                return true;
            }

            //这里是使用逻辑

            return false;
        }

        public bool HasItem(int itemID)
        {
            Item item = null;
            if(this.Items.TryGetValue(itemID,out item))
                return item.Count > 0;
            return false;

        }

        public Item GetItem(int itemID)
        {
            Item item = null;
            this.Items.TryGetValue(itemID, out item);
            Log.InfoFormat("GetItem: characterID:{0}:{1} ItemID:{2} Item:{3}", Owner.Id, Owner.Data, itemID, item);
            return item;
        }

        public bool AddItem(int itemID,int count)
        {
            Item item = null;

            if(this.Items.TryGetValue(itemID, out item))
            {
                item.Add(count);
            }
            else
            {
                TCharacterItem dbItem = new TCharacterItem();
                dbItem.CharacterID = Owner.Data.ID;
                dbItem.Owner = Owner.Data;
                dbItem.ItemID = itemID;
                dbItem.ItemCount = count;
                Owner.Data.Items.Add(dbItem);
                this.Items.Add(itemID,new Item(dbItem));
            }
            this.Owner.StatusManager.AddItemChange(itemID, count, StatusAction.Add);
            Log.InfoFormat("AddItem: characterID:{0}:{1} ItemID:{2} Count:{3}", Owner.Id, Owner.Data, itemID, count);
            DBService.Instance.Save();
            return true;
        }

        public bool RemoveItem(int itemID,int count)
        {
            if(!this.Items.ContainsKey(itemID))
                return false;
            Item item = this.Items[itemID];
            if (item.Count < count)
                return false;
            item.Remove(count);
            Log.InfoFormat("[{0}] RemoveItem[{1}] removeCount: {2}",this.Owner.Data.ID,item,count);
            DBService.Instance.Save();
            return true;
        }

        public void GetItemInfos(List<NItemInfo> list)
        {
            foreach(var item in this.Items)
            {
                list.Add(new NItemInfo() { Id = item.Value.ItemID, Count = item.Value.Count });
            }
        }
    }
}
