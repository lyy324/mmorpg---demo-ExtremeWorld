using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Threading.Tasks;

using Common;
using GameServer.Entities;
using GameServer.Managers;
using Network;
using SkillBridge.Message;

namespace GameServer.Models
{
    class Chat
    {
        Character Owner;

        //当前聊天记录拉到第几条了
        public int localIdx;
        public int worldIdx;
        public int systemIdx;
        public int teamIdx;
        public int guildIdx;
        public Chat(Character owner)
        {
            this.Owner = owner;
        }

        public void PostProcess(NetMessageResponse message)
        {
            if(message.Chat == null)
            {
                message.Chat = new ChatResponse();
                message.Chat.Result = Result.Success;
            }
            this.localIdx = ChatManager.Instance.GetLocalMessage(this.Owner.Info.mapId, this.localIdx, message.Chat.localMessages);
            this.worldIdx = ChatManager.Instance.GetWorldMessage(this.worldIdx, message.Chat.worldMessages);
            this.systemIdx = ChatManager.Instance.GetSystemMessage(this.systemIdx, message.Chat.systemMssages);
            if(this.Owner.Team != null)
            {
                this.teamIdx = ChatManager.Instance.GetTeamMessage(this.Owner.Team.Id, this.teamIdx, message.Chat.teamMessages);
            }
            if(this.Owner.Guild != null)
            {
                this.guildIdx = ChatManager.Instance.GetGuildMessage(this.Owner.Guild.Id, this.guildIdx, message.Chat.guildMessages);
            }
        }
    }
}
