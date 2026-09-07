using Common.Utils;
using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIGuildItem : ListView.ListViewItem
{
    public Text ID;
    public Text guildName;
    public Text memberCount;
    public Text leader;

    public Image background;
    public Sprite normalBg;
    public Sprite selectedBg;

    public override void onSelected(bool selected)
    {
        this.background.overrideSprite = selected ? selectedBg : normalBg;
    }
    public NGuildInfo Info;

    public void SetGuildInfo(NGuildInfo item)
    {
        this.Info = item;
        this.ID.text = this.Info.Id.ToString();
        this.guildName.text = this.Info.GuildName.ToString();
        this.memberCount.text = this.Info.memberCount.ToString();
        this.leader.text = this.Info.leaderName.ToString();
    }
}
