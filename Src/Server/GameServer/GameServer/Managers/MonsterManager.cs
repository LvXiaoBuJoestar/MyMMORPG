﻿using GameServer.Entities;
using GameServer.Models;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Managers
{
    class MonsterManager
    {
        private Map map;
        public Dictionary<int, Monster> Monsters = new Dictionary<int, Monster>();

        public void Init(Map map)
        {
            this.map = map;
        }

        internal Monster Create(int spawnMonID, int spawnLevel, NVector3 position, NVector3 direction)
        {
            Monster monster = new Monster(spawnMonID, spawnLevel, position, direction);
            EntityManager.Instance.AddEntity(map.ID, monster);
            monster.Info.Id = monster.entityId;
            monster.Info.EntityId = monster.entityId;
            monster.Info.mapId = map.ID;
            Monsters[monster.Id] = monster;

            map.MonsterEnter(monster);
            return monster;
        }
    }
}
