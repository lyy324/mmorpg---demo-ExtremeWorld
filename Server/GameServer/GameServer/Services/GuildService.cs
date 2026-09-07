using Common;
using GameServer.Entities;
using GameServer.Managers;
using Network;
using SkillBridge.Message;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Net.Configuration;
using System.Runtime.CompilerServices;
using System.Security.Authentication.ExtendedProtection;
using System.Security.Cryptography.Xml;
using System.ServiceModel.Channels;
using System.ServiceModel.Security;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace GameServer.Services
{
    class GuildService : Singleton<GuildService>
    {
        public GuildService()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<GuildCreateRequest>(this.OnGuildCreate);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<GuildListRequest>(this.OnGuildList);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<GuildJoinRequest>(this.OnGuildJoinRequest);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<GuildJoinResponse>(this.OnGuildJoinResponse);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<GuildLeaveRequest>(this.OnGuildLeave);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<GuildAdminRequest>(this.OnGuildAdmin);
        }
        /// <summary>
        /// 处理管理信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        private void OnGuildAdmin(NetConnection<NetSession> sender, GuildAdminRequest message)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnGuildLeave: :Character{0}",character.Id);
            TGuildMember DBcharacter = DBService.Instance.Entities.GuildMembers.FirstOrDefault(v => v.Id == character.Id);
            sender.Session.Response.guildAdmin = new GuildAdminResponse();
            if(character.Guild == null)
            {
                sender.Session.Response.guildAdmin.Result = Result.Failed;
                sender.Session.Response.guildAdmin.Errormsg = "你没公会不要乱来";
                sender.SendResponse();
                return;
            }
            if (DBcharacter != null && DBcharacter.Title == (int)GuildTitle.None)
            {
                sender.Session.Response.guildAdmin.Result = Result.Failed;
                sender.Session.Response.guildAdmin.Errormsg = "你还没有权限修改";
                sender.SendResponse();
                return;
            }
            character.Guild.ExecuteAdmin(message.Command, message.Target, character.Id);

            //目标在不在线
            var target = SessionManager.Instance.GetSession(message.Target);
            if(target != null)
            {
                target.Session.Response.guildAdmin = new GuildAdminResponse();
                target.Session.Response.guildAdmin.Result = Result.Success;
                target.Session.Response.guildAdmin.Command = message;
                target.SendResponse();
            }

            sender.Session.Response.guildAdmin.Result = Result.Success;
            sender.Session.Response.guildAdmin.Command = message;
            sender.SendResponse();
        }

        //创建公会
        private void OnGuildCreate(NetConnection<NetSession> sender, GuildCreateRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnGuildCreate : GuildName {0} Character {1} {2}", request.GuildName, character.Id, character.Name);
            sender.Session.Response.guildCreate = new GuildCreateResponse();
            if(character.Guild != null)
            {
                sender.Session.Response.guildCreate.Result = Result.Failed;
                sender.Session.Response.guildCreate.Errormsg = "已经有公会了";
                sender.SendResponse();
                return;
            }
            if(GuildManager.Instance.CheckNameExisted(request.GuildName))
            {
                sender.Session.Response.guildCreate.Result = Result.Failed;
                sender.Session.Response.guildCreate.Errormsg = "公会名称已被注册";
                sender.SendResponse();
                return;
            }
            GuildManager.Instance.CreateGuild(request.GuildName,request.GuildNotice,character);
            sender.Session.Response.guildCreate.guildInfo = character.Guild.GuildInfo(character);
            sender.Session.Response.guildCreate.Result = Result.Success;
            sender.SendResponse();
        }
        /// <summary>
        /// 返回公会列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="request"></param>
        private void OnGuildList(NetConnection<NetSession> sender, GuildListRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnGuildList : character {0} {1}",character.Id,character.Name);
            sender.Session.Response.guildList = new GuildListResponse();
            sender.Session.Response.guildList.Guilds.AddRange(GuildManager.Instance.GetGuildsInfo());
            sender.Session.Response.guildList.Result = Result.Success;
            sender.SendResponse();
        }

        private void OnGuildJoinRequest(NetConnection<NetSession> sender, GuildJoinRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnGuildJoinRequest: : GuildId:{0} characterId:{1} {2}",request.Apply.GuildId,request.Apply.characterId,request.Apply.Name);
            var guild = GuildManager.Instance.GetGuild(request.Apply.GuildId);
            if(guild == null)
            {
                sender.Session.Response.guildJoinRes = new GuildJoinResponse();
                sender.Session.Response.guildJoinRes.Result = Result.Failed;
                sender.Session.Response.guildJoinRes.Errormsg = "公会不存在";
                sender.SendResponse();
                return;
            }
            request.Apply.characterId = character.Data.ID;
            request.Apply.Name = character.Data.Name;
            request.Apply.Class = character.Data.Class;
            request.Apply.Level = character.Data.Level;

            if(guild.JoinApply(request.Apply))
            {
                var leader = SessionManager.Instance.GetSession(guild.Data.LeaderID);
                if(leader != null)
                {//给会长发送申请加入请求
                    leader.Session.Response.guildJoinReq = request;
                    leader.SendResponse();
                }
            }
            else
            {
                sender.Session.Response.guildJoinRes = new GuildJoinResponse();
                sender.Session.Response.guildJoinRes.Result = Result.Failed;
                sender.Session.Response.guildJoinRes.Errormsg = "已经请求过了";
                sender.SendResponse();
            }
        }
        /// <summary>
        /// 收到公会响应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="response"></param>
        private void OnGuildJoinResponse(NetConnection<NetSession> sender, GuildJoinResponse response)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnGuildJoinResponse : : GuildId :{0} CharacterId : {1}",response.Apply.GuildId,response.Apply.characterId);

            var guild = GuildManager.Instance.GetGuild(response.Apply.GuildId);
            if (response.Result == Result.Success)
            {//接受了公会请求
                guild.JoinAppove(response.Apply);
            }
            var requester = SessionManager.Instance.GetSession(response.Apply.characterId);
            if(requester != null)
            {
                requester.Session.Character.Guild = guild; 

                requester.Session.Response.guildJoinRes  = response;
                requester.Session.Response.guildJoinRes.Result = Result.Success;
                requester.Session.Response.guildJoinRes.Errormsg = "加入公会成功";
                requester.SendResponse();
            }

        }
        //离开公会
        private void OnGuildLeave(NetConnection<NetSession> sender, GuildLeaveRequest request)
        {
            Character character = sender.Session.Character;

            sender.Session.Response.guildLeave = new GuildLeaveResponse();

            if (character == null || character.Guild == null)
            {
                sender.Session.Response.guildLeave.Result = Result.Failed;
                sender.Session.Response.guildLeave.Errormsg = "你当前没有公会";
                sender.SendResponse();
                return;
            }

            Log.InfoFormat("OnGuildLeave: :character: {0}", character.Id);

            var guild = character.Guild;
            var memberIds = new List<int>();

            if (character.Id == guild.Data.LeaderID)
            {
                memberIds = guild.Disband();
                GuildManager.Instance.DisbandGuild(guild.Data);
            }
            else if (!guild.Leave(character.Id))
            {
                sender.Session.Response.guildLeave.Result = Result.Failed;
                sender.Session.Response.guildLeave.Errormsg = "你不是该公会成员";
                sender.SendResponse();
                return;
            }
            else
            {
                memberIds.Add(character.Id);
            }

            DBService.Instance.Save();

            sender.Session.Response.guildLeave.Result = Result.Success;
            sender.SendResponse();

            if (character.Id == guild.Data.LeaderID)
            {
                foreach (var memberId in memberIds)
                {
                    if (memberId == character.Id)
                        continue;

                    var memberSession = SessionManager.Instance.GetSession(memberId);
                    if (memberSession == null)
                        continue;

                    memberSession.Session.Response.guildLeave = new GuildLeaveResponse();
                    memberSession.Session.Response.guildLeave.Result = Result.Success;
                    memberSession.SendResponse();
                }
            }
        }


        public void Init()
        {
            GuildManager.Instance.Init();
        }
    }
}
