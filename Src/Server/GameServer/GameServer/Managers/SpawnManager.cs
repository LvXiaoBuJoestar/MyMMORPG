using GameServer.Models;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Managers
{
    internal class SpawnManager
    {
        private List<Spawner> Rules = new List<Spawner>();
        private Map map;

        public void Init(Map map)
        {
            this.map = map;
            if (DataManager.Instance.SpawnRules.ContainsKey(map.Define.ID))
            {
                foreach (var define in DataManager.Instance.SpawnRules[map.Define.ID].Values)
                {
                    Rules.Add(new Spawner(define, this.map));
                }
            }
        }

        public void Update()
        {
            if (Rules.Count == 0)
                return;

            foreach (var rule in Rules)
            {
                rule.Update();
            }
        }
    }
}
