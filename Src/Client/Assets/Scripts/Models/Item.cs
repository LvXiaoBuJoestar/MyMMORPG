using Common.Data;
using SkillBridge.Message;
using System;

namespace Models
{
    public class Item
    {
        public int Id;
        public int Count;
        public ItemDefine itemDefine;

        public Item(NItemInfo item)
        {
            Id = item.Id;
            Count = item.Count;
            itemDefine = DataManager.Instance.Items[Id];
        }

        public override string ToString()
        {
            return String.Format("Id:[{0}], Count:[{1}]", Id, Count);
        }
    }
}
