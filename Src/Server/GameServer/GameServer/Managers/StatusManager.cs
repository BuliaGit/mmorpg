using GameServer.Entities;
using SkillBridge.Message;
using System;
using System.Collections.Generic;

namespace GameServer.Managers
{
    class StatusManager
    {
        Character Owner;

        private List<NStatus> Status;

        public bool HasStatus
        {
            get { return this.Status.Count > 0; }
        }

        public StatusManager(Character owner)
        {
            this.Owner = owner;
            this.Status = new List<NStatus>();
        }

        #region 添加状态事件
        //通用
        public void AddStatus(StatusType type, int id, int value, StatusAction action)
        {
            this.Status.Add(new NStatus()
            {
                Type = type,
                Id = id,
                Value = value,
                Action = action
            });
        }

        //具体，为了省事
        public void AddGoldChange(int goldDelta)
        {
            if (goldDelta > 0)
            {
                this.AddStatus(StatusType.Money, 0, goldDelta, StatusAction.Add);
            }
            if (goldDelta < 0)
            {
                this.AddStatus(StatusType.Money, 0, -goldDelta, StatusAction.Delete);
            }
        }

        public void AddItemChange(int id, int count, StatusAction action)
        {
            this.AddStatus(StatusType.Item, id, count, action);
        }

        public void AddExpChange(int val)
        {
            AddStatus(StatusType.Exp,0,val, StatusAction.Add);
        }

        public void AddLevelUp(int val)
        {
            AddStatus(StatusType.Level, 0, val, StatusAction.Add);
        }

        #endregion

        /// <summary>
        /// 添加状态消息
        /// </summary>
        /// <param name="message"></param>
        public void PostProcess(NetMessageResponse message)
        {
            if (message.statusNotify == null)
                message.statusNotify = new StatusNotify();

            foreach (var status in this.Status)
            {
                message.statusNotify.Status.Add(status);
            }

            this.Status.Clear();  //保证唯一性
        }

        
    }
}
