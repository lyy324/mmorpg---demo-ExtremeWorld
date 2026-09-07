using Common.Data;
using Managers;
using Models;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIShop : UIWindow
{
    public Text title;
    public Text money;
    public Text page;
    public Transform[] itemRoot;
    public GameObject shopItem;
    ShopDefine shop;

    // Keep the existing item container and add paging on top of it.
    private const int ItemsPerPage = 8;
    private readonly List<KeyValuePair<int, ShopItemDefine>> availableItems = new List<KeyValuePair<int, ShopItemDefine>>();
    private int currentPage;
    private int totalPages = 1;


    private void Awake()
    {
        BindPageButtons();
    }

    // Start is called before the first frame update
    void Start()
    {
        User.Instance.GoldChange = this.UpDateMoney;
    }
    void UpDateMoney(int money)
    {
        this.money.text = money.ToString();
    }
    

    IEnumerator InitItems()
    {
        availableItems.Clear();
        currentPage = 0;
        if (shop == null || !DataManager.Instance.ShopItems.ContainsKey(shop.ID))
        {
            totalPages = 1;
            RefreshPage();
            yield break;
        }

        foreach(var kv in DataManager.Instance.ShopItems[shop.ID])
        {
            if(kv.Value.Status > 0)
                availableItems.Add(new KeyValuePair<int, ShopItemDefine>(kv.Key, kv.Value));
        }
        totalPages = Mathf.Max(1, Mathf.CeilToInt((float)availableItems.Count / ItemsPerPage));
        RefreshPage();
        yield return null;
    }

    private void RefreshPage()
    {
        if (itemRoot == null || itemRoot.Length == 0 || itemRoot[0] == null)
            return;

        selectedItem = null;
        for (int i = itemRoot[0].childCount - 1; i >= 0; i--)
            Destroy(itemRoot[0].GetChild(i).gameObject);

        int start = currentPage * ItemsPerPage;
        int end = Mathf.Min(start + ItemsPerPage, availableItems.Count);
        for (int i = start; i < end; i++)
        {
            KeyValuePair<int, ShopItemDefine> kv = availableItems[i];
            GameObject go = Instantiate(shopItem, itemRoot[0]);
            UIShopItem ui = go.GetComponent<UIShopItem>();
            ui.SetShopItem(kv.Key, kv.Value, this);
        }

        if (page != null)
            page.text = string.Format("第{0}页", currentPage + 1);
    }

    private void BindPageButtons()
    {
        Button[] buttons = GetComponentsInChildren<Button>(true);
        foreach (Button button in buttons)
        {
            if (button.name == "TabButton1")
            {
                TabButton tabButton = button.GetComponent<TabButton>();
                if (tabButton != null)
                    tabButton.enabled = false;
                button.onClick.AddListener(OnClickPreviousPage);
            }
            else if (button.name == "TabButton2")
            {
                TabButton tabButton = button.GetComponent<TabButton>();
                if (tabButton != null)
                    tabButton.enabled = false;
                button.onClick.AddListener(OnClickNextPage);
            }
        }
    }

    public void OnClickPreviousPage()
    {
        if (currentPage <= 0)
            return;
        currentPage--;
        RefreshPage();
    }

    public void OnClickNextPage()
    {
        if (currentPage + 1 >= totalPages)
            return;
        currentPage++;
        RefreshPage();
    }

    public void SetShop(ShopDefine shop)
    {
        this.shop = shop;
        this.title.text = shop.Name;
        this.money.text = User.Instance.CurrentCharacter.Gold.ToString();
        StartCoroutine(InitItems());
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    private UIShopItem selectedItem;
    internal void SelectShopItem(UIShopItem item)
    {
        if(selectedItem != null)
            selectedItem.Selected = false;
        selectedItem = item;
    }

    public void OnClickBuy()
    {
        if (selectedItem == null)
        {
            MessageBox.Show("请选择要购买的道具", "购买提示");
            return;
        }
        if(!ShopManager.Instance.BuyItem(this.shop.ID,this.selectedItem.ShopItemID))
        {
            
        }
    }
}
