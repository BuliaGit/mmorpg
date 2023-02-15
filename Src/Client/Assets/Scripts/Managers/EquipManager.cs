using Common.Data;
using Models;
using Services;
using SkillBridge.Message;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    /// <summary>
    /// 装备管理器
    /// </summary>
    public class EquipManager : Singleton<EquipManager>
    {
        public delegate void OnEquipChangeHandler();
        public event OnEquipChangeHandler OnEquipChanged;
        public Item[] Equips = new Item[(int)EquipSlot.SlotMax];
        byte[] Data;

        public Transform charCamTrans;

        unsafe public void Init(byte[] data)
        {
            this.Data = data;
            this.ParseEquipData(data);
        }


        public  void SetCharCam()
        {
            //设置人物展示相机相对位置
            charCamTrans.localPosition = GameObjectManager.Instance.pc.transform.position + GameObjectManager.Instance.pc.transform.forward * 2.5f + new Vector3(0, 0.9f, 0);
            charCamTrans.localEulerAngles = new Vector3(0, 180 + GameObjectManager.Instance.pc.transform.localEulerAngles.y, 0);
            charCamTrans.localScale = Vector3.one;
            charCamTrans.gameObject.SetActive(true);
        }

        public void CloseCharCam()
        {
            charCamTrans.gameObject.SetActive(false);
        }

        #region 解析or还原 装备数据
        /// <summary>
        /// 解析装备信息
        /// </summary>
        /// <param name="data"></param>
        unsafe void ParseEquipData(byte[] data)
        {
            fixed (byte* pt = data)
            {
                for (int i = 0; i < this.Equips.Length; i++)
                {
                    int itemId = *(int*)(pt + i * sizeof(int));
                    if (itemId > 0)
                        Equips[i] = ItemManager.Instance.Items[itemId];
                    else
                        Equips[i] = null;
                }
            }
        }

        /// <summary>
        /// 还原装备信息（转化为字节数组）
        /// </summary>
        /// <returns></returns>
        unsafe public byte[] GetEquipData()
        {
            fixed (byte* pt = Data)
            {
                for (int i = 0; i < (int)EquipSlot.SlotMax; i++)
                {
                    int* itemId = (int*)(pt + i * sizeof(int));
                    if (Equips[i] == null)
                        *itemId = 0;
                    else
                        *itemId = Equips[i].Id;
                }
            }
            return this.Data;
        }
        #endregion

        #region 功能
        /// <summary>
        /// 检查该槽位是否已经穿戴装备
        /// </summary>
        /// <param name="EquipId"></param>
        /// <returns></returns>
        public bool Contains(int EquipId)
        {
            for (int i = 0; i < this.Equips.Length; i++)
            {
                if (Equips[i] != null && this.Equips[i].Id == EquipId)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 获取某个槽位上的装备信息
        /// </summary>
        /// <param name="slot"></param>
        /// <returns></returns>
        public Item GetEquip(EquipSlot slot)
        {
            return Equips[(int)slot];
        }

        /// <summary>
        /// 获取所有槽位上的装备信息
        /// </summary>
        /// <returns></returns>
        public List<EquipDefine> GetEquipDefines()
        {
            List<EquipDefine> result = new List<EquipDefine>();
            for (int i = 0; i < (int)EquipSlot.SlotMax; i++)
            {
                if (Equips[i] != null)
                    result.Add(Equips[i].EquipInfo);
            }
            return result;
        }

        /// <summary>
        /// 穿戴装备
        /// </summary>
        /// <param name="equip"></param>
        public void EquipItem(Item equip)
        {
            ItemService.Instance.SendEquipItem(equip, true);
        }

        /// <summary>
        /// 卸下装备
        /// </summary>
        /// <param name="equip"></param>
        public void UnEquipItem(Item equip)
        {
            ItemService.Instance.SendEquipItem(equip, false);
        }
        #endregion

        #region 响应消息回调
        public void OnEquipItem(Item equip)
        {
            if (this.Equips[(int)equip.EquipInfo.Slot] != null && this.Equips[(int)equip.EquipInfo.Slot].Id == equip.Id)
            {
                return;
            }

            this.Equips[(int)equip.EquipInfo.Slot] = ItemManager.Instance.Items[equip.Id];

            if (OnEquipChanged != null)
            {
                OnEquipChanged();
            }
        }

        public void OnUnEquipItem(EquipSlot slot)
        {
            if (this.Equips[(int)slot] != null)
            {
                this.Equips[(int)slot] = null;

                if (OnEquipChanged != null)
                {
                    OnEquipChanged();
                }
            }
        }
        #endregion


        private float startRoate = 0;
        public void SetStartRoate()
        {
            startRoate = GameObjectManager.Instance.pc.transform.localEulerAngles.y;
        }

        public void SetPlayerRoate(float roate = 0)
        {
            GameObjectManager.Instance.pc.transform.localEulerAngles = new Vector3(0, startRoate + roate, 0);
        }


    }
}
