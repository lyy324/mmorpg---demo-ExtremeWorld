using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Common;
using Common.Data;
using GameServer.Services;
using Network;
using SkillBridge.Message;

namespace GameServer.Managers
{
    class ShopManager : Singleton<ShopManager>
    {
        public Result BuyItem(NetConnection<NetSession> sender, int shopId, int shopItemId)
        {
            if (!DataManager.Instance.Shops.ContainsKey(shopId))
                return Result.Failed;
            ShopItemDefine shopItem;
            if (DataManager.Instance.ShopItems[shopId].TryGetValue(shopItemId,out shopItem))
            {
                Log.InfoFormat("BuyItem: shopId{0} shopItemId{1} ItemID{2}", shopId, shopItemId, shopItem.ItemID);
                if(sender.Session.Character.Gold >= shopItem.Price)
                {
                    sender.Session.Character.ItemManager.AddItem(shopItem.ItemID, shopItem.Count);
                    sender.Session.Character.Gold -= shopItem.Price;
                    DBService.Instance.Save();
                    return Result.Success;
                }

            }
            return Result.Failed;
        }
    }
}
