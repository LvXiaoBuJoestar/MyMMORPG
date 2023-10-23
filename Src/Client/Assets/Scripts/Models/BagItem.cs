using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Models
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BagItem
    {
        public ushort ItemId;
        public ushort Count;

        public static BagItem zero = new BagItem() { ItemId = 0, Count = 0 };

        public BagItem(int itemId, int count)
        {
            ItemId = (ushort)itemId;
            Count = (ushort)count;
        }

        public static bool operator ==(BagItem lhs, BagItem rhs)
        {
            return lhs.ItemId == rhs.ItemId && lhs.Count == rhs.Count;
        }
        public static bool operator !=(BagItem lhs, BagItem rhs)
        {
            return !(lhs == rhs);
        }

        public bool Equals(BagItem other)
        {
            return other == this;
        }
        public override bool Equals(object obj)
        {
            if(obj is BagItem)
            {
                return Equals((BagItem)obj);
            }
            return false;
        }
        public override int GetHashCode()
        {
            return ItemId.GetHashCode() ^ (Count.GetHashCode() << 2);
        }
    }
}
