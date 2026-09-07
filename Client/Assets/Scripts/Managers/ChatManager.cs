using Models;
using Services;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Text;
using UnityEngine;

namespace Managers
{
    class ChatManager : Singleton<ChatManager>, IDisposable
    {
        public enum LocalChannel
        {
            All = 0,//所有
            Local = 1,//本地
            World = 2,//世界
            Team = 3,//队伍
            Guild = 4,//公会
            Private = 5,//私聊
        }

        private ChatChannel[] ChannelFilter = new ChatChannel[6]
        {
            ChatChannel.Local | ChatChannel.World | ChatChannel.Guild | ChatChannel.Team | ChatChannel.Private | ChatChannel.System,//All
            ChatChannel.Local,
            ChatChannel.World,
            ChatChannel.Team,
            ChatChannel.Guild,
            ChatChannel.Private,
        };
        public LocalChannel displayChannel;
        public LocalChannel sendChannel;

        public ChatChannel SendChannel
        {
            get
            {
                switch (sendChannel)
                {
                    case LocalChannel.Local: return ChatChannel.Local;
                    case LocalChannel.World: return ChatChannel.World;
                    case LocalChannel.Team: return ChatChannel.Team;
                    case LocalChannel.Guild: return ChatChannel.Guild;
                    case LocalChannel.Private: return ChatChannel.Private;
                }
                return ChatChannel.Local;
            }
        }
        public int PrivateID;
        public string PrivateName;
        //当前本地的所有聊天信息
        public List<ChatMessage>[] Messages = new List<ChatMessage>[6]
        {
            new List<ChatMessage>(),//All
            new List<ChatMessage>(),//Local
            new List<ChatMessage>(),//World
            new List<ChatMessage>(),//Team
            new List<ChatMessage>(),//Guild
            new List<ChatMessage>(),//Private
        };
        public Action OnChat { get; internal set; }

        //发起私聊
        public void StartPrivateChat(int targetId,string targetName)
        {
            this.PrivateID = targetId;
            this.PrivateName = targetName;

            this.sendChannel = LocalChannel.Private;
            if (this.OnChat != null)
                this.OnChat();
        }

        public void SendChat(string content,int toId = 0,string toName = "")
        {
            //this.Messages.Add(new ChatMessage()
            //{
            //    Channel = ChatChannel.Local,
            //    Message = content,
            //    FromId = User.Instance.CurrentCharacter.Id,
            //    FromName = User.Instance.CurrentCharacter.Name,
            //});
            ChatService.Instance.SendChat(this.SendChannel, content, toId, toName);
        }
        public void AddMessages(ChatChannel channel,List<ChatMessage> messages)
        {
            for(int ch = 0; ch < 6;ch ++)
            {
                if ((this.ChannelFilter[ch] & channel) == channel)
                {
                    this.Messages[ch].AddRange(messages);
                }
            }
            if(this.OnChat != null)
                this.OnChat();
        }
        public bool SetSendChannel(LocalChannel channel)
        {
            if(channel == LocalChannel.Team)
            {
                if(User.Instance.TeamInfo == null)
                {
                    this.AddSystemMessage("你没有加入任何队伍");
                    return false;
                }    
            }
            if(channel == LocalChannel.Guild)
            {
                if(User.Instance.CurrentCharacter.Guild == null)
                {
                    this.AddSystemMessage("你还没有公会");
                    return false;
                }
            }
            this.sendChannel = channel;
            Debug.LogFormat("Set Channel:{0}", this.sendChannel);
            return true;
        }
        //
        public void AddSystemMessage(string message,string from = "")
        {
            this.Messages[(int)LocalChannel.All].Add(new ChatMessage() 
            {
                Channel = ChatChannel.System,
                Message = message,
                FromName =from
            });
            if (this.OnChat != null)
                this.OnChat();
        }

        public string GetCurrentMessages()
        {
            StringBuilder sb = new StringBuilder();
            foreach(var message in this.Messages[(int)displayChannel])
            {
                sb.Append(FormatMessage(message)).Append('\n');
            }
            return sb.ToString();
        }

        private string FormatMessage(ChatMessage message)
        {
            string content = FormatPlainText(message.Message);
            switch (message.Channel)
            {
                case ChatChannel.Local:
                    return string.Format("<color=white>[本地]</color>{0}<color=white>{1}</color>", FormatFromPlayer(message), content);
                case ChatChannel.World:
                    return string.Format("<color=cyan>[世界]</color>{0}<color=white>{1}</color>", FormatFromPlayer(message), content);
                case ChatChannel.System:
                    return string.Format("<color=red>[系统]{0}</color>", content);
                case ChatChannel.Private:
                    return string.Format("<color=magenta>[私聊]</color>{0}<color=white>{1}</color>", FormatFromPlayer(message), content);
                case ChatChannel.Team:
                    return string.Format("<color=green>[队伍]</color>{0}<color=white>{1}</color>", FormatFromPlayer(message), content);
                case ChatChannel.Guild:
                    return string.Format("<color=blue>[公会]</color>{0}<color=white>{1}</color>", FormatFromPlayer(message), content);

            }
            return "";

        }

        private string FormatFromPlayer(ChatMessage message)
        {
            if(message.FromId  == User.Instance.CurrentCharacter.Id)
            {
                return "<a name=\"\" class=\"player\">[我]</a>";
            }
            else
            {
                return string.Format("<a name=\"c:{0}:{1}\" class=\"player\">[{2}]</a>",
                    message.FromId, Uri.EscapeDataString(message.FromName ?? ""), FormatPlainText(message.FromName));
            }
        }

        private static string FormatPlainText(string text)
        {
            // Legacy UI.Text does not decode HTML entities; full-width brackets remain readable literal text.
            return (text ?? "").Replace("\r\n", "\n").Replace('\r', '\n')
                .Replace('<', '\uFF1C').Replace('>', '\uFF1E');
        }

        public ChatManager()
        { 
        }

        public void Dispose()
        {
        }

        public void Init()
        {
            foreach(var messages in this.Messages)
            {
                messages.Clear();
            }
        }
    }
}


