using Common.Data;
using System;
using Services;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace Managers
{
    class ShopManager : Singleton<ShopManager>, IDisposable
    {
        public void Init()
        {
            NpcManager.Instance.RegisterNpcEvent(Common.Data.NpcFunction.InvokeShop,OnOpenShop);
        }

        private bool OnOpenShop(NpcDefine npc)
        {
            this.ShowShop(npc.Param);
            return true;
        }

        public void ShowShop(int shopId)
        {
            ShopDefine shop;
            if (DataManager.Instance.Shops.TryGetValue(shopId,out shop))
            {
                UIShop uiShop = UIManager.Instance.Show<UIShop>();
                if(uiShop != null)
                {
                    uiShop.SetShop(shop);
                }
            }
        }

        public bool BuyItem(int shopId,int shopItemId)
        {
            // Implementation for buying an item from the shop
            ItemService.Instance.SendBuyItem(shopId, shopItemId);
            return true;
        }

        public ShopManager()
        {

        }

        public void Dispose()
        {
        }

    }
}


