using Models;
using Services;
using SkillBridge.Message;
using System;
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

            StatusService.Instance.RegisterStatusNotify(StatusType.Item, this.OnItemNotify);
        }

        private bool OnItemNotify(NStatus status)
        {
            if (status.Action == StatusAction.Add)
                AddItem(status.Id, status.Value);
            else if(status.Action == StatusAction.Delete)
                RemoveItem(status.Id, status.Value);

            return true;
        }

        void AddItem(int itemId, int count)
        {
            if (Items.TryGetValue(itemId, out var item))
                item.Count += count;
            else
            {
                item = new Item(itemId, count);
                Items.Add(item.Id, item);
            }
            BagManager.Instance.AddItem(itemId, count);
        }

        void RemoveItem(int itemId, int count)
        {
            if(!Items.ContainsKey(itemId)) return;
            Item item = Items[itemId];
            if (item.Count < count) return;
            item.Count -= count;

            BagManager.Instance.RemoveItem(itemId, count);
        }
    }
}
