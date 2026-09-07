using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Common;
using Common.Utils;
using GameServer.Entities;
using GameServer.Managers;
using GameServer.Services;
using Network;
using SkillBridge.Message;

namespace GameServer.Models
{
    class Guild
    {
        public int Id { get { return this.Data.Id; } }
        public string Name { get { return this.Data.Name;  } }
        public List<Character> Members = new List<Character>();
        public double timestamp;
        public TGuild Data;
        public Guild(TGuild guild)
        {
            this.Data = guild;
        }
        /// <summary>
        /// 加入公会申请
        /// </summary>
        /// <param name="apply"></param>
        /// <returns></returns>
        public bool JoinApply(NGuildApplyInfo apply)
        {
            //一个人只可以申请一次
            var oldApply = this.Data.Applies.FirstOrDefault(v => v.CharacterId == apply.characterId);
            if(oldApply != null)
            {
                return false;
            }
            var dbApply = DBService.Instance.Entities.GuildApplies.Create(); //创建一条数据库的记录
            dbApply.GuildId = apply.GuildId;
            dbApply.CharacterId = apply.characterId;
            dbApply.Name = apply.Name;
            dbApply.Class = apply.Class;
            dbApply.Level = apply.Level;
            dbApply.ApplyTime = DateTime.Now;

            DBService.Instance.Entities.GuildApplies.Add(dbApply);
            this.Data.Applies.Add(dbApply);

            DBService.Instance.Save();

            this.timestamp = TimeUtil.timestamp;
            return true;
        }
        /// <summary>
        /// 加入公会审核
        /// </summary>
        /// <param name="apply"></param>
        /// <returns></returns>
        public bool JoinAppove(NGuildApplyInfo apply)
        {
            var oldApply = this.Data.Applies.FirstOrDefault(v => v.CharacterId == apply.characterId && v.Result == 0);
            if(oldApply == null)
            {
                return false;
            }
            oldApply.Result = (int)apply.Result;
            if(apply.Result == ApplyResult.Accept)
            {
                this.AddMember(apply.characterId, apply.Name,apply.Class, apply.Level, GuildTitle.None);
            }
            DBService.Instance.Save();

            this.timestamp = TimeUtil.timestamp;
            return true;
        }
        //添加成员的逻辑
        public void AddMember(int id, string name,int @class, int level, GuildTitle title)
        {
            DateTime now = DateTime.Now;
            TGuildMember dbMember = new TGuildMember()
            {
                CharacterId = id,
                Name = name,
                Class = @class,
                Level = level,
                Title = (int)title,
                JoinTime = now,
                LastTime = now,
            };
            this.Data.Members.Add(dbMember);
            var character = CharacterManager.Instance.GetCharacter(id);
            if (character != null)
                character.Data.GuildId = this.Id;
            else
            {
                TCharacter dbChar = DBService.Instance.Entities.Characters.SingleOrDefault(c => c.ID == id);
                dbChar.GuildId = this.Id;
            }
            timestamp = TimeUtil.timestamp;
        }
        //获取公会信息
        public NGuildInfo GuildInfo(Character from)
        {
            NGuildInfo info = new NGuildInfo()
            {
                Id = this.Id,
                GuildName = this.Name,
                Notice = this.Data.Notice,
                leaderId = this.Data.LeaderID,
                leaderName = this.Data.LeaderName,
                createTime = (long)TimeUtil.GetTimestamp(this.Data.CreateTime),
                memberCount = this.Data.Members.Count,
            };
            if (from != null)
            {
                info.Members.AddRange(GetMemberInfos());
                if (from.Id == this.Data.LeaderID)
                    info.Applies.AddRange(GetApplyInfos());
                    
            }
            return info;
        }
        //获得公会申请信息
        private List<NGuildApplyInfo> GetApplyInfos()
        {
            List<NGuildApplyInfo> applies = new List<NGuildApplyInfo>();
            foreach(var apply in this.Data.Applies)
            {
                if (apply.Result != (int)ApplyResult.None) continue;
                applies.Add(new NGuildApplyInfo()
                {
                    characterId = apply.CharacterId,
                    GuildId = apply.GuildId,
                    Class = apply.Class,
                    Level = apply.Level,
                    Name = apply.Name,
                    Result = (ApplyResult)apply.Result,
                });
            }
            return applies;
        }
        //获得公会成员信息
        private List<NGuildMemberInfo> GetMemberInfos()
        {
            List<NGuildMemberInfo> members = new List<NGuildMemberInfo>();
            foreach(var member in this.Data.Members)
            {
                var memberInfo = new NGuildMemberInfo()
                {
                    Id = member.Id,
                    characterId = member.CharacterId,
                    Title = (GuildTitle)member.Title,
                    joinTime = (long)TimeUtil.GetTimestamp(member.JoinTime),
                    lastTime = (long)TimeUtil.GetTimestamp(member.LastTime)
                };
                //应该加入更多的检查
                //成员在线的话 更新一下对应的信息
                var character = CharacterManager.Instance.GetCharacter(member.CharacterId);
                if(character != null)
                {
                    memberInfo.Info = character.GetBasicInfo();
                    memberInfo.Status = 1;
                    member.Level = character.Data.Level;
                    member.Name = character.Data.Name;
                    member.LastTime = DateTime.Now;
                }
                else
                {
                    memberInfo.Info = this.GetMemberInfo(member);
                    memberInfo.Status = 0;
                }

                members.Add(memberInfo);
            }
            return members;
        }
        //获取单个成员的信息
        private NCharacterInfo GetMemberInfo(TGuildMember member)
        {
            return new NCharacterInfo()
            {
                Id = member.CharacterId,
                Name = member.Name,
                Class = (CharacterClass)member.Class,
                Level = member.Level,
            };

        }

