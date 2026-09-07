using Managers;
using Services;
using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Runtime.CompilerServices;
using UnityEngine;

public class UIGuild : UIWindow
{
    public GameObject itemPrefab;
    public ListView listMain;
    public Transform itemRoot;
    public UIGuildInfo uiInfo;
    public UIGuildMemberItem selectedItem;
    public GameObject panelAdmin;
    public GameObject panelLeader;

    private void Start()
    {
        GuildService.Instance.OnGuildUpdate += UpdateUI;
        this.listMain.onItemSelected += this.OnGuildMemberSelected;
        this.UpdateUI();
    }
    private void OnDestroy()
    {
        GuildService.Instance.OnGuildUpdate -= UpdateUI;
    }
    private void OnGuildMemberSelected(ListView.ListViewItem item)
    {
        this.selectedItem = item as UIGuildMemberItem;
    }

    private void UpdateUI()
    {
        this.uiInfo.Info = GuildManager.Instance.guildInfo;
        ClearList();
        InitItems();

        this.panelAdmin.SetActive(GuildManager.Instance.myMemberInfo.Title > GuildTitle.None);
        this.panelLeader.SetActive(GuildManager.Instance.myMemberInfo.Title == GuildTitle.President);

    }
    private void ClearList()
    {
        this.listMain.RemoveAll();
    }
    private void InitItems()
    {
        foreach(var item in GuildManager.Instance.guildInfo.Members)
        {
            GameObject go = Instantiate(itemPrefab,this.listMain.transform);
            UIGuildMemberItem ui = go.GetComponent<UIGuildMemberItem>();
            ui.SetGuildMemberInfo(item);
            this.listMain.AddItem(ui);
        }
    }
    //显示申请信息
    public void OnClickAppliesList()
    {
        UIManager.Instance.Show<UIGuildApplyList>();
    }
    //离开公会
    public void OnClickLeave()
    {
        if (GuildManager.Instance.myMemberInfo.Title == GuildTitle.President)
        {
            MessageBox.Show("确定要解散公会吗", "解散公会", MessageBoxType.Confirm, "确认", "取消").OnYes = () =>
            {
                GuildService.Instance.SendGuildLeaveRequest();
            };
        }
        else
        {
            MessageBox.Show("是否要离开公会", "离开公会", MessageBoxType.Confirm, "确认", "取消").OnYes = () =>
            {

                GuildService.Instance.SendGuildLeaveRequest();
            };
        }

    }
    public void OnClickChat()
    {
        if (this.selectedItem == null || this.selectedItem.Info == null || this.selectedItem.Info.Info == null)
        {
            MessageBox.Show("请选择要私聊的公会成员");
            return;
        }

        ChatManager.Instance.StartPrivateChat(
            this.selectedItem.Info.Info.Id,
            this.selectedItem.Info.Info.Name
        );
    }
    //踢出成员
    public void OnClickKickout()
    {
        //if (selectedItem.Info.Title >= GuildManager.Instance.myMemberInfo.Title)
        //{
        //    MessageBox.Show("不能踢出不低于自己职位的成员");
        //    return;
        //}
        if (selectedItem == null)
        {
            MessageBox.Show("请选择要踢出的成员");
            return;
        }
        MessageBox.Show(string.Format("要踢【{0}】出公会吗", this.selectedItem.Info.Info.Name), "踢出公会", MessageBoxType.Confirm,"确认","取消").OnYes = () =>
        {
            GuildService.Instance.SendAdiminCommand(GuildAdminCommand.Kickout,this.selectedItem.Info.Info.Id);
        };
    }
    //晋升成员的职位
    public void OnClickPromote()
    {
        if (selectedItem.Info.Title >= GuildManager.Instance.myMemberInfo.Title)
        {
            MessageBox.Show("不能晋升不低于自己职位的成员");
            return;
        }
        if (selectedItem == null)
        {
            MessageBox.Show("请选择要晋升的成员");
            return ;
        }
        if(selectedItem.Info.Title != GuildTitle.None)
        {
            MessageBox.Show("对方已经身份高贵");
            return;
        } 
        MessageBox.Show(string.Format("要晋升【{0}】为副会长吗",this.selectedItem.Info.Info.Name),"晋升",MessageBoxType.Confirm,"确认","取消").OnYes = () =>
        {
            GuildService.Instance.SendAdiminCommand(GuildAdminCommand.Promote, this.selectedItem.Info.Info.Id);
        };
    }
    //罢免成员的职位
    public void OnClickDepose()
    {
        if (selectedItem.Info.Title >= GuildManager.Instance.myMemberInfo.Title)
        {
            MessageBox.Show("不能罢免不低于自己职位高的成员");
            return;
        }
        if (selectedItem == null)
        {
            MessageBox.Show("请选择要罢免的成员");
            return;
        }
        if(selectedItem.Info.Title  == GuildTitle.None)
        {
            MessageBox.Show("对方已经是最低的职位了");
            return;
        }
        if(selectedItem.Info.Title  == GuildTitle.President)
        {
            MessageBox.Show("会长不是你能动的");
            return;
        }
        MessageBox.Show(string.Format("要罢免【{0}】吗?", this.selectedItem.Info.Info.Name), "罢免职务", MessageBoxType.Confirm, "确认", "取消").OnYes = () =>
        {
            GuildService.Instance.SendAdiminCommand(GuildAdminCommand.Depost, this.selectedItem.Info.Info.Id);
        };
    }
    //转让公会会长的身份给别人
    public void OnClickTransfer()
    {
        if (selectedItem.Info.Title == GuildTitle.President)
        {
            return;
        }
        if (selectedItem == null)
        {
            MessageBox.Show("请选择把会长让给的成员");
            return ;
        }
        MessageBox.Show(string.Format("确定要把会长转给【{0}】吗？", selectedItem.Info.Info.Name), "转让公会", MessageBoxType.Confirm, "确认", "取消").OnYes = () =>
        {
            GuildService.Instance.SendAdiminCommand(GuildAdminCommand.Transfer, this.selectedItem.Info.Info.Id);
        };
    }
    //选择修改组织的宣言
    public void OnClickSetNotice()
    {

    }

}
