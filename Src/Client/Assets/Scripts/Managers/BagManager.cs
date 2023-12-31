using Models;
using SkillBridge.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public class BagManager : Singleton<BagManager>
    {
        public int Unlocked;
        public BagItem[] Items;
        NBagInfo Info;

        unsafe public void Init(NBagInfo info)
        {
            this.Info = info;
            this.Unlocked = info.Unlocked;
            Items = new BagItem[Unlocked];

            if(info.Items != null && info.Items.Length >= Unlocked)
            {
                Analyze(info.Items);
            }
            else
            {
                Info.Items = new byte[sizeof(BagItem) * Unlocked];
                Reset();
            }
        }

        public void Reset()
        {
            int i = 0;
            foreach(var item in ItemManager.Instance.Items)
            {
                int limitCount = item.Value.itemDefine.StackLimit;
                if (item.Value.Count <= limitCount)
                {
                    Items[i].ItemId = (ushort)item.Key;
                    Items[i].Count = (ushort)item.Value.Count;
                }
                else
                {
                    int count = item.Value.Count;
                    while(count > limitCount)
                    {
                        Items[i].ItemId = (ushort)item.Key;
                        Items[i].Count = (ushort)limitCount;
                        i++;
                        count -= limitCount;
                    }
                    Items[i].ItemId = (ushort)item.Key;
                    Items[i].Count = (ushort)count;
                }
                i++;
            }
        }

        unsafe void Analyze(byte[] data)
        {
            fixed(byte* pt = data)
            {
                for(int i = 0; i < Unlocked; i++)
                {
                    BagItem* item = (BagItem*)(pt + i * sizeof(BagItem));
                    Items[i] = *item;
                }
            }
        }

        unsafe public NBagInfo GetBagInfo()
        {
            fixed (byte* pt = Info.Items)
            {
                for (int i = 0; i < Unlocked; i++)
                {
                    BagItem* item = (BagItem*)(pt + i * sizeof(BagItem));
                    *item = Items[i];
                }
                return Info;
            }
        }

        internal void AddItem(int itemId, int count)
        {
            ushort addCount = (ushort)count;
            for(int i = 0; i < Items.Length; i++)
            {
                if (Items[i].ItemId == itemId)
                {
                    ushort canAdd = (ushort)(DataManager.Instance.Items[itemId].StackLimit - Items[i].Count);
                    if (canAdd >= addCount)
                    {
                        Items[i].Count += addCount;
                        addCount = 0; break;
                    }
                    else
                    {
                        Items[i].Count += canAdd;
                        addCount -= canAdd;
                    }
                }
            }
            if (addCount > 0)
            {
                for (int i = 0; i < Items.Length; i++)
                {
                    if (Items[i].ItemId == 0)
                    {
                        Items[i].ItemId = (ushort)itemId;
                        Items[i].Count = addCount; break;
                    }
                }
            }
        }

        public void RemoveItem(int itemId, int count)
        {

        }
    }
}
