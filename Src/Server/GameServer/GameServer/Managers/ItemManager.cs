using Common;
using GameServer.Entities;
using GameServer.Models;
using GameServer.Services;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Managers
{
    internal class ItemManager
    {
        Character Owner;

        public Dictionary<int, Item> Items = new Dictionary<int, Item>();

        public ItemManager(Character owner)
        {
            this.Owner = owner;

            foreach(var item in owner.Data.Items)
            {
                Items.Add(item.ItemID, new Item(item));
            }
        }

        public bool UseItem(int itemId, int count = 1)
        {
            Log.InfoFormat("{0} Use Item {1}, {2}", Owner.Data.ID, itemId, count);

            Item item = null;
            if(Items.TryGetValue(itemId, out item))
            {
                if (item.Count < count)
                    return false;

                item.Remove(count);
                return true;
            }

            return false;
        }

        public bool HasItem(int itemId)
        {
            if(Items.TryGetValue(itemId, out Item item))
                return item.Count > 0;
            return false;
        }

        public Item GetItem(int itemId)
        {
            Item item = null;
            Items.TryGetValue(itemId, out item);
            Log.InfoFormat("{0} Get Item {1}", Owner.Data.ID, itemId);
            return item;
        }

        public void AddItem(int itemId, int count)
        {
            Item item = null;

            if(Items.TryGetValue(itemId, out item))
                item.Add(count);
            else
            {
                TCharacterItem dbItem = new TCharacterItem();
                dbItem.TCharacterID = Owner.Data.ID;
                dbItem.Owner = Owner.Data;
                dbItem.ItemID = itemId;
                dbItem.ItemCount = count;
                Owner.Data.Items.Add(dbItem);
                item = new Item(dbItem);
                Items.Add(itemId, item);
            }

            Log.InfoFormat("{0} Add Item {1} {2}", Owner.Data.ID, itemId, count);
            this.Owner.StatusManager.AddItemChange(itemId, count, StatusAction.Add);
            //DBService.Instance.Save();
        }

        public bool RemoveItem(int itemId, int count)
        {
            if(!Items.ContainsKey(itemId))
                return false;
            Item item = Items[itemId];
            if (item.Count < count)
                return false;
            item.Remove(count);

            Log.InfoFormat("{0} Remove Item {1} {2}", Owner.Data.ID, itemId, count);
            this.Owner.StatusManager.AddItemChange(itemId, count, StatusAction.Delete);
            //DBService.Instance.Save();

            return true;
        }

        public void GetItemInfos(List<NItemInfo> nItemInfos)
        {
            foreach(var item in Items)
            {
                nItemInfos.Add(new NItemInfo() { Id = item.Value.ItemID, Count = item.Value.Count });
            }
        }
    }
}
