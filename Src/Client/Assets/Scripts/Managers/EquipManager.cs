using Models;
using Services;
using SkillBridge.Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public class EquipManager : Singleton<EquipManager>
    {
        public delegate void OnEquipChangeHandler();
        public event OnEquipChangeHandler OnEquipChanged;

        public Item[] Equips = new Item[(int)EquipSlot.SlotMax];
        byte[] Data;

        public void Init(byte[] data)
        {
            Data = data;
            ParseEquipData(data);
        }

        public bool Contains(int equipId)
        {
            for(int i = 0; i < Equips.Length; i++)
            {
                if (Equips[i] != null && Equips[i].Id == equipId)
                    return true;
            }
            return false;
        }

        public Item GetEquip(EquipSlot slot)
        {
            return Equips[(int)slot];
        }

        unsafe void ParseEquipData(byte[] data)
        {
            fixed (byte* pt = data)
            {
                for(int i = 0; i < Equips.Length; i++)
                {
                    int itemId = *(int*)(pt + i * sizeof(int));
                    if(itemId > 0)
                        Equips[i] = ItemManager.Instance.Items[itemId];
                    else
                        Equips[i] = null;
                }
            }
        }

        unsafe public byte[] GetEquipData()
        {
            fixed (byte* pt = Data)
            {
                for(int i = 0; i < (int)EquipSlot.SlotMax; i++)
                {
                    int* itemId = (int*)(pt + i * sizeof(int));
                    if (Equips[i] == null)
                        *itemId = 0;
                    else
                        *itemId = Equips[i].Id;
                }
            }
            return Data;
        }

        public void EquipItem(Item equip)
        {
            ItemService.Instance.SendEquipItem(equip, true);
        }
        public void UnEquipItem(Item equip)
        {
            ItemService.Instance.SendEquipItem(equip, false);
        }

        public void OnEquipItem(Item equip)
        {
            if (Equips[(int)equip.equipDefine.Slot] != null && Equips[(int)equip.equipDefine.Slot].Id == equip.Id) return;

            Equips[(int)equip.equipDefine.Slot] = ItemManager.Instance.Items[equip.Id];
            if (OnEquipChanged != null)
                OnEquipChanged();
        }

        public void OnUnEquipItem(EquipSlot slot)
        {
            if (Equips[(int)slot] != null)
            {
                Equips[(int)slot] = null;
                if (OnEquipChanged != null)
                    OnEquipChanged();
            }
        }
    }
}
