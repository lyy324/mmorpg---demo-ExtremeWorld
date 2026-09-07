using Common.Data;
using Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class UIQuestSystem : UIWindow
{
    public Text title;
    public GameObject itemPrefab;
    public TabView Tabs;
    public ListView listMain;
    public ListView ListBranch;

    public UIQuestInfo questInfo;
    private bool showAvailableList = false;
    // Start is called before the first frame update
    void Start()
    {
        this.listMain.onItemSelected += this.OnQuestSelected;
        this.ListBranch.onItemSelected += this.OnQuestSelected;
        this.Tabs.OnTabSelect += this.OnSelectTab;
        RefreshUI();
    }

    private void RefreshUI()
    {
        ClearAllQuestList();
        InitAllQuestItems();
    }

    /// <summary>
    /// 初始化所有的装备列表
    /// </summary>
    private void InitAllQuestItems()
    {
        foreach (var kv in QuestManager.Instance.allQuests)
        {
            if(showAvailableList)
            {
                if (kv.Value.info != null)
                    continue;
            }
            else
            {
                if(kv.Value.info == null || kv.Value.info.Status == SkillBridge.Message.QuestStatus.Finished)
                    continue;
            }

            GameObject go = Instantiate(itemPrefab, kv.Value.Define.Type == QuestType.Main ? this.listMain.transform : this.ListBranch.transform);
            UIQuestItem ui = go.GetComponent<UIQuestItem>();
            ui.SetQuestInfo(kv.Value);
            if (kv.Value.Define.Type == QuestType.Main)
                this.listMain.AddItem(ui as ListView.ListViewItem);
            else
                this.ListBranch.AddItem(ui as ListView.ListViewItem);
        }

    }

    private void ClearAllQuestList()
    {
        this.listMain.RemoveAll();
        this.ListBranch.RemoveAll();
    }

    void OnSelectTab(int idx)
    {
        showAvailableList = idx == 1;
        RefreshUI();
    }
    private void OnQuestSelected(ListView.ListViewItem item)
    {
        if(item.owner == listMain)
            ListBranch.UnSelectAllItems();
        if (item.owner == ListBranch)
            listMain.UnSelectAllItems();
        UIQuestItem questItem = item as UIQuestItem;
        this.questInfo.SetQuestInfo(questItem.quest);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
