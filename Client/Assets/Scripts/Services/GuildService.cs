using Network;
using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;

namespace Services
{

    class GuildService : Singleton<GuildService>, IDisposable
    {
        //创建触发 传到 创建界面 创建公会
        public UnityAction<bool> OnGuildCreateResult;
        //请求公会列表信息触发 传到加入公会界面 更新加入公会界面 公会列表 
        public UnityAction<List<NGuildInfo>> OnGuildListResult;
        //请求已有公会信息触发 传到公会界面 更新公会信息
        public UnityAction OnGuildUpdate;
        public GuildService()
        {
            MessageDistributer.Instance.Subscribe<GuildCreateResponse>(this.OnGuildCreate);
            MessageDistributer.Instance.Subscribe<GuildListResponse>(this.OnGuildList);
            MessageDistributer.Instance.Subscribe<GuildJoinRequest>(this.OnGuildJoinRequest);
            MessageDistributer.Instance.Subscribe<GuildJoinResponse>(this.OnGuildJoinResponse);
            MessageDistributer.Instance.Subscribe<GuildResponse>(this.OnGuild);
            MessageDistributer.Instance.Subscribe<GuildLeaveResponse>(this.OnGuildLeave);
            MessageDistributer.Instance.Subscribe<GuildAdminResponse>(this.OnGuildAdmin);
        }
        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<GuildCreateResponse>(this.OnGuildCreate);
            MessageDistributer.Instance.Unsubscribe<GuildListResponse>(this.OnGuildList);
            MessageDistributer.Instance.Unsubscribe<GuildJoinRequest>(this.OnGuildJoinRequest);
            MessageDistributer.Instance.Unsubscribe<GuildJoinResponse>(this.OnGuildJoinResponse);
            MessageDistributer.Instance.Unsubscribe<GuildResponse>(this.OnGuild);
            MessageDistributer.Instance.Unsubscribe<GuildLeaveResponse>(this.OnGuildLeave);
            MessageDistributer.Instance.Unsubscribe<GuildAdminResponse>(this.OnGuildAdmin);


        }

        /// <summary>
        /// 发送创建公会的信息
        /// </summary>
        /// <param name="guildName"></param>
        /// <param name="notice"></param>
        public void SendGuildCreate(string guildName,string notice)
        {
            Debug.Log("SendGuildCreate");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.guildCreate = new GuildCreateRequest();
            message.Request.guildCreate.GuildName = guildName;
            message.Request.guildCreate.GuildNotice = notice;
            NetClient.Instance.SendMessage(message);
        }
        /// <summary>
        /// 收到公会创建的响应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        private void OnGuildCreate(object sender, GuildCreateResponse response)
        {
            Debug.LogFormat("OnGuildCreateResponse : {0}" ,response.Result);
            if(OnGuildCreateResult != null)
            {
                this.OnGuildCreateResult(response.Result == Result.Success);
            }
            if(response.Result == Result.Success)
            {
                GuildManager.Instance.Init(response.guildInfo);
                MessageBox.Show(string.Format("{0} 工会创建成功",response.guildInfo.GuildName),"公会");
                return;
            }
                MessageBox.Show(string.Format("{0} 工会创建失败", response.guildInfo.GuildName), "公会");
        }
        /// <summary>
        /// 发送请求列表
        /// </summary>
        public void SendGuildListRequest()
        {
            Debug.Log("SendGuildListRequest");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.guildList = new GuildListRequest();
            NetClient.Instance.SendMessage(message);
        }
        /// <summary>
        /// 收到列表刷新请求
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void OnGuildList(object sender, GuildListResponse response)
        {
            if(OnGuildListResult != null)
            {
                this.OnGuildListResult(response.Guilds);
            }
        }


