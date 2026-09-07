using Managers;
using Models;
using Network;
using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Channels;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Services
{
    class TeamService : Singleton<TeamService>, IDisposable
    {
        public TeamService()
        {
            MessageDistributer.Instance.Subscribe<TeamInviteRequest>(this.OnTeamInviteRequest);
            MessageDistributer.Instance.Subscribe<TeamInviteResponse>(this.OnTeamInviteResponse);
            MessageDistributer.Instance.Subscribe<TeamInfoResponse>(this.OnTeamInfo);
            MessageDistributer.Instance.Subscribe<TeamLeaveResponse>(this.OnTeamLeave);

        }

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<TeamInviteRequest>(this.OnTeamInviteRequest);
            MessageDistributer.Instance.Unsubscribe<TeamInviteResponse>(this.OnTeamInviteResponse);
            MessageDistributer.Instance.Unsubscribe<TeamInfoResponse>(this.OnTeamInfo);
            MessageDistributer.Instance.Unsubscribe<TeamLeaveResponse>(this.OnTeamLeave);
        }
        //发送组队的邀请
        public void SendTeamInviteRequest(int friendId, string friendName)
        {
            Debug.Log("SendTeamInviteRequest");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.teamInviteReq = new TeamInviteRequest();
            message.Request.teamInviteReq.FromId = User.Instance.CurrentCharacter.Id;
            message.Request.teamInviteReq.FromName = User.Instance.CurrentCharacter.Name;
            message.Request.teamInviteReq.ToId = friendId;
            message.Request.teamInviteReq.ToName = friendName;
            NetClient.Instance.SendMessage(message);
        }
        //收到别人的组队邀请
        private void OnTeamInviteRequest(object sender, TeamInviteRequest request)
        {
            var mes = MessageBox.Show(string.Format("是否接受来自好友【{0}】的组队邀请",request.FromName),"组队请求",MessageBoxType.Confirm,"接受","拒绝");
            mes.OnYes = () =>
            {
                this.SendTeamInviteResponse(true, request);
            }; 
            mes.OnNo = () =>
            {
                this.SendTeamInviteResponse(false, request);
            }; 

        }
        //发送组队邀请的回应
        public void SendTeamInviteResponse(bool response,TeamInviteRequest request)
        {
            Debug.Log("SendTeamInviteResponse");
            NetMessage message = new NetMessage();
            message.Response = new NetMessageResponse();
            message.Response.teamInviteRes = new TeamInviteResponse();
            message.Response.teamInviteRes.Result = response ? Result.Success : Result.Failed;
            message.Response.teamInviteRes.Errormsg = response ? "组队成功了！" : "对方拒绝了你的组队请求";
            message.Response.teamInviteRes.Request = request;
            NetClient.Instance.SendMessage(message);
        }
        //请求方收到回应
        private void OnTeamInviteResponse(object sender, TeamInviteResponse message)
        {
            if (message.Result == Result.Success)
                MessageBox.Show(message.Request.ToName + "加入您的队伍", "组队成功");
            else
                MessageBox.Show(message.Errormsg, "组队失败");
        }
        
        //采取后处理的方式
        private void OnTeamInfo(object sender, TeamInfoResponse message)
        {
            Debug.Log("OnTeamInfo");
            TeamManager.Instance.UpdateTeamInfo(message.Team);
        }
        //发送离开队伍请求
        public void SendTeamLeaveRequest(int id)
        {
            Debug.Log("OnTeamLeaveRequest");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.teamLeave = new TeamLeaveRequest();
            message.Request.teamLeave.TeamId = id;
            message.Request.teamLeave.characterId = User.Instance.CurrentCharacter.Id;
            NetClient.Instance.SendMessage(message);
        }
        //离开队伍
        private void OnTeamLeave(object sender, TeamLeaveResponse message)
        {
            if (message.Result == Result.Success)
            {
                TeamManager.Instance.UpdateTeamInfo(null);
                MessageBox.Show("退出成功", "退出队伍");
            }
            else
                MessageBox.Show(message.Errormsg, "退出队伍", MessageBoxType.Error); 
        }

        public void Init()
        {
            
        }




    }
}
