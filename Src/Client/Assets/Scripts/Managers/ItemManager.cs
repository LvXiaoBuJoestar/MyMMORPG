using Models;
using SkillBridge.Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public class ItemManager : Singleton<ItemManager>
    {
        public Dictionary<int, Item> Items = new Dictionary<int, Item>();

        internal void Init(List<NItemInfo> infos)
        {
            Items.Clear();
            foreach (NItemInfo info in infos)
            {
                Item item = new Item(info);
                Items.Add(item.Id, item);

                Debug.LogFormat("ItemManager:Init:[{0}]", item);
            }
        }
    }
}
