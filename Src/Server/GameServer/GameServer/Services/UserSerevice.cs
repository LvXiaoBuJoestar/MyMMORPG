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

            NetMessage message = new NetMessage();
            message.Response = new NetMessageResponse();
            message.Response.userLogin = new UserLoginResponse();


            TUser user = DBService.Instance.Entities.Users.Where(u => u.Username == request.User).FirstOrDefault();
            if(user==null)
            {
                message.Response.userLogin.Result = Result.Failed;
                message.Response.userLogin.Errormsg = "用户不存在";
            }
            else if(user.Password != request.Passward)
            {
                message.Response.userLogin.Result = Result.Failed;
                message.Response.userLogin.Errormsg = "密码错误";
            }
            else
            {
                sender.Session.User = user;

                message.Response.userLogin.Result = Result.Success;
                message.Response.userLogin.Errormsg = "None";
                message.Response.userLogin.Userinfo = new NUserInfo();
                message.Response.userLogin.Userinfo.Id = (int)user.ID;
                message.Response.userLogin.Userinfo.Player = new NPlayerInfo();
                message.Response.userLogin.Userinfo.Player.Id = user.Player.ID;
                foreach(var c in user.Player.Characters)
                {
                    NCharacterInfo info = new NCharacterInfo();
                    info.Id = c.ID;
                    info.Tid = c.ID;
                    info.Type = CharacterType.Player;
                    info.Name = c.Name;
                    info.Class = (CharacterClass)c.Class;
                    message.Response.userLogin.Userinfo.Player.Characters.Add(info);
                }
               
            }
            byte[]  data = PackageHandler.PackMessage(message);
            sender.SendData(data, 0, data.Length);
        }

        void OnRegister(NetConnection<NetSession> sender, UserRegisterRequest request)
        {
            Log.InfoFormat("UserRegisterRequest: User:{0}  Pass:{1}", request.User, request.Passward);

            NetMessage message = new NetMessage();
            message.Response = new NetMessageResponse();
            message.Response.userRegister = new UserRegisterResponse();


            TUser user = DBService.Instance.Entities.Users.Where(u => u.Username == request.User).FirstOrDefault();
            if (user != null)
            {
                message.Response.userRegister.Result = Result.Failed;
                message.Response.userRegister.Errormsg = "用户已存在.";
            }
            else
            {
                TPlayer player = DBService.Instance.Entities.Players.Add(new TPlayer());
                DBService.Instance.Entities.Users.Add(new TUser() { Username = request.User, Password = request.Passward, Player = player });
                DBService.Instance.Entities.SaveChanges();
                message.Response.userRegister.Result = Result.Success;
                message.Response.userRegister.Errormsg = "None";
            }

            byte[] data = PackageHandler.PackMessage(message);
            sender.SendData(data, 0, data.Length);
        }

        private void OnCreateCharacter(NetConnection<NetSession> sender, UserCreateCharacterRequest message)
        {
            Log.InfoFormat("CreateCharacterRequest: name:{0} class:{1}", message.Name, message.Class);

            TCharacter character = new TCharacter()
            {
                Name = message.Name,
                Class = (int)message.Class,
                TID = (int)message.Class,
                MapID = 1,
                MapPosX = 5000,
                MapPosY = 4000,
                MapPosZ = 820,
            };

            character = DBService.Instance.Entities.Characters.Add(character);
            sender.Session.User.Player.Characters.Add(character);
            DBService.Instance.Entities.SaveChanges();

            NetMessage netMessage = new NetMessage();
            netMessage.Response = new NetMessageResponse();
            netMessage.Response.createChar = new UserCreateCharacterResponse();
            netMessage.Response.createChar.Result = Result.Success;
            netMessage.Response.createChar.Errormsg = "None";

            foreach(var c in sender.Session.User.Player.Characters)
            {
                NCharacterInfo nCharacterInfo = new NCharacterInfo();
                nCharacterInfo.Name = c.Name;
                nCharacterInfo.Type = CharacterType.Player;
                nCharacterInfo.Id = 0;
                nCharacterInfo.Class = (CharacterClass)c.Class;
                nCharacterInfo.Tid = c.ID;
                netMessage.Response.createChar.Characters.Add(nCharacterInfo);
            }

            byte[] data = PackageHandler.PackMessage(netMessage);
            sender.SendData(data, 0, data.Length);
        }

        private void OnGameEnter(NetConnection<NetSession> sender, UserGameEnterRequest message)
        {
            TCharacter dbChar = sender.Session.User.Player.Characters.ElementAt(message.characterIdx);
            Log.InfoFormat("UserGameEnterRequest: characterID:{0}:{1} map:{2}", dbChar.ID, dbChar.Name, dbChar.MapID);
            Character character = CharacterManager.Instance.AddCharacter(dbChar);

            NetMessage netMessage = new NetMessage();
            netMessage.Response = new NetMessageResponse();
            netMessage.Response.gameEnter = new UserGameEnterResponse();
            netMessage.Response.gameEnter.Result = Result.Success;
            netMessage.Response.gameEnter.Errormsg = "None";

            netMessage.Response.gameEnter.Character = character.Info;

            //测试
            int itemId = 2;
            bool hasItem = character.ItemManager.HasItem(itemId);
            Log.InfoFormat("HasItem:[{0}]{1}", itemId, hasItem);
            if (hasItem)
                character.ItemManager.RemoveItem(itemId, 1);
            else
                character.ItemManager.AddItem(itemId, 5);
            Item item = character.ItemManager.GetItem(itemId);
            Log.InfoFormat("Item:[{0}]{1}", itemId, item);

            byte[] data = PackageHandler.PackMessage(netMessage);
            sender.SendData(data, 0, data.Length);
            sender.Session.Character = character;
            MapManager.Instance[dbChar.MapID].CharacterEnter(sender, character);
        }

        private void OnGameLeave(NetConnection<NetSession> sender, UserGameLeaveRequest message)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("UserGameLeaveRequest: character:{0},{1}  map:{2}", character.Id, character.Info.Name, character.Info.mapId);

            CharacterLeave(character);

            NetMessage netMessage = new NetMessage();
            netMessage.Response = new NetMessageResponse();
            netMessage.Response.gameLeave = new UserGameLeaveResponse();
            netMessage.Response.gameLeave.Result = Result.Success;
            netMessage.Response.gameLeave.Errormsg = "None";

            byte[] data = PackageHandler.PackMessage(netMessage);
            sender.SendData(data, 0, data.Length);
        }

        internal void CharacterLeave(Character character)
        {
            CharacterManager.Instance.RemoveCharacter(character.Id);
            MapManager.Instance[character.Info.mapId].CharacterLeave(character);
        }
    }
}
