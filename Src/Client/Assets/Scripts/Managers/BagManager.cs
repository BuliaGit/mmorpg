using Models;
using SkillBridge.Message;

namespace Managers
{
    /// <summary>
    /// 背包管理器
    /// </summary>
    class BagManager : Singleton<BagManager>  
    {
        public int Unlocked;
        public BagItem[] items;
        NBagInfo Info;

        public UIBag uiBag;

        unsafe public void Init(NBagInfo info)
        {
            this.Info = info;
            this.Unlocked = info.Unlocked;
            items = new BagItem[this.Unlocked];
            if (info.Items != null && info.Items.Length >= this.Unlocked)
            {
                Anylyze(info.Items);
            }
            else
            {
                //第一次登录
                int v = sizeof(BagItem);
                Info.Items = new byte[v * this.Unlocked];
                Reset();
            }
        }

        /// <summary>
        /// 背包整理
        /// </summary>
        public void Reset()
        {
            int i = 0;
            foreach (var kv in ItemManager.Instance.Items)
            {
                if (kv.Value.Count <= kv.Value.Define.StackLimit)
                {
                    this.items[i].ItemId = (ushort)kv.Key;
                    this.items[i].Count = (ushort)kv.Value.Count;
                }
                else
                {
                    int count = kv.Value.Count;
                    while (count > kv.Value.Define.StackLimit)
                    {
                        this.items[i].ItemId = (ushort)kv.Key;
                        this.items[i].Count = (ushort)kv.Value.Define.StackLimit;
                        i++;
                        count -= kv.Value.Define.StackLimit;
                    }
                    this.items[i].ItemId = (ushort)kv.Key;
                    this.items[i].Count = (ushort)count;
                }
                i++;
            }
        }

        /// <summary>
        /// 将字节数组转化为结构体数组
        /// </summary>
        /// <param name="data"></param>
        unsafe void Anylyze(byte[] data)
        {
            fixed (byte* pt = data) //获取data的指针
            {
                for (int i = 0; i < this.Unlocked; i++)
                {
                    BagItem* item = (BagItem*)(pt + i * sizeof(BagItem));   //开始的指针 + 第几个格子的大小
                    items[i] = *item;
                }
            }
        }

        /// <summary>
        /// 将结构体数组转化为字节数组
        /// </summary>
        /// <returns></returns>
        unsafe public NBagInfo GetNBagInfo()
        {
            fixed (byte* pt = Info.Items)   //c#用指针的格式
            {
                for (int i = 0; i < this.Unlocked; i++)
                {
                    BagItem* item = (BagItem*)(pt + i * sizeof(BagItem));
                    *item = items[i];
                }
            }
            return this.Info;
        }


        public void AddItem(int itemId, int count)
        {
            ushort addCount = (ushort)count;
            for (int i = 0; i < this.Unlocked; i++)
            {
                if (this.items[i].ItemId == itemId)
                {
                    ushort canAdd = (ushort)(DataManager.Instance.Items[itemId].StackLimit - this.items[i].Count);
                    if (canAdd >= addCount)
                    {
                        this.items[i].Count += addCount;
                        addCount = 0;
                        break;
                    }
                    else
                    {
                        this.items[i].Count += canAdd;
                        addCount -= canAdd;
                    }
                }
            }
            if (addCount > 0)
            {
                for (int i = 0; i < this.items.Length; i++)
                {
                    if (this.items[i].ItemId == 0)
                    {
                        this.items[i].ItemId = (ushort)itemId;
                        this.items[i].Count = addCount;
                        break;
                    }
                }
            }
        }

        public void RemoveItem(int itemId, int count)
        {
           
        }

        internal void SetMoney()
        {
            if (uiBag != null)
            {
                this.uiBag.money.text = User.Instance.CurrentCharacterInfo.Gold.ToString();
            }            
        }
    }
}
