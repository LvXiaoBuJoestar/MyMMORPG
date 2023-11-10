using Common;
using GameServer.Entities;
using GameServer.Managers;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Services
{
    class FriendService : Singleton<FriendService>
    {
        List<FriendAddRequest> friendRequests = new List<FriendAddRequest>();

        public FriendService()
        {
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<FriendAddRequest>(OnFriendAddRequest);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<FriendAddResponse>(OnFriendAddResponse);
            MessageDistributer<NetConnection<NetSession>>.Instance.Subscribe<FriendRemoveRequest>(OnFriendRemove);
        }

        public void Init() { }

        private void OnFriendAddRequest(NetConnection<NetSession> sender, FriendAddRequest message)
        {
            Log.InfoFormat("OnFriendAddRequest:: FromId:{0}, FromName:{1}, ToId:{2}, ToName:{3}", message.FromId, message.FromName, message.ToId, message.ToName);
            Character character = sender.Session.Character;

            if(message.ToId == 0)
            {
                foreach(var cha in CharacterManager.Instance.characters)
                {
                    if(cha.Value.Info.Name == message.ToName)
                    {
                        message.ToId = cha.Key;
                        break;
                    }
                }
            }

            NetConnection<NetSession> friend = null;
            if(message.ToId > 0)
            {
                if(character.FriendManager.GetFriendInfo(message.ToId) != null)
                {
                    sender.Session.Response.friendAddRes = new FriendAddResponse();
                    sender.Session.Response.friendAddRes.Result = Result.Failed;
                    sender.Session.Response.friendAddRes.Errormsg = "你们已经是好友了哦";
                    sender.SendResponse();
                    return;
                }
                friend = SessionManager.Instance.GetSession(message.ToId);
            }
            if(friend == null)
            {
                sender.Session.Response.friendAddRes = new FriendAddResponse();
                sender.Session.Response.friendAddRes.Result = Result.Failed;
                sender.Session.Response.friendAddRes.Errormsg = "该玩家不存在或者不在线";
                sender.SendResponse();
                return;
            }

            Log.Info("OnfriendAddRequestFinished");
            friend.Session.Response.friendAddReq = message;
            friend.SendResponse();
        }

        private void OnFriendAddResponse(NetConnection<NetSession> sender, FriendAddResponse response)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnFriendAddResponse:: Character: {0}, Result: {1} , FromId: {2}, ToId: {3}", character.Id, response.Result, response.Request.FromId, response.Request.ToId);
            sender.Session.Response.friendAddRes = response;
            if(response.Result == Result.Success)
            {
                var requester = SessionManager.Instance.GetSession(response.Request.FromId);
                if(requester == null)
                {
                    sender.Session.Response.friendAddRes.Result = Result.Failed;
                    sender.Session.Response.friendAddRes.Errormsg = "申请者已下线";
                }
                else
                {
                    character.FriendManager.AddFriend(requester.Session.Character);
                    requester.Session.Character.FriendManager.AddFriend(character);
                    DBService.Instance.Save();
                    requester.Session.Response.friendAddRes = response;
                    requester.Session.Response.friendAddRes.Result = Result.Success;
                    requester.Session.Response.friendAddRes.Errormsg = "添加好友成功";
                    requester.SendResponse();
                }
            }
            sender.SendResponse();
        }

        private void OnFriendRemove(NetConnection<NetSession> sender, FriendRemoveRequest request)
        {
            Character character = sender.Session.Character;
            Log.InfoFormat("OnFriendRemove:: Character: {0}, FriendRelationid: {1}", character.Id, request.Id);
            sender.Session.Response.friendRemove = new FriendRemoveResponse();
            sender.Session.Response.friendRemove.Id = request.Id;

            if (character.FriendManager.RemoveFriendById(request.Id))
            {
                sender.Session.Response.friendRemove.Result = Result.Success;
                var friend = SessionManager.Instance.GetSession(request.friendId);
                if (friend != null)
                    friend.Session.Character.FriendManager.RemoveFriendByFriendId(request.friendId);
                else
                    RemoveFriend(request.friendId, character.Id);
            }
            else
                sender.Session.Response.friendRemove.Result = Result.Failed;

            DBService.Instance.Save();
            sender.SendResponse();
        }

        private void RemoveFriend(int charId, int friendId)
        {
            var removeItem = DBService.Instance.Entities.CharacterFriends.FirstOrDefault(v => v.TCharacterID == charId && v.FriendId == friendId);
            if(removeItem != null)
                DBService.Instance.Entities.CharacterFriends.Remove(removeItem);
        }
    }
}
