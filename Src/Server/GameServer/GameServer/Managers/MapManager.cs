using Common;
using GameServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Managers
{
    internal class MapManager : Singleton<MapManager>
    {
        Dictionary<int, Map> maps = new Dictionary<int, Map>();

        public void Init()
        {
            foreach(var mapDefine in DataManager.Instance.Maps.Values)
            {
                Map map = new Map(mapDefine);
                Log.InfoFormat("MapManager.Init > Map: {0}:{1}", mapDefine.ID, mapDefine.Name);
                maps.Add(mapDefine.ID, map);
            }
        }

        public Map this[int key]
        {
            get
            {
                return maps[key];
            }
        }

        public void Update()
        {
            foreach(Map map in maps.Values)
            {
                map.Update();
            }
        }
    }
}
