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

        public Item(NItemInfo item) : 
            this(item.Id, item.Count)
        {

        }

        public Item(int id, int count)
        {
            this.Id = id;
            this.Count = count;
            itemDefine = DataManager.Instance.Items[id];
        }

        public override string ToString()
        {
            return String.Format("Id:[{0}], Count:[{1}]", Id, Count);
        }
    }
}
