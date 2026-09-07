using Managers;
using Network;
using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Services
{
    class ChatService : Singleton<ChatService>, IDisposable
    {
        public ChatService()
        {
            MessageDistributer.Instance.Subscribe<ChatResponse>(this.OnChat);
        }



        public void Dispose()
        {

        }

        public void Init()
        {
            
        }

        internal void SendChat(ChatChannel sendChannel, string content, int toId, string toName)
        {
            NetMessage message = new NetMessage();
            message.Request = new NetMessageRequest();
            message.Request.Chat = new ChatRequest();
            message.Request.Chat.Message = new ChatMessage();
            ChatMessage chatMessage = new ChatMessage()
            {
                ToId = toId,
                ToName = toName,
                Channel =sendChannel,
                Message = content
            };
            message.Request.Chat.Message = chatMessage;
            NetClient.Instance.SendMessage(message);
        }
        private void OnChat(object sender, ChatResponse response)
        {
            if(response.Result == Result.Failed)
            {
                ChatManager.Instance.AddSystemMessage(response.Errormsg);
                return; 
            }
            ChatManager.Instance.AddMessages(ChatChannel.World,response.worldMessages);
            ChatManager.Instance.AddMessages(ChatChannel.System,response.systemMssages);
            ChatManager.Instance.AddMessages(ChatChannel.Local,response.localMessages);
            ChatManager.Instance.AddMessages(ChatChannel.Team,response.teamMessages);
            ChatManager.Instance.AddMessages(ChatChannel.Guild,response.guildMessages);
            //if(ChatManager.Instance.OnChat != null)
            //    ChatManager.Instance.OnChat();
        }
        
    }
}