        public void PostProcess(Character from, NetMessageResponse message)
        {
            if(message.Guild == null)
            {
                message.Guild = new GuildResponse();
                message.Guild.Result = Result.Success;
                message.Guild.guildInfo = this.GuildInfo(from);
            }
        }

        public void RemoveMember(TGuildMember member)
        {
            if (member == null)
                return;

            var apply = GetDbApply(member.CharacterId);
            DBService.Instance.Entities.GuildMembers.Remove(member);
            if (apply != null)
                DBService.Instance.Entities.GuildApplies.Remove(apply);

            var character = CharacterManager.Instance.GetCharacter(member.CharacterId);
            if (character != null)
            {
                character.Data.GuildId = 0;
                character.Guild = null;
            }
            else
            {
                TCharacter dbchar = DBService.Instance.Entities.Characters.FirstOrDefault(c => c.ID == member.CharacterId);
                if (dbchar != null)
                    dbchar.GuildId = 0;
            }

        }

        public bool Leave(int characterId)
        {
            var member = GetDbMember(characterId);
            if (member == null)
                return false;

            RemoveMember(member);
            return true;
        }

        public List<int> Disband()
        {
            var members = this.Data.Members.ToList();
            var memberIds = members.Select(member => member.CharacterId).ToList();

            foreach (var member in members)
                RemoveMember(member);

            return memberIds;
        }
        TGuildMember GetDbMember(int characterId)
        {
            foreach (var member in this.Data.Members)
            {
                if (member.CharacterId == characterId)
                {
                    return member;
                }
            }
            return null;

        }
        TGuildApply GetDbApply(int characterId)
        {
            foreach (var apply in this.Data.Applies)
            {
                if (apply.CharacterId == characterId)
                {
                    return apply;
                }
            }
            return null;
        }

        internal void ExecuteAdmin(GuildAdminCommand command, int targetId, int sourceId)
        {
            var target = GetDbMember(targetId);
            var source = GetDbMember(sourceId);
            switch (command)
            {
                case GuildAdminCommand.Promote:
                    target.Title = (int)GuildTitle.VicePresident;
                    break;
                case GuildAdminCommand.Depost:
                    target.Title = (int)GuildTitle.None; 
                    break;
                case GuildAdminCommand.Transfer:
                    target.Title = (int)GuildTitle.President;
                    source.Title = (int)GuildTitle.None;
                    this.Data.LeaderID = targetId;
                    this.Data.LeaderName = target.Name;
                    break;
                case GuildAdminCommand.Kickout:
                    this.Leave(targetId);
                    break;
            }
            DBService.Instance.Save();
            timestamp = TimeUtil.timestamp;

        }
    }
}
