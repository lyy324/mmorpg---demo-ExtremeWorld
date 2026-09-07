using Managers;
using Models;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SkillBridge.Message;
using System.Globalization;
using JetBrains.Annotations;
using UnityEngine.Audio;

public class UICharEquip : UIWindow
{
    public Text title;

    public GameObject itemPrefab;
    public GameObject charEquipedPrefab;
    public Transform itemListRoot;
    public List<Transform> slots = new List<Transform>();
    private UICharacterView characterView;

    
    // Start is called before the first frame update
    void Start()
    {
        RefreshUI();
        EquipManager.Instance.OnEquipChanged += RefreshUI;
    }
    private void OnDestroy()
    {
        EquipManager.Instance.OnEquipChanged -= RefreshUI;
    }

    private void RefreshUI()
    {
        UpdateCharacterView();
        ClearAllEquipList();
        InitAllEquipItems();
        ClearEquipedList();
        InitEquipedItems();
        
    }

    private void UpdateCharacterView()
    {
        if (this.characterView == null)
            this.characterView = GetComponentInChildren<UICharacterView>(true);

        if (this.characterView != null && User.Instance.CurrentCharacter != null)
            this.characterView.CurrectCharacter = (int)User.Instance.CurrentCharacter.Class - 1;
    }

    //清空左侧的装备列表
    private void ClearAllEquipList()
    {
        for(int i = 0;i < itemListRoot.childCount;i ++)
        {
            Destroy(itemListRoot.GetChild(i).gameObject);
        }
    }

    private void InitAllEquipItems()
    {
        if (User.Instance.CurrentCharacter == null)
            return;

        string currentClass = User.Instance.CurrentCharacter.Class.ToString();
        foreach(var kv in ItemManager.Instance.Items)
        {
            if(kv.Value.Define.Type == ItemType.Equip)
            {
                string limitClass = kv.Value.Define.LimitClass;
                if (!string.IsNullOrEmpty(limitClass) &&
                    !string.Equals(limitClass, currentClass, StringComparison.OrdinalIgnoreCase) &&
                    !string.Equals(limitClass, "ALL", StringComparison.OrdinalIgnoreCase))
                    continue;

                if (EquipManager.Instance.Contains(kv.Value.Id))
                    continue;
                GameObject go = Instantiate(itemPrefab, itemListRoot);
                UIEquipItem ui = go.GetComponent<UIEquipItem>();
                ui.SetEquipItem(kv.Key, kv.Value, this, false);
            }
        }
    }

    private void ClearEquipedList()
    {
        foreach(var item in slots)
        {
            if(item.childCount > 0)
                Destroy(item.GetChild(0).gameObject);
        }
    }
    private void InitEquipedItems()
    {
        
        for(int i = 0;i < (int)EquipSlot.SlotMax;i ++)
        {
            var Item = EquipManager.Instance.Equips[i];
            {
                if (Item != null)
                {
                    GameObject go = Instantiate(charEquipedPrefab, slots[i]);
                    UIEquipItem ui = go.GetComponent<UIEquipItem>();
                    ui.SetEquipItem(i, Item, this, true);

                }
            }
                
        }
    }

    public void DoEquip(Item item)
    {
        EquipManager.Instance.EquipItem(item);
    }

    public void UpEquip(Item item)
    {
        EquipManager.Instance.UnEquipItem(item);
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
