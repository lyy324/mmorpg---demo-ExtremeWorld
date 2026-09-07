using Models;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIQuestDialog : UIWindow
{
    public UIQuestInfo questInfo;
    public Quest quest;
    public GameObject openButtons;
    public GameObject submitButtons;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SetQuest(Quest quest)
    {
        this.quest = quest;
        this.UpdateQuest();
        if (this.quest.info == null)
        {
            openButtons.SetActive(true);
            submitButtons.SetActive(false);
        }
        else
        {
            if(this.quest.info.Status == SkillBridge.Message.QuestStatus.Complated)
            {
                openButtons.SetActive(false);
                submitButtons.SetActive(true);
            }
            else
            {
                openButtons.SetActive(false);
                submitButtons.SetActive(false);
            }
        }

    }

    private void UpdateQuest()
    {
        if(this.quest != null)
        {
            if(this.questInfo != null)
            {
                this.questInfo.SetQuestInfo(quest);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
