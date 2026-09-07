using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Network;
using SkillBridge.Message;
using GameServer.Entities;
using GameServer.Managers;
using System.Data.SqlClient;

namespace GameServer.Services
{
    class UserService : Singleton<UserService>
    {

        public UserService()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserLoginRequest>(this.OnLogin);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserRegisterRequest>(this.OnRegister);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserCreateCharacterRequest>(this.OnCreateCharacter);

            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserGameEnterRequest>(this.OnGameEnter);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<UserGameLeaveRequest>(this.OnGameLeave);
        }


        public void Init()
        {

        }

        void OnLogin(NetConnection<NetSession> sender, UserLoginRequest request)
        {
            Log.InfoFormat("UserLoginRequest: User:{0}  Pass:{1}", request.User, request.Passward);

            sender.Session.Response.userLogin = new UserLoginResponse();

            //获取服务器中 User表中存储的登录相关的数据 并且创建一个 TUser 类型的实例 user 来接收数据
            TUser user = DBService.Instance.Execute(db =>
                db.Users.Where(u => u.Username == request.User).FirstOrDefault());
            //校验
            if (user == null)
            {
                sender.Session.Response.userLogin.Result = Result.Failed;
                sender.Session.Response.userLogin.Errormsg = "用户不存在";
            }
            else if (user.Password != request.Passward)
            {
                sender.Session.Response.userLogin.Result = Result.Failed;
                sender.Session.Response.userLogin.Errormsg = "密码错误";
            }
            else
            {
                sender.Session.User = user;

                
                sender.Session.Response.userLogin.Result = Result.Success;
                sender.Session.Response.userLogin.Errormsg = "None";
                sender.Session.Response.userLogin.Userinfo = new NUserInfo();
                sender.Session.Response.userLogin.Userinfo.Id = (int)user.ID;
                sender.Session.Response.userLogin.Userinfo.Player = new NPlayerInfo();
                sender.Session.Response.userLogin.Userinfo.Player.Id = user.Player.ID;
                foreach (var c in user.Player.Characters)
                {
                    NCharacterInfo info = new NCharacterInfo();
                    info.Id = c.ID;
                    info.Name = c.Name;
                    info.Type = CharacterType.Player;
                    info.Class = (CharacterClass)c.Class;
                    info.ConfigId = c.ID;
                    sender.Session.Response.userLogin.Userinfo.Player.Characters.Add(info);
                }

            }
            sender.SendResponse();
        }

        void OnRegister(NetConnection<NetSession> conn, UserRegisterRequest request)
        {
            Log.InfoFormat("UserRegisterRequest: User:{0}  Pass:{1}", request.User, request.Passward);
            conn.Session.Response.userRegister = new UserRegisterResponse();

            //这一段是 
            DBService.Instance.Execute(db =>
            {
                TUser user = db.Users.Where(u => u.Username == request.User).FirstOrDefault();
                if (user != null)
                {
                    conn.Session.Response.userRegister.Result = Result.Failed;
                    conn.Session.Response.userRegister.Errormsg = "用户已存在.";
                }
                else
                {
                    TPlayer player = db.Players.Add(new TPlayer());
                    db.Users.Add(new TUser() { Username = request.User, Password = request.Passward, Player = player });
                    db.SaveChanges();
                    conn.Session.Response.userRegister.Result = Result.Success;
                    conn.Session.Response.userRegister.Errormsg = "None";
                }
            });

            conn.SendResponse();
        }

