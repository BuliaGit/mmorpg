using System.Runtime.InteropServices;

namespace Models
{
    /// <summary>
    /// 背包项类
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)] //结构布局：结构体内存布局
    struct BagItem  //这里用结构体，是因为它是值类型，便于背包格子交换
    {
        public ushort ItemId;   //这两个大小相当一个int大小 2+2=4 
        public ushort Count;

        public static BagItem zero = new BagItem { ItemId = 0, Count = 0 };

        public BagItem(int itemId, int count)
        {
            this.ItemId = (ushort)itemId;
            this.Count = (ushort)count;
        }


        public static bool operator ==(BagItem lhs, BagItem rhs)
        {
            return lhs.ItemId == rhs.ItemId && lhs.Count == rhs.Count;
        }
        public static bool operator !=(BagItem lhs, BagItem rhs)
        {
            return !(lhs == rhs);
        }
        public override bool Equals(object other)
        {
            if (other is BagItem)
            {
                return Equals((BagItem)other);
            }
            return false;
        }
        public bool Equals(BagItem other)
        {
            return this == other;
        }
        public override int GetHashCode()
        {
            return ItemId.GetHashCode() ^ (Count.GetHashCode() << 2);
        }
    }
}
