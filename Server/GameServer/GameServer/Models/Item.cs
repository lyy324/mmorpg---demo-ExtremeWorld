using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Models
{
    //这里不直接使用数据库的原因是 不应该频繁操作数据库 当角色登录后先把道具拉出来在
    //内存中存一份 这样通讯时直接读内存使用 不用频繁调用数据库
    class Item
    {
        TCharacterItem dbItem;

        public int ItemID;

        public int Count;

        public Item(TCharacterItem item)
        {
            this.dbItem = item;

            this.ItemID = (short)item.ItemID;
            this.Count = (short)item.ItemCount;

        }

        public void Add(int count)
        {
            this.Count += count;
            dbItem.ItemCount = this.Count;
        }

        public void Remove(int count)
        {
            this.Count -= count;
            dbItem.ItemCount = this.Count;
        }
        public bool Use(int count = 1)
        {
            return false;
        }

        public override string ToString()
        {
            return string.Format("ID: {0}, Count: {1}", this.ItemID, this.Count);
        }
    }
}