        private void OnCreateCharacter(NetConnection<NetSession> sender, UserCreateCharacterRequest request)
        {
            Log.InfoFormat("UserCreateCharacterRequest: Name:{0}  Class:{1}", request.Name, request.Class);

            UserCreateCharacterResponse response = new UserCreateCharacterResponse()
            {
                Result = Result.Failed,
                Errormsg = "角色创建失败"
            };

            try
            {
                TCharacter character = DBService.Instance.Execute(db =>
                {
                    TCharacter characterToCreate = new TCharacter()
                    {
                        Name = request.Name,
                        Class = (int)request.Class,
                        TID = (int)request.Class,
                        Level = 1,
                        MapID = 1,
                        MapPosX = 5000,
                        MapPosY = 4000,
                        MapPosZ = 820,
                        Gold = 100000,
                        Equips = new byte[28],
                        Player = sender.Session.User.Player
                    };
                    var bag = new TCharacterBag()
                    {
                        Owner = characterToCreate,
                        Items = new byte[0],
                        Unlocked = 20
                    };
                    characterToCreate.Bag = db.CharacterBags.Add(bag);

                    characterToCreate = db.Characters.Add(characterToCreate);
                    characterToCreate.Items.Add(new TCharacterItem()
                    {
                        Owner = characterToCreate,
                        ItemID = 1,
                        ItemCount = 20,
                    });
                    characterToCreate.Items.Add(new TCharacterItem()
                    {
                        Owner = characterToCreate,
                        ItemID = 2,
                        ItemCount = 20,
                    });

                    db.SaveChanges();
                    return characterToCreate;
                });

                if (!sender.Session.User.Player.Characters.Contains(character))
                    sender.Session.User.Player.Characters.Add(character);

                response.Result = Result.Success;
                response.Errormsg = "None";
            }
            catch (Exception ex)
            {
                DBService.Instance.DiscardPendingChanges();
                Log.ErrorFormat("Create character failed: {0}", ex);
                response.Errormsg = "角色创建失败，请检查数据库结构和服务器日志";
            }

            if (response.Result == Result.Success)
            {
                foreach (var c in sender.Session.User.Player.Characters)
                {
                    NCharacterInfo info = new NCharacterInfo();
                    info.Id = c.ID;
                    info.Name = c.Name;
                    info.Type = CharacterType.Player;
                    info.Class = (CharacterClass)c.Class;
                    info.ConfigId = c.TID;
                    response.Characters.Add(info);
                }
            }

            sender.Session.Response.createChar = response;
            sender.SendResponse();
        }


        void OnGameEnter(NetConnection<NetSession> sender, UserGameEnterRequest request)
        {
            TCharacter dbchar = sender.Session.User.Player.Characters.ElementAt(request.characterIdx);
            Log.InfoFormat("UserGameEnterRequest: characterID:{0}:{1} Map:{2}", dbchar.ID, dbchar.Name, dbchar.MapID);
            Character character = CharacterManager.Instance.AddCharacter(dbchar);

            SessionManager.Instance.AddSession(character.Id, sender);
            sender.Session.Response.gameEnter = new UserGameEnterResponse();
            sender.Session.Response.gameEnter.Result = Result.Success;
            sender.Session.Response.gameEnter.Errormsg = "None";


            //进入成功，发送初始角色信息
            sender.Session.Character = character;
            sender.Session.PostResponser = character;

            sender.Session.Response.gameEnter.Character = character.Info;

            //道具系统测试
            //int itemId = 2;
            //bool hasItem = character.ItemManager.HasItem(itemId);
            //Log.InfoFormat("Character has item {0} : {1}", itemId, hasItem);
            //if(hasItem)
            //{
            //    //character.ItemManager.RemoveItem(itemId, 1);
            //}
            //else
            //{
            //    character.ItemManager.AddItem(itemId, 5);
            //    character.ItemManager.AddItem(1, 100);
            //    character.ItemManager.AddItem(2, 200);
            //    character.ItemManager.AddItem(3, 70);
            //    character.ItemManager.AddItem(4, 397);
            //}
            //Models.Item item = character.ItemManager.GetItem(itemId);

            //Log.InfoFormat("Character item {0} : {1}", itemId, item);

            sender.SendResponse();

            MapManager.Instance[dbchar.MapID].CharacterEnter(sender, character);
        }

        void OnGameLeave(NetConnection<NetSession> sender, UserGameLeaveRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("UserGameLeaveRequest: characterID:{0}:{1} Map:{2}", character.Id, character.Info.Name, character.Info.mapId);
            this.CharacterLeave(character);
            sender.Session.Response.gameLeave = new UserGameLeaveResponse();
            sender.Session.Response.gameLeave.Result = Result.Success;
            sender.Session.Response.gameLeave.Errormsg = "None";

            sender.SendResponse();
        }

        public void CharacterLeave(Character character)
        {
            Log.InfoFormat("CharacterLeave： characterID:{0}:{1}", character.Id, character.Info.Name);
            SessionManager.Instance.RemoveSession(character.Id);
            CharacterManager.Instance.RemoveCharacter(character.Id);
            character.Clear();
            MapManager.Instance[character.Info.mapId].CharacterLeave(character);
        }
    }
}
