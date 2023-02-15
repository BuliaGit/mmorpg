using Common;
using GameServer.Entities;
using GameServer.Models;
using SkillBridge.Message;
using System.Collections.Generic;

namespace GameServer.Managers
{
    class ItemManager
    {
        Character Owner;

        public Dictionary<int, Item> Items = new Dictionary<int, Item>();

        public ItemManager(Character owner)
        {
            this.Owner = owner;

            foreach (var item in owner.Data.Items)
            {
                this.Items.Add(item.ItemID, new Item(item));
            }
        }

        /// <summary>
        /// 使用道具
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public bool UseItem(int itemId, int count = 1)
        {
            Log.InfoFormat("[{0}]UseItem[{1}:{2}]", this.Owner.Data.ID, itemId, count);

            Item item = null;
            if (this.Items.TryGetValue(itemId, out item))
            {
                if (item.Count < count)
                {
                    return false;
                }

                //Todo 增加使用逻辑

                item.Remove(count);

                return true;
            }
            return false;
        }

        /// <summary>
        /// 道具是否存在
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public bool HasItem(int itemId)
        {
            Item item = null;
            if (this.Items.TryGetValue(itemId, out item))
            {
                return item.Count > 0;
            }
            return false;
        }

        /// <summary>
        /// 获取道具
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public Item GetItem(int itemId)
        {
            Item item = null;
            this.Items.TryGetValue(itemId, out item);
            Log.InfoFormat("[{0}]GetItem[{1}:{2}]", this.Owner.Data.ID, itemId, item);
            return item;
        }

        /// <summary>
        /// 增加道具
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public bool AddItem(int itemId, int count = 1)
        {
            Item item = null;
            if (this.Items.TryGetValue(itemId, out item))
            {
                item.Add(count);
            }
            else
            {
                TCharacterItem dbItem = new TCharacterItem();
                dbItem.CharacterID = Owner.Data.ID;
                dbItem.Owner = Owner.Data;
                dbItem.ItemID = itemId;
                dbItem.ItemCount = count;

                Owner.Data.Items.Add(dbItem);   //添加到数据库

                item = new Item(dbItem);
                this.Items.Add(itemId, item);
            }

            this.Owner.StatusManager.AddItemChange(itemId, count, StatusAction.Add);

            Log.InfoFormat("[{0}]AddItem[{1}:{2}]", this.Owner.Data.ID, itemId, count);
            return true;
        }

        /// <summary>
        /// 道具删除
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public bool RemoveItem(int itemId, int count = 1)
        {
            if (!this.Items.ContainsKey(itemId))
            {
                return false;
            }

            Item item = this.Items[itemId];
            if (item.Count < count)
                return false;
            item.Remove(count);

            this.Owner.StatusManager.AddItemChange(itemId, count, StatusAction.Delete);

            Log.InfoFormat("[{0}]AddItem[{1}:{2}]", this.Owner.Data.ID, itemId, count);
            return true;
        }

        /// <summary>
        /// 获取道具列表（从内存数据转换到网络数据）
        /// </summary>
        /// <param name="list"></param>
        public void GetItemInfos(List<NItemInfo> list)
        {
            foreach (var item in this.Items)
            {
                list.Add(new NItemInfo() { Id = item.Value.ItemID, Count = item.Value.Count });
            }
        }
    }
}
