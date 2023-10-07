using Network;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SkillBridge.Message;
using Models;
using Common.Data;
using Managers;

namespace Services
{
    public class MapService : Singleton<MapService>, IDisposable
    {
        public MapService()
        {
            MessageDistributer.Instance.Subscribe<MapCharacterEnterResponse>(OnMapCharacterEnter);
            MessageDistributer.Instance.Subscribe<MapCharacterLeaveResponse>(OnMapCharacterLeave);

            MessageDistributer.Instance.Subscribe<MapEntitySyncResponse>(OnMapEntitySync);
        }

        public int currentMapId { get; set; }

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<MapCharacterEnterResponse>(OnMapCharacterEnter);
            MessageDistributer.Instance.Unsubscribe<MapCharacterLeaveResponse>(OnMapCharacterLeave);

            MessageDistributer.Instance.Unsubscribe<MapEntitySyncResponse>(OnMapEntitySync);
        }

        public void Init()
        {

        }

        private void OnMapCharacterEnter(object sender, MapCharacterEnterResponse message)
        {
            Debug.LogFormat("OnMapCharacterEnter:Map:{0} Count:{1}", message.mapId, message.Characters.Count);

            foreach (var character in message.Characters)
            {
                if(User.Instance.CurrentCharacter == null || User.Instance.CurrentCharacter.Id == character.Id)
                {
                    User.Instance.CurrentCharacter = character;
                }
                CharacterManager.Instance.AddCharacter(character);
            }
            if(currentMapId != message.mapId)
            {
                EnterMap(message.mapId);
                currentMapId = message.mapId;
            }
        }

        private void OnMapCharacterLeave(object sender, MapCharacterLeaveResponse message)
        {
            Debug.LogFormat("OnMapCharacterLeave:characterId:{0}", message.characterId);

            if(User.Instance.CurrentCharacter != null || message.characterId != User.Instance.CurrentCharacter.Id)
            {
                CharacterManager.Instance.RemoveCharacter(message.characterId);
            }
            else
            {
                CharacterManager.Instance.Clear();
            }
        }

        private void EnterMap(int mapId)
        {
            if (DataManager.Instance.Maps.ContainsKey(mapId))
            {
                MapDefine map = DataManager.Instance.Maps[mapId];
                User.Instance.currentMapData = map;
                SceneManager.Instance.LoadScene(map.Resource);
            }
            else
                Debug.LogErrorFormat("EnterMap:Map {0} not exited", mapId);
        }

        internal void SendMapEntitySync(EntityEvent entityEvent, NEntity entityData)
        {
            Debug.LogFormat("MapEntityUpdateRequest: Id:{0}, Pos:{1}, Dir:{2}, Speed:{3}", entityData.Id, entityData.Position.String(), entityData.Direction.String(), entityData.Speed);
            NetMessage netMessage = new NetMessage();
            netMessage.Request = new NetMessageRequest();
            netMessage.Request.mapEntitySync = new MapEntitySyncRequest();
            netMessage.Request.mapEntitySync.entitySync = new NEntitySync()
            {
                Id = entityData.Id,
                Event = entityEvent,
                Entity = entityData,
            };
            NetClient.Instance.SendMessage(netMessage);
        }

        private void OnMapEntitySync(object sender, MapEntitySyncResponse message)
        {
            foreach (var entity in message.entitySyncs)
            {
                EntityManager.Instance.OnEntitySync(entity);
            }
        }

        internal void SendMapTeleport(int id)
        {
            Debug.LogFormat("MapTeleportRequest: TeleporterId:{0}", id);
            NetMessage netMessage = new NetMessage();
            netMessage.Request = new NetMessageRequest();
            netMessage.Request.mapTeleport = new MapTeleportRequest();
            netMessage.Request.mapTeleport.teleporterId = id;
            NetClient.Instance.SendMessage(netMessage);
        }
    }
}
