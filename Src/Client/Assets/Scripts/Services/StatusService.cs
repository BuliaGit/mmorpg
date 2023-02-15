using Managers;
using Models;
using Network;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Services
{
    class StatusService : Singleton<StatusService>, IDisposable
    {
        public delegate bool StatusNotifyHandler(NStatus status);

        Dictionary<StatusType, StatusNotifyHandler> eventMap = new Dictionary<StatusType, StatusNotifyHandler>();

        HashSet<StatusNotifyHandler> handlers = new HashSet<StatusNotifyHandler>(); //用于校验，不会重复,因为重复进入游戏时，会初始化其他管理器，此时就会注册状态事件，这样就会重复注册同一状态事件，所以用HashSet来校验

        public void Init()
        {

        }

        /// <summary>
        /// 添加状态事件
        /// </summary>
        /// <param name="function"></param>
        /// <param name="action"></param>
        public void RegisterStatusNofity(StatusType function, StatusNotifyHandler action)
        {
            if (handlers.Contains(action))
                return;

            if (!eventMap.ContainsKey(function))
                eventMap[function] = action;
            else
                eventMap[function] += action;

            handlers.Add(action);
        }

        public StatusService()
        {
            MessageDistributer.Instance.Subscribe<StatusNotify>(this.OnStatusNotify);
        }

        public void Dispose()
        {
            MessageDistributer.Instance.Unsubscribe<StatusNotify>(this.OnStatusNotify);
        }

        /// <summary>
        /// 收到状态通知时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="notity"></param>
        private void OnStatusNotify(object sender, StatusNotify notity)
        {
            foreach (NStatus status in notity.Status)
            {
                Notify(status);
            }
        }

        private void Notify(NStatus status)
        {
            Debug.LogFormat("StatusNotify:[{0}[{1}]{2}:{3}]", status.Type, status.Action, status.Id, status.Value);

            if (status.Type == StatusType.Money)
            {
                if (status.Action == StatusAction.Add)
                    User.Instance.AddGold(status.Value);
                else if (status.Action == StatusAction.Delete)
                    User.Instance.AddGold(-status.Value);

                ShopManager.Instance.SetMoney();
                BagManager.Instance.SetMoney();
            }

            StatusNotifyHandler handler;
            if (eventMap.TryGetValue(status.Type, out handler))
            {
                handler(status);
            }
        }
    }
}
