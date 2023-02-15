using Common;
using GameServer.Entities;
using GameServer.Services;
using Network;
using SkillBridge.Message;

namespace GameServer.Managers
{
    class EquipManager : Singleton<EquipManager>
    {
        /// <summary>
        /// 穿戴装备
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="slot"></param>
        /// <param name="itemId"></param>
        /// <param name="isEquip">是否穿上</param>
        /// <returns></returns>

        public Result EquipItem(NetConnection<NetSession> sender, int slot, int itemId, bool isEquip)
        {
            Character charactor = sender.Session.Character;
            if (!charactor.ItemManager.Items.ContainsKey(itemId))
                return Result.Failed;

            UpdateEquip(charactor.Data.Equips, slot, itemId, isEquip);

            DBService.Instance.Save();
            return Result.Success;
        }

        /// <summary>
        /// 更新装备
        /// </summary>
        /// <param name="equipData"></param>
        /// <param name="slot"></param>
        /// <param name="itemId"></param>
        /// <param name="isEquIp"></param>
        unsafe void UpdateEquip(byte[] equipData, int slot, int itemId, bool isEquIp)
        {
            fixed (byte* pt = equipData)
            {
                int* slotId = (int*)(pt + slot * sizeof(int));
                if (isEquIp)
                    *slotId = itemId;
                else
                    *slotId = 0;
            }
        }
    }
}
