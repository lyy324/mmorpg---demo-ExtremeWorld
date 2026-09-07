using Common.Data;
using JetBrains.Annotations;
using Models;
using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class UIQuestInfo : MonoBehaviour
{
    public Text title;
    public Text[] targets;
    public Transform[] rewardItems;
    public Text description;
    public GameObject rewardItem;
    public Text rewardMoney;
    public Text rewardExp;

    internal void SetQuestInfo(Quest quest)
    {
        this.title.text = string.Format("[{0}]{1}", quest.Define.Type, quest.Define.Name);
        SetQuestDescription(quest);
        for(int i = 0;i < 3;i ++)
        {
            targets[i].gameObject.SetActive(false);
            rewardItems[i].gameObject.SetActive(false);
            if (rewardItems[i].transform.childCount > 0)
                Destroy(rewardItems[i].transform.GetChild(0).gameObject);
        }
        //设置 任务目标
        SetQuestTarget(quest.Define, quest.info);

        SetQuestRewardItems(quest.Define);

        this.rewardMoney.text = quest.Define.RewardGold.ToString();
        this.rewardExp.text = quest.Define.RewardExp.ToString();

        foreach (var fitter in this.GetComponentsInChildren<ContentSizeFitter>())
        {
            fitter.SetLayoutVertical();
        }
    }

    void SetQuestDescription(Quest quest)
    {
        if (quest.info == null)
        {
            this.description.text = quest.Define.Dialog;
            return;
        }

        if (quest.info.Status == QuestStatus.Complated || quest.info.Status == QuestStatus.Finished)
        {
            this.description.text = quest.Define.DialogFinish;
            return;
        }

        this.description.text = string.IsNullOrEmpty(quest.Define.DialogIncomplete)
            ? quest.Define.Overview
            : quest.Define.DialogIncomplete;
    }

    void SetQuestTarget(QuestDefine quest, NQuestInfo info)
    {
        QuestTarget[] targetTypes = { quest.Target1, quest.Target2, quest.Target3 };
        int[] targetIds = { quest.Target1ID, quest.Target2ID, quest.Target3ID };
        int[] targetCounts = { quest.Target1Num, quest.Target2Num, quest.Target3Num };

        for (int i = 0; i < targetTypes.Length; i++)
        {
            if (targetTypes[i] == QuestTarget.None)
                continue;

            int current = info != null && info.Targets != null && info.Targets.Length > i
                ? info.Targets[i]
                : 0;
            targets[i].gameObject.SetActive(true);
            targets[i].text = string.Format("{0}{1} {2}/{3}",
                GetTargetAction(targetTypes[i]),
                GetTargetName(targetTypes[i], targetIds[i]),
                current,
                targetCounts[i]);
        }
    }

    string GetTargetAction(QuestTarget target)
    {
        if (target == QuestTarget.Kill)
            return "击杀";
        if (target == QuestTarget.Item)
            return "收集";
        return "";
    }

    string GetTargetName(QuestTarget target, int targetId)
    {
        if (target == QuestTarget.Kill)
        {
            CharacterDefine character;
            if (DataManager.Instance.Characters != null &&
                DataManager.Instance.Characters.TryGetValue(targetId, out character))
                return character.Name;
            return string.Format("怪物({0})", targetId);
        }

        if (target == QuestTarget.Item)
        {
            ItemDefine item;
            if (DataManager.Instance.Items != null &&
                DataManager.Instance.Items.TryGetValue(targetId, out item))
                return item.Name;
            return string.Format("物品({0})", targetId);
        }

        return string.Format("目标({0})", targetId);
    }
    void SetQuestRewardItems(QuestDefine quest)
    {
        ItemDefine RewardItem;
        if (quest.RewardItem1 <= 0)
            return;
        if(DataManager.Instance.Items.TryGetValue(quest.RewardItem1, out RewardItem))
        {
            GameObject go = Instantiate(this.rewardItem, rewardItems[0]);
            go.GetComponent<UIIconItem>().SetMainIcon(RewardItem.Icon, quest.RewardItem1Count.ToString());
            this.rewardItems[0].gameObject.SetActive(true);
        }

        if (quest.RewardItem2 <= 0)
            return;
        if(DataManager.Instance.Items.TryGetValue(quest.RewardItem2, out RewardItem))
        {
            GameObject go = Instantiate(this.rewardItem, rewardItems[1]);
            go.GetComponent<UIIconItem>().SetMainIcon(RewardItem.Icon, quest.RewardItem2Count.ToString());
            this.rewardItems[2].gameObject.SetActive(true);
        }

        if (quest.RewardItem3 <= 0)
            return;
        if(DataManager.Instance.Items.TryGetValue(quest.RewardItem3, out RewardItem))
        {
            GameObject go = Instantiate(this.rewardItem, rewardItems[2]);
            go.GetComponent<UIIconItem>().SetMainIcon(RewardItem.Icon, quest.RewardItem3Count.ToString());
            this.rewardItems[3].gameObject.SetActive(true);
        }


    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
