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
using GameServer.Models;

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

        void OnLogin(NetConnection<NetSession> sender,UserLoginRequest request)
        {
            Log.InfoFormat("UserLoginRequest: User:{0}  Pass:{1}", request.User, request.Passward);

            sender.Session.Response.userLogin = new UserLoginResponse();

            TUser user = DBService.Instance.Entities.Users.Where(u => u.Username == request.User).FirstOrDefault();
            if(user==null)
            {
                sender.Session.Response.userLogin.Result = Result.Failed;
                sender.Session.Response.userLogin.Errormsg = "用户不存在";
            }
            else if(user.Password != request.Passward)
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
                foreach(var c in user.Player.Characters)
                {
                    NCharacterInfo info = new NCharacterInfo();
                    info.Id = c.ID;
                    info.ConfigId = c.ID;
                    info.Type = CharacterType.Player;
                    info.Name = c.Name;
                    info.Class = (CharacterClass)c.Class;
                    sender.Session.Response.userLogin.Userinfo.Player.Characters.Add(info);
                }
            }
            sender.SendResponse();
        }

        void OnRegister(NetConnection<NetSession> sender, UserRegisterRequest request)
        {
            Log.InfoFormat("UserRegisterRequest: User:{0}  Pass:{1}", request.User, request.Passward);

            sender.Session.Response.userRegister = new UserRegisterResponse();


            TUser user = DBService.Instance.Entities.Users.Where(u => u.Username == request.User).FirstOrDefault();
            if (user != null)
            {
                sender.Session.Response.userRegister.Result = Result.Failed;
                sender.Session.Response.userRegister.Errormsg = "用户已存在.";
            }
            else
            {
                TPlayer player = DBService.Instance.Entities.Players.Add(new TPlayer());
                DBService.Instance.Entities.Users.Add(new TUser() { Username = request.User, Password = request.Passward, Player = player });
                DBService.Instance.Entities.SaveChanges();
                sender.Session.Response.userRegister.Result = Result.Success;
                sender.Session.Response.userRegister.Errormsg = "None";
            }

            sender.SendResponse();
        }

        private void OnCreateCharacter(NetConnection<NetSession> sender, UserCreateCharacterRequest message)
        {
            Log.InfoFormat("CreateCharacterRequest: name:{0} class:{1}", message.Name, message.Class);

            TCharacter character = new TCharacter()
            {
                Name = message.Name,
                Class = (int)message.Class,
                TID = (int)message.Class,
                Level = 1,
                MapID = 1,
                MapPosX = 5000,
                MapPosY = 4000,
                MapPosZ = 820,
                Gold = 10000,
                Equips = new byte[28],
            };

            TCharacterBag bag = new TCharacterBag();
            bag.Items = new byte[0];
            bag.Unlocked = 20;
            bag.Owner = character;
            character.Bag = DBService.Instance.Entities.CharacterBag.Add(bag);

            character = DBService.Instance.Entities.Characters.Add(character);

            character.Items.Add(new TCharacterItem() { Owner = character, ItemID = 1, ItemCount = 20, });
            character.Items.Add(new TCharacterItem() { Owner = character, ItemID = 2, ItemCount = 20, });

            sender.Session.User.Player.Characters.Add(character);
            DBService.Instance.Entities.SaveChanges();

            sender.Session.Response.createChar = new UserCreateCharacterResponse();
            sender.Session.Response.createChar.Result = Result.Success;
            sender.Session.Response.createChar.Errormsg = "None";

            foreach(var c in sender.Session.User.Player.Characters)
            {
                NCharacterInfo nCharacterInfo = new NCharacterInfo();
                nCharacterInfo.Name = c.Name;
                nCharacterInfo.Type = CharacterType.Player;
                nCharacterInfo.Id = c.ID;
                nCharacterInfo.Class = (CharacterClass)c.Class;
                nCharacterInfo.ConfigId = c.TID;
                sender.Session.Response.createChar.Characters.Add(nCharacterInfo);
            }

            sender.SendResponse();
        }

        private void OnGameEnter(NetConnection<NetSession> sender, UserGameEnterRequest message)
        {
            TCharacter dbChar = sender.Session.User.Player.Characters.ElementAt(message.characterIdx);
            Log.InfoFormat("UserGameEnterRequest: characterID:{0}:{1} map:{2}", dbChar.ID, dbChar.Name, dbChar.MapID);
            Character character = CharacterManager.Instance.AddCharacter(dbChar);
            SessionManager.Instance.AddSession(character.Id, sender);

            sender.Session.Response.gameEnter = new UserGameEnterResponse();
            sender.Session.Response.gameEnter.Result = Result.Success;
            sender.Session.Response.gameEnter.Errormsg = "None";
            sender.Session.Response.gameEnter.Character = character.Info;
            sender.SendResponse();
            
            sender.Session.Character = character;
            sender.Session.PostResponser = character;
            MapManager.Instance[dbChar.MapID].CharacterEnter(sender, character);
        }

        private void OnGameLeave(NetConnection<NetSession> sender, UserGameLeaveRequest message)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("UserGameLeaveRequest: character:{0},{1}  map:{2}", character.Id, character.Info.Name, character.Info.mapId);
            SessionManager.Instance.RemoveSession(character.Id);
            CharacterLeave(character);

            sender.Session.Response.gameLeave = new UserGameLeaveResponse();
            sender.Session.Response.gameLeave.Result = Result.Success;
            sender.Session.Response.gameLeave.Errormsg = "None";
            sender.SendResponse();
        }

        internal void CharacterLeave(Character character)
        {
            CharacterManager.Instance.RemoveCharacter(character.Id);
            character.Clear();
            MapManager.Instance[character.Info.mapId].CharacterLeave(character);
        }
    }
}
