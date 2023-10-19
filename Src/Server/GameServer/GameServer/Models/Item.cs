using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Models
{
     internal class Item
     {
        TCharacterItem dbItem;

        public int ItemID;
        public int Count;

        public Item(TCharacterItem characterItem)
        {
            this.dbItem = characterItem;

            this.ItemID = characterItem.ItemID;
            this.Count = characterItem.ItemCount;
        }

        public void Add(int count)
        {
            Count += count;
            dbItem.ItemCount = Count;
        }

        public void Remove(int count)
        {
            Count -= count;
            dbItem.ItemCount = Count;
        }

        public bool Use(int count = 1)
        {
            return false;
        }

        public override string ToString()
        {
            return String.Format("ID:{0}  Count:{1}", ItemID, Count);
        }
    }
}
