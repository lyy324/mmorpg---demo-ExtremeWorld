using SkillBridge.Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITeamItem : ListView.ListViewItem
{
    public Text nickname;
    public Image classIcon;
    public Image leaderIcon;
    public Image background;
    public Sprite normalBg;
    public Sprite selectedBg;

    public override void onSelected(bool selected)
    {
        this.background.overrideSprite = selected ? selectedBg : normalBg;
    }

    public int idx;
    public NCharacterInfo Info;
    void Start()
    {
        this.background.overrideSprite = normalBg;
    }
    /// <summary>
    /// 初始化成员的信息
    /// </summary>
    /// <param name="idx">队伍的 id</param>
    /// <param name="item"> 角色的 信息</param>
    /// <param name="isLeader">是否为 队长</param>
    public void SetMemberInfo(int idx,NCharacterInfo item,bool isLeader)
    {
        this.idx = idx;
        this.Info = item;
        if(this.nickname != null) this.nickname.text = this.Info.Level.ToString() + " " + this.Info.Name;
        if (this.classIcon != null) this.classIcon.overrideSprite = SpriteManager.Instance.classIcons[(int)this.Info.Class - 1];
        if (this.leaderIcon != null) this.leaderIcon.gameObject.SetActive(isLeader);

    }
}