        /// <summary>
        /// 发送加入公会的请求
        /// </summary>
        /// <param name="guildId"></param>
        public void SendGuildJoinRequest(int guildId)
        {
            Debug.LogFormat("SendGuildJoinRequest");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.guildJoinReq = new GuildJoinRequest();
            message.Request.guildJoinReq.Apply = new NGuildApplyInfo();
            //这里只填 公会Id 是因为省消息 服务器有对应信息
            message.Request.guildJoinReq.Apply.GuildId = guildId;
            NetClient.Instance.SendMessage(message);
        }
        /// <summary>
        /// 收到请求加入消息 处理后回应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="request"></param>
        private void OnGuildJoinRequest(object sender, GuildJoinRequest request)
        {
            var confirm = MessageBox.Show(string.Format("{0} 申请加入公会",request.Apply.Name),"公会申请",MessageBoxType.Confirm,"允许","拒绝");
            confirm.OnYes = () =>
            {
                this.SendGuildJoinResponse(true,request);
            };
            confirm.OnNo = () =>
            {
                this.SendGuildJoinResponse(false, request);
            };
        }
        /// <summary>
        /// 返回请求处理
        /// </summary>
        /// <param name="accept"></param>
        /// <param name="request"></param>
        public void SendGuildJoinResponse(bool accept,GuildJoinRequest request)
        {
            Debug.LogFormat("SendGuildJoinResponse");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.guildJoinRes = new GuildJoinResponse();
            message.Request.guildJoinRes.Result = Result.Success;
            //这里只填 公会Id 是因为省消息 服务器有对应信息
            message.Request.guildJoinRes.Apply = request.Apply;
            message.Request.guildJoinRes.Apply.Result = accept ? ApplyResult.Accept : ApplyResult.Reject;
            NetClient.Instance.SendMessage(message);
        }
        /// <summary>
        /// 收到加入公会响应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        private void OnGuildJoinResponse(object sender, GuildJoinResponse response)
        {
            Debug.LogFormat("OnGuildJoinResponse : {0}",response.Result);
            if(response.Result == Result.Success)
            {
                MessageBox.Show("加入公会成功","公会");
            }
            else
            {
                MessageBox.Show("加入公会失败", "公会");
            }
        }

        /// <summary>
        /// 公会信息刷新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        private void OnGuild(object sender, GuildResponse message)
        {
            Debug.LogFormat("OnGuild: {0} {1} : {2}", message.Result, message.guildInfo.Id, message.guildInfo.GuildName);
            GuildManager.Instance.Init(message.guildInfo);
            if (this.OnGuildUpdate != null)
                this.OnGuildUpdate();
        }
        /// <summary>
        /// 发送离开公会请求
        /// </summary>
        public void SendGuildLeaveRequest()
        {
            Debug.Log("SendFriendRemoveRequest");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.guildLeave = new GuildLeaveRequest();
            NetClient.Instance.SendMessage(message);
        }
        /// <summary>
        /// 收到离开公会的回应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        private void OnGuildLeave(object sender, GuildLeaveResponse message)
        {
            if (message.Result == Result.Success)
            {
                GuildManager.Instance.Init(null);
                MessageBox.Show("离开公会成功", "公会");
            }
            else
                MessageBox.Show("离开公会失败", "公会");
        }

        public void Init()
        {

        }
        //发送加入公会审批
        public void SendGuildJoinApply(bool accept,NGuildApplyInfo apply)
        {
            Debug.Log("SendGuildJoinResponse");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.guildJoinRes = new GuildJoinResponse();
            message.Request.guildJoinRes.Result = Result.Success;
            message.Request.guildJoinRes.Apply = apply;
            message.Request.guildJoinRes.Apply.Result = accept ? ApplyResult.Accept : ApplyResult.Reject;
            NetClient.Instance.SendMessage(message);
        }
        //发送管理指令
        public void SendAdiminCommand(GuildAdminCommand command, int characterId)
        {
            Debug.Log("SendAdminCommand");
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.guildAdmin = new GuildAdminRequest();
            message.Request.guildAdmin.Command = command;
            message.Request.guildAdmin.Target = characterId;
            NetClient.Instance.SendMessage(message);

        }
        /// <summary>
        /// 收到公会管理信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        private void OnGuildAdmin(object sender, GuildAdminResponse message)
        {
            Debug.LogFormat("GuildAdmin: {0} {1}",message.Command,message.Result);
            MessageBox.Show(string.Format("执行操作 {0} 结果 : {1} {2}", message.Command, message.Result, message.Errormsg));
        }


    }
}
