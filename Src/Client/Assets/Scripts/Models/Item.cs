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
        public EquipDefine equipDefine;

        public Item(NItemInfo item) : 
            this(item.Id, item.Count)
        {

        }

        public Item(int id, int count)
        {
            this.Id = id;
            this.Count = count;
            DataManager.Instance.Items.TryGetValue(id, out itemDefine);
            DataManager.Instance.Equips.TryGetValue(id, out equipDefine);
        }

        public override string ToString()
        {
            return String.Format("Id:[{0}], Count:[{1}]", Id, Count);
        }
    }
}
