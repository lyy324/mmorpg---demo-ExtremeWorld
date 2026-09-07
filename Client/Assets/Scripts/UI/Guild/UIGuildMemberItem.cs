using Common.Utils;
using SkillBridge.Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGuildMemberItem : ListView.ListViewItem
{
    public Text nickname;
    public Text @class;
    public Text level;
    public Text title;
    public Text joinTime;
    public Text status;

    public Image background;
    public Sprite normalBg;
    public Sprite selectedBg;

    public override void onSelected(bool selected)
    {
        this.background.overrideSprite =  selected ? selectedBg : normalBg;
    }
    public NGuildMemberInfo Info;

    public void SetGuildMemberInfo(NGuildMemberInfo item)
    {
        this.Info = item;
        if (this.nickname != null) this.nickname.text = this.Info.Info.Name;
        if(this.@class != null) this.@class.text = this.Info.Info.Class.ToString();
        if(this.level != null) this.level.text = this.Info.Info.Level.ToString();
        if(this.joinTime != null) this.joinTime.text = TimeUtil.GetTime(this.Info.joinTime).ToShortDateString();
        if(this.status != null) this.status.text = this.Info.Status == 1 ? "在线" : TimeUtil.GetTime(this.Info.lastTime).ToShortDateString();
        if(this.title != null)
        {
            if (item.Title == GuildTitle.President)
                title.text = "会长";
            if (item.Title == GuildTitle.None)
                title.text = "成员";
            if (item.Title == GuildTitle.VicePresident)
                title.text = "副会长";
        }
    }
}
